/* Licensed under the Open Software License version 3.0 */

using ZeldaSaveTool.Utility;

namespace ZeldaSaveTool.Save;

internal class File /* format */ {
	public enum Format { N64Save, PcPortSav }
	public enum Sound { Stereo, Mono, Headset, Surround }
	public enum ZTargeting { Switch, Hold } // Target mode

	public Format? FormatUsed { get; set; }
	public Format? FormatExport { get; set; }
	public bool OverwriteBackups { get; set; }
	public bool ToNTSC { get; set; }
	public bool AlternateChecksum { get; set; }
	public bool ValidSave { get; set; }
	public byte SoundMode { get; set; }
	public byte ZTargetingMode { get; set; }

	public Slot Slot1;
	public Slot Slot2;
	public Slot Slot3;

	private readonly byte[] _preConvertedData = Zero;
	private byte[] _saveData = Zero;
	private bool _isExtractedFromRam;

	public File(string filePath) {
		if (!IO.Exists(filePath)) {
			ValidSave = false;
			return;
		}

		_preConvertedData = PreConvert(IO.GetFileBytes(filePath));

		if (!(ValidSave = HasValidSize(_preConvertedData.Length)))
			return;
		FormatUsed = GetFormat(_preConvertedData, IO.GetFileBytes(filePath).Length);
		SoundMode = _preConvertedData[0] <= 3 ? _preConvertedData[0] : (byte)0;
		ZTargetingMode = _preConvertedData[1] <= 1 ? _preConvertedData[1] : (byte)0;
		bool be = FormatUsed != Format.PcPortSav;

		Structs.Oot.Save save1 = Reader.ByteToType<Structs.Oot.Save>(_preConvertedData.Get(0x20, 0x1354), be);
		Slot1.DeathCount = (short)save1.info.playerData.deaths;
		Slot1.HeartsTotal = save1.info.playerData.healthCapacity;
		Slot1.HeartsCount = save1.info.playerData.health;
		Slot1.DoubleDefense = save1.info.playerData.isDoubleDefenseAcquired != 0;

		Structs.Oot.Save save2 = Reader.ByteToType<Structs.Oot.Save>(_preConvertedData.Get(0x1470, 0x1354), be);

		Slot2.DeathCount = (short)save2.info.playerData.deaths;
		Slot2.HeartsTotal = save2.info.playerData.healthCapacity;
		Slot2.HeartsCount = save2.info.playerData.health;
		Slot2.DoubleDefense = save2.info.playerData.isDoubleDefenseAcquired != 0;

		Structs.Oot.Save save3 = Reader.ByteToType<Structs.Oot.Save>(_preConvertedData.Get(0x28C0, 0x1354), be);

		Slot3.DeathCount = (short)save3.info.playerData.deaths;
		Slot3.HeartsTotal = save3.info.playerData.healthCapacity;
		Slot3.HeartsCount = save3.info.playerData.health;
		Slot3.DoubleDefense = save3.info.playerData.isDoubleDefenseAcquired != 0;
	}

	private bool IsOpenOotSave { get; set; }
	private bool IsGCISave { get; set; }
	private bool IsSRMSave { get; set; }

	private const int MaxSize = 0x8000; // Default save file size.
	private const int MinSize = 0x7A00; // Used in Open Ocarina.
	private const int SRMSize = 0x48800; // SaveRAM save file size.

	// GCI (Dolphin) format constants
	private const int GCIHeaderSize  = 0x40; // Directory-entry header size.
	private const int GCIBlockSize   = 0x2000; // Size of each data block.
	private const int GCISaveOffset  = 0x6044; // Offset of N64 save data within the GCI file.
	// 0x20 (global header) + 3 * 0x1450 (slots) + 0x4 (skip between slot 2 and 3)
	private const int GCISaveLength  = 0x20 + 3 * 0x1450 + 0x4;
	private const int GCIMinFileSize = GCISaveOffset + GCISaveLength; // Minimum valid GCI size.

	private Format GetFormat(byte[] input, int length) {
		if (length == MinSize) return Format.PcPortSav;
		// TODO: Some sizes check was removed, so checking for this in this moment is not right.
		return input[0x87] == 0 ? Format.PcPortSav : Format.N64Save;
	}

	public bool HasValidSize(int length) {
		if (length == MaxSize) return true;

		Message.New(Message.Level.E, T("Wrong_Size"));
		return false;
	}

	private class SramCandidate {
		public byte[] Data = Zero;
		public bool IsMultiSlot;
	}

