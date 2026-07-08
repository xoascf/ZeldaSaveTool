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

	public File(string filePath) {
		if (!(ValidSave = HasValidSize(filePath)))
			return;

		_preConvertedData = PreConvert(IO.GetFileBytes(filePath));
		FormatUsed = GetFormat(_preConvertedData);
		SoundMode = _preConvertedData[0];
		ZTargetingMode = _preConvertedData[1];
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

	private static Format GetFormat(byte[] input) =>
		input.Get(0x87, 1)[0] == 0 ? Format.PcPortSav : Format.N64Save;

	public bool HasValidSize(string path) {
		if (!IO.Exists(path)) return false;

		long length = IO.GetFileLength(path);

		switch (length) {
			case MaxSize:
				return true;

			case MinSize:
				IsOpenOotSave = true;
				return true;

			case SRMSize:
				IsSRMSave = true;
				return true;

			default:
				/* GCI (Dolphin): 0x40-byte directory header + N whole 0x2000-byte data blocks */
				if ((length - GCIHeaderSize) % GCIBlockSize == 0 && length >= GCIMinFileSize) {
					IsGCISave = true;
					return true;
				}
				Message.New(Message.Level.E, T("Wrong_Size"));
				return false;
		}
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
		if (IsGCISave)
			GetN64FromGCI(ref data);
		else if (IsSRMSave)
			GetN64FromSRM(ref data);
		else
			data.ToBigEndian();

		if (IsOpenOotSave)
			Array.Resize(ref data, MaxSize);

		FixName(ref data, 0x0044);
		FixName(ref data, 0x1494);
		FixName(ref data, 0x28E4);

		Slot1.Name = Charset.GetReadableName(data.Get(0x0044, 8));
		Slot2.Name = Charset.GetReadableName(data.Get(0x1494, 8));
		Slot3.Name = Charset.GetReadableName(data.Get(0x28E4, 8));

		return data;
	}

	public void NormalizeNames() {
		if (Slot1.Name != null) _saveData.Set(0x0044, Charset.GetNameBytes(Slot1.Name));
		if (Slot2.Name != null) _saveData.Set(0x1494, Charset.GetNameBytes(Slot2.Name));
		if (Slot3.Name != null) _saveData.Set(0x28E4, Charset.GetNameBytes(Slot3.Name));

		FixName(ref _saveData, 0x0044, ToNTSC);
		FixName(ref _saveData, 0x1494, ToNTSC);
		FixName(ref _saveData, 0x28E4, ToNTSC);
	}

	public byte[] ConvertSave() {
		Array.Resize(ref _saveData, MaxSize);
		_preConvertedData.CopyTo(_saveData, 0);
		NormalizeNames();
		_saveData.Set(0, SoundMode);
		_saveData.Set(1, ZTargetingMode);

		bool be = FormatUsed != Format.PcPortSav;
		bool exportBe = FormatExport != Format.PcPortSav;

		Structs.Oot.Save save1 = Reader.ByteToType<Structs.Oot.Save>(_saveData.Get(0x20, 0x1354), be);
		save1.info.playerData.deaths = (ushort)Slot1.DeathCount;
		save1.info.playerData.healthCapacity = Slot1.HeartsTotal;
		save1.info.playerData.health = Slot1.HeartsCount;
		save1.info.playerData.isDoubleDefenseAcquired = (byte)(Slot1.DoubleDefense ? 1 : 0);
		save1.info.inventory.defenseHearts = (sbyte)(Slot1.DoubleDefense ? 0x14 : 0x00);
		_saveData.Set(0x20, Reader.TypeToByte(save1, exportBe));

		Structs.Oot.Save save2 = Reader.ByteToType<Structs.Oot.Save>(_saveData.Get(0x1470, 0x1354), be);
		save2.info.playerData.deaths = (ushort)Slot2.DeathCount;
		save2.info.playerData.healthCapacity = Slot2.HeartsTotal;
		save2.info.playerData.health = Slot2.HeartsCount;
		save2.info.playerData.isDoubleDefenseAcquired = (byte)(Slot2.DoubleDefense ? 1 : 0);
		save2.info.inventory.defenseHearts = (sbyte)(Slot2.DoubleDefense ? 0x14 : 0x00);
		_saveData.Set(0x1470, Reader.TypeToByte(save2, exportBe));

		Structs.Oot.Save save3 = Reader.ByteToType<Structs.Oot.Save>(_saveData.Get(0x28C0, 0x1354), be);
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
			} else if (b > (int)Charset.Chars.N9 && b != (int)Charset.Chars.Space)
				switch (b) {
					// Has NTSC-U charset.
					case >= (int)Charset.Chars.AaA + (int)Charset.Chars.NtscLatin:
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

						break;

					// Has NTSC-J charset.
					case >= (int)Charset.Chars.Unk0:
						b = (int)Charset.Chars.Dot; // Replace with a dot for now.
						break;
				}

			newNameData[i] += b;
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