	private SramCandidate ExtractSramFromRamDump(byte[] data) {
		// Handle ZIP compressed memory dumps
		if (data.Length > 4 && data[0] == 0x50 && data[1] == 0x4B && data[2] == 0x03 && data[3] == 0x04) {
			try {
				byte[] unzipped = IO.DecompressZip(data);
				if (unzipped.Length > 0) data = unzipped;
			} catch {
				// Ignore and proceed
			}
		}

		// Handle GZIP compressed memory dumps
		if (data.Length > 2 && data[0] == 0x1F && data[1] == 0x8B) {
			try {
				data = IO.DecompressGzip(data);
			} catch {
				// ""
			}
		}

		// Minimum size for a memory dump is larger than SRAM
		if (data.Length <= MaxSize) return new SramCandidate { Data = data, IsMultiSlot = true };

		// Collect all candidate SRAM blocks found in the dump.
		List<SramCandidate> candidates = new();

		for (int i = 0; i < data.Length - MaxSize - 8; i += 4) {
			byte[] fourBytes = data.Get(i, 4);
			ByteOrder type = Convert.Identify(fourBytes, Convert.SaveMagic);
			if (type == ByteOrder.Unknown) continue;
			if (!CheckZeldazPattern(data, i, type)) continue;

			byte[] sram = data.Get(i - 0x3C, MaxSize);
			byte[] converted = sram.ToBigEndian(type);

			// Check if there is also a ZELDAZ at slot-2 offset (full SRAM block).
			// If so, skip ahead past the remaining slots to avoid adding them individually.
			int slot2ZeldOffset = i + 0x1450;
			bool isMultiSlot = false;
			if (slot2ZeldOffset + 4 < data.Length) {
				byte[] slot2Bytes = data.Get(slot2ZeldOffset, 4);
				ByteOrder slot2Type = Convert.Identify(slot2Bytes, Convert.SaveMagic);
				if (slot2Type == type) isMultiSlot = true;
			}

			// De-duplicate: only add if no existing candidate has the same bytes.
			bool isDuplicate = false;
			foreach (SramCandidate existing in candidates) {
				if (existing.Data.Matches(converted)) {
					isDuplicate = true;
					break;
				}
			}

			if (!isDuplicate)
				candidates.Add(new SramCandidate { Data = converted, IsMultiSlot = isMultiSlot });

			// If this was a multi-slot SRAM block, skip past all its slots.
			if (isMultiSlot)
				i += 0x1450 * 5; // Skip past all 6 slot/backup ZELDAZ patterns.
		}

		if (candidates.Count == 0)
			return new SramCandidate { Data = data, IsMultiSlot = true };

		// The scan finds candidates in file order (oldest first in a save state).
		// Reverse so candidates[0] is the most recently saved state (last occurrence in file).
		candidates.Reverse();

		if (candidates.Count == 1)
			return candidates[0];

		// Multiple distinct SRAM blocks found, guess we gotta ask the user which one to load.
		return PromptSramSelection(candidates);
	}

	private static SramCandidate PromptSramSelection(List<SramCandidate> candidates) {
		using Form form = new() {
			Text = T("Select_Save"),
			Width = 380,
			Height = 300,
			StartPosition = FormStartPosition.CenterParent,
			FormBorderStyle = FormBorderStyle.FixedDialog,
			MaximizeBox = false,
			MinimizeBox = false,
		};

		ListBox listBox = new() {
			Dock = DockStyle.Fill,
			Font = new System.Drawing.Font("Consolas", 10),
		};

		for (int c = 0; c < candidates.Count; c++) {
			byte[] sram = candidates[c].Data;
			string name1 = Charset.GetReadableName(sram.Get(0x0044, 8));
			string name2 = Charset.GetReadableName(sram.Get(0x1494, 8));
			string name3 = Charset.GetReadableName(sram.Get(0x28E4, 8));
			if (name1.IsNullOrWhiteSpace()) name1 = "---";
			if (name2.IsNullOrWhiteSpace()) name2 = "---";
			if (name3.IsNullOrWhiteSpace()) name3 = "---";
			listBox.Items.Add($"#{c + 1}: {name1} | {name2} | {name3}");
		}

		listBox.SelectedIndex = 0;

		Button ok = new() {
			Text = "OK",
			Dock = DockStyle.Bottom,
			DialogResult = DialogResult.OK,
		};

		form.Controls.Add(listBox);
		form.Controls.Add(ok);
		form.AcceptButton = ok;

		int selected = 0;
		if (form.ShowDialog() == DialogResult.OK && listBox.SelectedIndex >= 0)
			selected = listBox.SelectedIndex;

		return candidates[selected];
	}

	private static bool CheckZeldazPattern(byte[] data, int offset, ByteOrder type) {
		int start = offset - 0x3C;
		if (start < 0 || start + MaxSize > data.Length) return false;

		return true;
	}

	public static void GetN64FromGCI(ref byte[] data) {
		if (data.Length < GCIMinFileSize)
			throw new(T("Wrong_Size"));

		byte[] newData = new byte[MaxSize];
		int[] tailToSkip = { 0, 0x4, 0x0 };
		int offset = GCISaveOffset;

		Array.Copy(data, offset, newData, 0, 0x20);
		offset += 0x20;

		for (int i = 0; i < 3; ++i) {
			Array.Copy(data, offset, newData, 0x20 + i * 0x1450, 0x1450);
			offset += 0x1450 + tailToSkip[i];
		}

		data = newData.ToBigEndian();
	}

	public static void GetN64FromSRM(ref byte[] data) {
		byte[] newData;
		try {
			newData = data.Get(0x40800, MaxSize);
			data = newData.ToBigEndian();
		} catch {
			newData = data.Get(0x20800, MaxSize);
			data = newData.ToBigEndian();
		}
	}

	public byte[] PreConvert(byte[] data) {
		int length = data.Length;
		if (length == MinSize)
			IsOpenOotSave = true;
		else if (length == SRMSize)
			IsSRMSave = true;
		else if ((length - GCIHeaderSize) % GCIBlockSize == 0 && length >= GCIMinFileSize)
			IsGCISave = true;

		if (IsGCISave)
			GetN64FromGCI(ref data);
		else if (IsSRMSave)
			GetN64FromSRM(ref data);
		else {
			bool wasLarger = data.Length > MaxSize;
			SramCandidate candidate = ExtractSramFromRamDump(data);
			data = candidate.Data;
			if (data.Length == MaxSize)
				data.ToBigEndian();

			if (wasLarger)
				_isExtractedFromRam = !candidate.IsMultiSlot;
		}

		if (IsOpenOotSave)
			Array.Resize(ref data, MaxSize);

		// Determine the format early so that ToNTSC evaluates correctly for FixName
		FormatUsed = GetFormat(data, length);

		Slot1.Name = Charset.GetReadableName(data.Get(0x0044, 8));
		Slot2.Name = Charset.GetReadableName(data.Get(0x1494, 8));
		Slot3.Name = Charset.GetReadableName(data.Get(0x28E4, 8));

		FixName(ref data, 0x0044, FormatUsed == Format.N64Save);
		FixName(ref data, 0x1494, FormatUsed == Format.N64Save);
		FixName(ref data, 0x28E4, FormatUsed == Format.N64Save);

		return data;
	}

	public void NormalizeNames() {
		if (Slot1.Name != null) _saveData.Set(0x0044, Charset.GetNameBytes(Slot1.Name, ToNTSC));
		if (Slot2.Name != null) _saveData.Set(0x1494, Charset.GetNameBytes(Slot2.Name, ToNTSC));
		if (Slot3.Name != null) _saveData.Set(0x28E4, Charset.GetNameBytes(Slot3.Name, ToNTSC));
	}

	public byte[] ConvertSave() {
		Array.Resize(ref _saveData, MaxSize);
		_preConvertedData.CopyTo(_saveData, 0);
		NormalizeNames();
		_saveData.Set(0, SoundMode);
		_saveData.Set(1, ZTargetingMode);
		_saveData.Set(3, new byte[] { 0x98, 0x09, 0x10, 0x21, (byte)'Z', (byte)'E', (byte)'L', (byte)'D', (byte)'A' });

		bool be = FormatUsed != Format.PcPortSav;
		bool exportBe = FormatExport != Format.PcPortSav;

		Structs.Oot.Save save1 = Reader.ByteToType<Structs.Oot.Save>(_saveData.Get(0x20, 0x1354), be);
		save1.info.playerData.deaths = (ushort)Slot1.DeathCount;
		save1.info.playerData.healthCapacity = Slot1.HeartsTotal;
		save1.info.playerData.health = Slot1.HeartsCount;
		save1.info.playerData.isDoubleDefenseAcquired = (byte)(Slot1.DoubleDefense ? 1 : 0);
		save1.info.inventory.defenseHearts = (sbyte)(Slot1.DoubleDefense ? 0x14 : 0x00);
		if (_isExtractedFromRam)
			save1.cutsceneIndex = 0; // Prevent crash when falling back to savedSceneId from File Select

		_saveData.Set(0x20, Reader.TypeToByte(save1, exportBe));

		Structs.Oot.Save save2;
		if (_isExtractedFromRam)
			save2 = new Structs.Oot.Save(); // Memory dumps only contain Slot 1. Slot 2 is garbage?
		else
			save2 = Reader.ByteToType<Structs.Oot.Save>(_saveData.Get(0x1470, 0x1354), be);

		save2.info.playerData.deaths = (ushort)Slot2.DeathCount;
		save2.info.playerData.healthCapacity = Slot2.HeartsTotal;
		save2.info.playerData.health = Slot2.HeartsCount;
		save2.info.playerData.isDoubleDefenseAcquired = (byte)(Slot2.DoubleDefense ? 1 : 0);
		save2.info.inventory.defenseHearts = (sbyte)(Slot2.DoubleDefense ? 0x14 : 0x00);
		_saveData.Set(0x1470, Reader.TypeToByte(save2, exportBe));

		Structs.Oot.Save save3;
		if (_isExtractedFromRam)
			save3 = new Structs.Oot.Save(); // "" Slot 3 is garbage?
		else
			save3 = Reader.ByteToType<Structs.Oot.Save>(_saveData.Get(0x28C0, 0x1354), be);

		save3.info.playerData.deaths = (ushort)Slot3.DeathCount;
		save3.info.playerData.healthCapacity = Slot3.HeartsTotal;
		save3.info.playerData.health = Slot3.HeartsCount;
		save3.info.playerData.isDoubleDefenseAcquired = (byte)(Slot3.DoubleDefense ? 1 : 0);
		save3.info.inventory.defenseHearts = (sbyte)(Slot3.DoubleDefense ? 0x14 : 0x00);
		_saveData.Set(0x28C0, Reader.TypeToByte(save3, exportBe));

		bool to = !AlternateChecksum;

		/* Update checksum */
		_saveData.Set(0x1352 + 0x20,
			ByteArray.FromU16(GetChecksum(_saveData, 0x20, to), to));
		_saveData.Set(0x1352 + 0x1470,
			ByteArray.FromU16(GetChecksum(_saveData, 0x1470, to), to));
		_saveData.Set(0x1352 + 0x28C0,
			ByteArray.FromU16(GetChecksum(_saveData, 0x28C0, to), to));

		if (OverwriteBackups)
			_saveData = CopyBackupSaves(_saveData);

		if (FormatExport == Format.N64Save)
			_saveData.DataTo(ByteOrder.LittleEndian, 0, MaxSize);

		return _saveData;
	}

	private static void FixName(ref byte[] save, int offset, bool toNtsc = false) {
		byte[] nameData = save.Get(offset, 8);
		byte[] newNameData = new byte[8];

		for (int i = 0; i < nameData.Length; i++) {
			byte b = nameData[i];
			if (toNtsc) {
				switch (b) {
					case (int)Charset.Chars.Dash:
						b = (int)Charset.Chars.NtscDash;
						break;

					case (int)Charset.Chars.Dot:
						b = (int)Charset.Chars.NtscDot;
						break;

					case > (int)Charset.Chars.N9 and <= (int)Charset.Chars.Unk0:
						b += (int)Charset.Chars.NtscLatin;
						break;
				}
			} else if (b > (int)Charset.Chars.N9 && b != (int)Charset.Chars.Space) {
				// Has NTSC-U charset.
				if (b >= (int)Charset.Chars.AaA + (int)Charset.Chars.NtscLatin) {
					switch (b) {
						case (int)Charset.Chars.NtscDash:
							b = (int)Charset.Chars.Dash;
							break;
						case (int)Charset.Chars.NtscDot:
							b = (int)Charset.Chars.Dot;
							break;
						default:
							b -= (int)Charset.Chars.NtscLatin;
							break;
					}
				}
				// Preserve NTSC-J Japanese characters by doing nothing if b is between Unk0 and NtscLatin boundaries.
			}

			newNameData[i] = b;
		}

		save.Set(offset, newNameData);
	}

	public static ushort SwapEndian(ushort val) => (ushort)(val << 8 | val >> 8);

	private static ushort GetChecksum(byte[] saveBytes, int offset, bool shouldSwap) {
		ushort checksum = 0;
		for (int i = 0; i < 0x9A9; i++) {
			ushort chk = saveBytes.ToU16(offset + i * 2, false);
			checksum += shouldSwap ? SwapEndian(chk) : chk;
		}

		return checksum;
	}

	/*   This function will overwrite save data backups!   */
	/* Be aware! Emulator saves shouldn't have this issue! */
	private static byte[] CopyBackupSaves(byte[] saveBytes) {
		byte[] save1 = saveBytes.Get(0x0020, 0x1450);
		byte[] save2 = saveBytes.Get(0x1470, 0x1450);
		byte[] save3 = saveBytes.Get(0x28C0, 0x1450);

		saveBytes.Set(0x3D10, save1);
		saveBytes.Set(0x5160, save2);
		saveBytes.Set(0x65B0, save3);

		return saveBytes;
	}
}