/* Licensed under the Open Software License version 3.0 */

using ZeldaSaveTool.Utility;

namespace ZeldaSaveTool.Save;

internal class File /* format */ {
	public enum Format { N64Emu, N64Console, PcPortSav, CTRSave, SohJson }
	public enum Sound { Stereo, Mono, Headset, Surround }
	public enum ZTargeting { Switch, Hold } // Target mode
	private bool IsSohJsonSave { get; set; }

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
		Slot1.ScarecrowSong = GetScarecrowSongString(save1.info.scarecrowSpawnSongSet, save1.info.scarecrowSpawnSong);

		Structs.Oot.Save save2 = Reader.ByteToType<Structs.Oot.Save>(_preConvertedData.Get(0x1470, 0x1354), be);

		Slot2.DeathCount = (short)save2.info.playerData.deaths;
		Slot2.HeartsTotal = save2.info.playerData.healthCapacity;
		Slot2.HeartsCount = save2.info.playerData.health;
		Slot2.DoubleDefense = save2.info.playerData.isDoubleDefenseAcquired != 0;
		Slot2.ScarecrowSong = GetScarecrowSongString(save2.info.scarecrowSpawnSongSet, save2.info.scarecrowSpawnSong);

		Structs.Oot.Save save3 = Reader.ByteToType<Structs.Oot.Save>(_preConvertedData.Get(0x28C0, 0x1354), be);

		Slot3.DeathCount = (short)save3.info.playerData.deaths;
		Slot3.HeartsTotal = save3.info.playerData.healthCapacity;
		Slot3.HeartsCount = save3.info.playerData.health;
		Slot3.DoubleDefense = save3.info.playerData.isDoubleDefenseAcquired != 0;
		Slot3.ScarecrowSong = GetScarecrowSongString(save3.info.scarecrowSpawnSongSet, save3.info.scarecrowSpawnSong);
	}

	private bool IsOpenOotSave { get; set; }
	private bool IsGCISave { get; set; }
	private bool IsSRMSave { get; set; }
	private bool Is3DSSave { get; set; }

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

	private const int CTRSaveSize = 0x14DC; // 3DS save file size.

	private Format GetFormat(byte[] input, int length) {
		if (IsSohJsonSave) return Format.SohJson;
		if (length == MinSize) return Format.PcPortSav;
		if (length == CTRSaveSize) return Format.CTRSave;
		
		return input[0x87] == 0 ? Format.PcPortSav : Format.N64Emu;
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

		List<SramCandidate> candidates = new();

		for (int i = 0; i < data.Length - GCISaveLength - 8; i += 4) {
			byte[] fourBytes = data.Get(i, 4);
			ByteOrder type = Convert.Identify(fourBytes, Convert.SaveMagic);
			if (type == ByteOrder.Unknown) continue;
			if (!CheckZeldazPattern(data, i, type)) continue;

			int start = i - 0x3C;
			if (start < 0 || start + GCISaveLength > data.Length) continue;

			byte[] newData = new byte[MaxSize];
			int[] tailToSkip = { 0, 0x4, 0x0 };
			int offset = start;

			Array.Copy(data, offset, newData, 0, 0x20);
			offset += 0x20;

			for (int j = 0; j < 3; ++j) {
				Array.Copy(data, offset, newData, 0x20 + j * 0x1450, 0x1450);
				offset += 0x1450 + tailToSkip[j];
			}

			byte[] converted = newData.ToBigEndian(type);

			int slot2ZeldOffset = i + 0x1450;
			bool isMultiSlot = false;
			if (slot2ZeldOffset + 4 < data.Length) {
				byte[] slot2Bytes = data.Get(slot2ZeldOffset, 4);
				ByteOrder slot2Type = Convert.Identify(slot2Bytes, Convert.SaveMagic);
				if (slot2Type == type) isMultiSlot = true;
			}

			bool isDuplicate = false;
			foreach (SramCandidate existing in candidates) {
				if (existing.Data.Matches(converted)) {
					isDuplicate = true;
					break;
				}
			}

			if (!isDuplicate)
				candidates.Add(new SramCandidate { Data = converted, IsMultiSlot = isMultiSlot });

			if (isMultiSlot)
				i += 0x1450 * 5;
		}

		if (candidates.Count == 0) {
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
			return;
		}

		candidates.Reverse();

		if (candidates.Count == 1) {
			data = candidates[0].Data;
			return;
		}

		data = PromptSramSelection(candidates).Data;
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

	private static readonly byte[] gItemSlots = new byte[] {
		0, 1, 2, 3, 4, 5, 6, 7, 7, 8, 9, 9, 10, 11, 12, 13, 14, 15, 16, 17, // 0-19
		18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18,                 // 20-32 (Bottles)
		23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,                     // 33-44 (Child Trade)
		22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22                          // 45-55 (Adult Trade)
	};

	private static Structs.Oot.ItemEquips ConvertEquips(Structs.Oot3D.ItemEquips equips3D, byte[] items64) {
		Structs.Oot.ItemEquips equips64 = new();
		equips64.buttonItems = new byte[4];
		equips64.cButtonSlots = new byte[3] { 0xFF, 0xFF, 0xFF };

		// B Button (Sword)
		equips64.buttonItems[0] = equips3D.buttonItems[0] != 0xFF ? equips3D.buttonItems[0] : (byte)0xFF;

		// C Buttons mapping (3DS Y, X, I, II -> N64 C-Left, C-Down, C-Right)
		int cIndex = 1; // 1 = C-Left, 2 = C-Down, 3 = C-Right
		for (int i = 1; i <= 4 && cIndex <= 3; i++) {
			byte item = equips3D.buttonItems[i];
			if (item == 0xFF) continue;

			// Skip boots (Kokiri: 0x44, Iron: 0x45, Hover: 0x46)
			if (item >= 0x44 && item <= 0x46) continue;
			// Skip swords, shields, tunics
			if (item >= 0x3B && item <= 0x43) continue;

			// Find slot of this item in N64 items
			byte slot = 0xFF;
			for (byte s = 0; s < 24; s++) {
				if (items64[s] == item) {
					slot = s;
					break;
				}
			}

			if (slot != 0xFF) {
				equips64.buttonItems[cIndex] = item;
				equips64.cButtonSlots[cIndex - 1] = slot;
				cIndex++;
			}
		}

		// Fill remaining C-buttons with 0xFF
		for (int i = cIndex; i <= 3; i++) {
			equips64.buttonItems[i] = 0xFF;
			equips64.cButtonSlots[i - 1] = 0xFF;
		}

		equips64.equipment = equips3D.equipment;
		return equips64;
	}

	public static void GetN64From3DS(ref byte[] data) {
		byte[] newData = new byte[MaxSize];
		Structs.Oot3D.Save save3D = Reader.ByteToType<Structs.Oot3D.Save>(data, false);
		Structs.Oot.Save save64 = new();

		save64.info.playerData.newf = new byte[] { (byte)'Z', (byte)'E', (byte)'L', (byte)'D', (byte)'A', (byte)'Z' };
		save64.info.playerData.deaths = save3D.info.playerData.deaths;
		
		string utf16Name = System.Text.Encoding.Unicode.GetString(save3D.info.playerData.playerName);
		string nameStr = "";
		for (int i = 0; i < 8; i++) {
			char c = utf16Name.Length > i ? utf16Name[i] : ' ';
			if (c == '\0') c = ' ';
			nameStr += c;
		}
		save64.info.playerData.playerName = Charset.GetNameBytes(nameStr.TrimEnd(), false);

		save64.info.playerData.healthCapacity = (short)save3D.info.playerData.healthCapacity;
		save64.info.playerData.health = save3D.info.playerData.health;
		save64.info.playerData.magicLevel = save3D.info.playerData.magicLevel;
		save64.info.playerData.magic = save3D.info.playerData.magic;
		save64.info.playerData.rupees = save3D.info.playerData.rupees;
		save64.info.playerData.swordHealth = save3D.info.playerData.bgsHitsLeft;
		save64.info.playerData.naviTimer = save3D.info.playerData.naviTimer;
		save64.info.playerData.isMagicAcquired = save3D.info.playerData.isMagicAcquired;
		save64.info.playerData.isDoubleMagicAcquired = save3D.info.playerData.isDoubleMagicAcquired;
		save64.info.playerData.isDoubleDefenseAcquired = save3D.info.playerData.isDoubleDefenseAcquired;
		save64.info.playerData.bgsFlag = save3D.info.playerData.bgsFlag;

		save64.info.inventory.items = new byte[24];
		for (int i = 0; i < 24; i++) {
			byte item = save3D.info.inventory.items[i];
			if (item == 0xFF) {
				save64.info.inventory.items[i] = 0xFF;
				continue;
			}
			
			bool isValid = false;
			if (item < gItemSlots.Length) {
				int expectedSlot = gItemSlots[item];
				if (expectedSlot == i) {
					isValid = true;
				} else if (expectedSlot == 18 && i >= 18 && i <= 21) {
					// 18 is SLOT_BOTTLE_1. Bottle slots are 18, 19, 20, 21.
					isValid = true;
				}
			}

			if (isValid) {
				save64.info.inventory.items[i] = item;
			} else {
				save64.info.inventory.items[i] = 0xFF; // Wipe invalid item
			}
		}
		save64.info.inventory.ammo = save3D.info.inventory.ammo;
		save64.info.inventory.equipment = save3D.info.inventory.equipment;
		save64.info.inventory.upgrades = save3D.info.inventory.upgrades;
		save64.info.inventory.questItems = save3D.info.inventory.questItems;
		save64.info.inventory.dungeonItems = save3D.info.inventory.dungeonItems;
		save64.info.inventory.dungeonKeys = save3D.info.inventory.dungeonKeys;
		save64.info.inventory.defenseHearts = save3D.info.inventory.defenseHearts;
		save64.info.inventory.gsTokens = save3D.info.inventory.gsTokens;

		save64.info.sceneFlags = new uint[868];
		for (int i = 0; i < 124; i++) {
			save64.info.sceneFlags[i * 7 + 0] = save3D.info.sceneFlags[i].chest;
			save64.info.sceneFlags[i * 7 + 1] = save3D.info.sceneFlags[i].swch;
			save64.info.sceneFlags[i * 7 + 2] = save3D.info.sceneFlags[i].clear;
			save64.info.sceneFlags[i * 7 + 3] = save3D.info.sceneFlags[i].collect;
			save64.info.sceneFlags[i * 7 + 4] = save3D.info.sceneFlags[i].unk;
			save64.info.sceneFlags[i * 7 + 5] = save3D.info.sceneFlags[i].rooms1;
			save64.info.sceneFlags[i * 7 + 6] = save3D.info.sceneFlags[i].rooms2;
		}

		save64.entranceIndex = save3D.info.playerData.entranceIndex;
		save64.linkAge = save3D.info.playerData.linkAge;
		save64.cutsceneIndex = save3D.info.playerData.cutsceneIndex;
		save64.dayTime = save3D.info.playerData.dayTime;
		save64.nightFlag = save3D.info.playerData.nightFlag;
		save64.totalDays = save3D.info.playerData.unk_14;
		save64.bgsDayCount = save3D.info.playerData.unk_18;

		save64.info.playerData.savedSceneId = (short)save3D.info.playerData.savedSceneId;
		save64.info.playerData.childEquips = ConvertEquips(save3D.info.playerData.childEquips, save64.info.inventory.items);
		save64.info.playerData.adultEquips = ConvertEquips(save3D.info.playerData.adultEquips, save64.info.inventory.items);
		save64.info.equips = ConvertEquips(save3D.info.equips, save64.info.inventory.items);

		save64.info.eventChkInf = save3D.info.eventChkInf;
		save64.info.itemGetInf = save3D.info.itemGetInf;
		save64.info.infTable = save3D.info.infTable;
		save64.info.worldMapAreaData = save3D.info.worldMapAreaData;

		save64.info.highScores = new int[7];
		save64.info.highScores[0] = (int)save3D.info.horsebackArcheryHighscore;
		save64.info.highScores[1] = BitConverter.ToInt32(save3D.info.unk_ED4, 0);
		save64.info.highScores[2] = BitConverter.ToInt32(save3D.info.unk_ED4, 4);
		save64.info.highScores[3] = (int)save3D.info.horseRaceRecordTime;
		save64.info.highScores[4] = (int)save3D.info.marathonRaceRecordTime;
		save64.info.highScores[5] = BitConverter.ToInt32(save3D.info.unk_EE4, 0);
		save64.info.highScores[6] = BitConverter.ToInt32(save3D.info.unk_EE4, 4);

		byte[] gsFlagsBytes = new byte[28];
		Array.Copy(save3D.info.gsFlags, 0, gsFlagsBytes, 0, 22);
		Array.Copy(save3D.info.unk_ECA, 0, gsFlagsBytes, 22, 6);

		save64.info.gsFlags = new int[6];
		for (int i = 0; i < 6; i++) {
			save64.info.gsFlags[i] = BitConverter.ToInt32(gsFlagsBytes, i * 4);
		}
		save64.info.unk_EB4 = new byte[4];
		Array.Copy(gsFlagsBytes, 24, save64.info.unk_EB4, 0, 4);
		save64.info.fw = new Structs.Oot.FaroresWindData {
			pos = new Structs.Oot.Vec3i {
				x = save3D.info.fw.pos != null && save3D.info.fw.pos.Length >= 3 ? save3D.info.fw.pos[0] : 0,
				y = save3D.info.fw.pos != null && save3D.info.fw.pos.Length >= 3 ? save3D.info.fw.pos[1] : 0,
				z = save3D.info.fw.pos != null && save3D.info.fw.pos.Length >= 3 ? save3D.info.fw.pos[2] : 0,
			},
			yaw = save3D.info.fw.yaw,
			playerParams = save3D.info.fw.playerParams,
			entranceIndex = save3D.info.fw.entranceIndex,
			roomIndex = save3D.info.fw.roomIndex,
			set = save3D.info.fw.set,
			tempSwchFlags = save3D.info.fw.tempSwchFlags,
			tempCollectFlags = save3D.info.fw.tempCollectFlags
		};

		// Initialize required arrays to prevent Marshal crashing or garbage data
		save64.info.unk_E8C = new byte[0x10];
		save64.info.unk_F34 = new byte[4];
		save64.info.unk_F3C = new byte[4];

		byte[] save64Bytes = Reader.TypeToByte(save64, true);
		Array.Copy(save64Bytes, 0, newData, 0x20, save64Bytes.Length);
		data = newData;
	}

	public byte[] PreConvert(byte[] data) {
		int length = data.Length;
		bool isJson = IsJson(data);
		if (isJson)
			IsSohJsonSave = true;
		else if (length == MinSize)
			IsOpenOotSave = true;
		else if (length == SRMSize)
			IsSRMSave = true;
		else if (length == CTRSaveSize)
			Is3DSSave = true;
		else if ((length - GCIHeaderSize) % GCIBlockSize == 0 && length >= GCIMinFileSize)
			IsGCISave = true;

		if (IsGCISave)
			GetN64FromGCI(ref data);
		else if (IsSRMSave)
			GetN64FromSRM(ref data);
		else if (Is3DSSave)
			GetN64From3DS(ref data);
		else if (IsSohJsonSave)
			GetN64FromSohJson(ref data);
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

		if (data.Length < MaxSize)
			return data; // Let HasValidSize reject it later without crashing

		// Determine the format early so that ToNTSC evaluates correctly for FixName
		FormatUsed = GetFormat(data, length);

		Slot1.Name = Charset.GetReadableName(data.Get(0x0044, 8));
		Slot2.Name = Charset.GetReadableName(data.Get(0x1494, 8));
		Slot3.Name = Charset.GetReadableName(data.Get(0x28E4, 8));

		FixName(ref data, 0x0044, FormatUsed == Format.N64Emu || FormatUsed == Format.N64Console);
		FixName(ref data, 0x1494, FormatUsed == Format.N64Emu || FormatUsed == Format.N64Console);
		FixName(ref data, 0x28E4, FormatUsed == Format.N64Emu || FormatUsed == Format.N64Console);

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

		if (be != exportBe) {
			SwapSongLengths(save1.info.scarecrowSpawnSong);
			SwapSongLengths(save1.info.scarecrowLongSong);
		}
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
		if (be != exportBe) {
			SwapSongLengths(save2.info.scarecrowSpawnSong);
			SwapSongLengths(save2.info.scarecrowLongSong);
		}
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
		if (be != exportBe) {
			SwapSongLengths(save3.info.scarecrowSpawnSong);
			SwapSongLengths(save3.info.scarecrowLongSong);
		}
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

		if (FormatExport == Format.N64Emu)
			_saveData.DataTo(ByteOrder.LittleEndian, 0, MaxSize);
		else if (FormatExport == Format.N64Console)
			_saveData.DataTo(ByteOrder.BigEndian, 0, MaxSize);

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

	private void GetN64FromSohJson(ref byte[] data) {
		string jsonString = System.Text.Encoding.UTF8.GetString(data);
		var root = LiteJsonParser.Parse(jsonString) as Dictionary<string, object>;
		if (root == null)
			throw new InvalidOperationException("Invalid JSON save file");

		if (!root.TryGetValue("sections", out object? sectionsObj) || !(sectionsObj is Dictionary<string, object> sections))
			throw new InvalidOperationException("JSON save file is missing 'sections'");

		if (!sections.TryGetValue("base", out object? baseObj) || !(baseObj is Dictionary<string, object> baseSection))
			throw new InvalidOperationException("JSON save file is missing 'sections.base'");

		if (!baseSection.TryGetValue("data", out object? dataObj) || !(dataObj is Dictionary<string, object> gameData))
			throw new InvalidOperationException("JSON save file is missing 'sections.base.data'");

		byte[] newData = new byte[MaxSize];
		Structs.Oot.Save save = new();

		// Root fields of Save
		save.entranceIndex = GetVal<int>(gameData, "entranceIndex");
		save.linkAge = GetVal<int>(gameData, "linkAge");
		save.cutsceneIndex = GetVal<int>(gameData, "cutsceneIndex");
		save.dayTime = GetVal<ushort>(gameData, "dayTime");
		save.nightFlag = GetVal<int>(gameData, "nightFlag");
		save.totalDays = GetVal<int>(gameData, "totalDays");
		save.bgsDayCount = GetVal<int>(gameData, "bgsDayCount");

		// SavePlayerData
		save.info.playerData.newf = new byte[] { (byte)'Z', (byte)'E', (byte)'L', (byte)'D', (byte)'A', (byte)'Z' };
		save.info.playerData.deaths = GetVal<ushort>(gameData, "deaths");
		save.info.playerData.playerName = GetArray<byte>(gameData, "playerName", 8);
		save.info.playerData.n64ddFlag = GetVal<short>(gameData, "n64ddFlag");
		save.info.playerData.healthCapacity = GetVal<short>(gameData, "healthCapacity");
		save.info.playerData.health = GetVal<short>(gameData, "health");
		save.info.playerData.magicLevel = GetVal<sbyte>(gameData, "magicLevel");
		save.info.playerData.magic = GetVal<sbyte>(gameData, "magic");
		save.info.playerData.rupees = GetVal<short>(gameData, "rupees");
		save.info.playerData.swordHealth = GetVal<ushort>(gameData, "swordHealth");
		save.info.playerData.naviTimer = GetVal<ushort>(gameData, "naviTimer");
		save.info.playerData.isMagicAcquired = gameData.ContainsKey("isMagicAcquired") ? GetVal<byte>(gameData, "isMagicAcquired") : GetVal<byte>(gameData, "magicAcquired");
		save.info.playerData.isDoubleMagicAcquired = gameData.ContainsKey("isDoubleMagicAcquired") ? GetVal<byte>(gameData, "isDoubleMagicAcquired") : GetVal<byte>(gameData, "doubleMagic");
		save.info.playerData.isDoubleDefenseAcquired = gameData.ContainsKey("isDoubleDefenseAcquired") ? GetVal<byte>(gameData, "isDoubleDefenseAcquired") : GetVal<byte>(gameData, "doubleDefense");
		save.info.playerData.bgsFlag = GetVal<byte>(gameData, "bgsFlag");
		save.info.playerData.ocarinaGameRoundNum = GetVal<byte>(gameData, "ocarinaGameRoundNum");

		save.info.playerData.unk_3B = new byte[1];
		save.info.playerData.unk_54 = GetVal<uint>(gameData, "unk_54");
		save.info.playerData.unk_58 = new byte[0x0E];
		save.info.playerData.savedSceneId = GetVal<short>(gameData, "savedSceneNum");

		// Equips
		var childEquipsDict = GetDict(gameData, "childEquips");
		save.info.playerData.childEquips.buttonItems = GetArray<byte>(childEquipsDict, "buttonItems", 4);
		save.info.playerData.childEquips.cButtonSlots = GetArray<byte>(childEquipsDict, "cButtonSlots", 3);
		save.info.playerData.childEquips.equipment = GetVal<ushort>(childEquipsDict, "equipment");

		var adultEquipsDict = GetDict(gameData, "adultEquips");
		save.info.playerData.adultEquips.buttonItems = GetArray<byte>(adultEquipsDict, "buttonItems", 4);
		save.info.playerData.adultEquips.cButtonSlots = GetArray<byte>(adultEquipsDict, "cButtonSlots", 3);
		save.info.playerData.adultEquips.equipment = GetVal<ushort>(adultEquipsDict, "equipment");

		var equipsDict = GetDict(gameData, "equips");
		save.info.equips.buttonItems = GetArray<byte>(equipsDict, "buttonItems", 4);
		save.info.equips.cButtonSlots = GetArray<byte>(equipsDict, "cButtonSlots", 3);
		save.info.equips.equipment = GetVal<ushort>(equipsDict, "equipment");

		// Inventory
		var invDict = GetDict(gameData, "inventory");
		save.info.inventory.items = GetArray<byte>(invDict, "items", 24);
		save.info.inventory.ammo = GetArray<sbyte>(invDict, "ammo", 16);
		save.info.inventory.equipment = GetVal<ushort>(invDict, "equipment");
		save.info.inventory.upgrades = GetVal<uint>(invDict, "upgrades");
		save.info.inventory.questItems = GetVal<uint>(invDict, "questItems");
		save.info.inventory.dungeonItems = GetArray<byte>(invDict, "dungeonItems", 20);
		save.info.inventory.dungeonKeys = GetArray<sbyte>(invDict, "dungeonKeys", 19);
		save.info.inventory.defenseHearts = GetVal<sbyte>(invDict, "defenseHearts");
		save.info.inventory.gsTokens = GetVal<short>(invDict, "gsTokens");

		// Scene Flags
		save.info.sceneFlags = new uint[868];
		if (gameData.TryGetValue("sceneFlags", out object? sfVal) && sfVal is List<object> sfList) {
			for (int i = 0; i < Math.Min(124, sfList.Count); i++) {
				if (sfList[i] is Dictionary<string, object> sfDict) {
					save.info.sceneFlags[i * 7 + 0] = GetVal<uint>(sfDict, "chest");
					save.info.sceneFlags[i * 7 + 1] = GetVal<uint>(sfDict, "swch");
					save.info.sceneFlags[i * 7 + 2] = GetVal<uint>(sfDict, "clear");
					save.info.sceneFlags[i * 7 + 3] = GetVal<uint>(sfDict, "collect");
					save.info.sceneFlags[i * 7 + 4] = GetVal<uint>(sfDict, "unk");
					save.info.sceneFlags[i * 7 + 5] = GetVal<uint>(sfDict, "rooms");
					save.info.sceneFlags[i * 7 + 6] = GetVal<uint>(sfDict, "floors");
				}
			}
		}

		// Farore's Wind
		var fwDict = GetDict(gameData, "fw");
		if (fwDict != null) {
			var fwPosDict = GetDict(fwDict, "pos");
			save.info.fw.pos.x = GetVal<int>(fwPosDict, "x");
			save.info.fw.pos.y = GetVal<int>(fwPosDict, "y");
			save.info.fw.pos.z = GetVal<int>(fwPosDict, "z");
			save.info.fw.yaw = GetVal<int>(fwDict, "yaw");
			save.info.fw.playerParams = GetVal<int>(fwDict, "playerParams");
			save.info.fw.entranceIndex = GetVal<int>(fwDict, "entranceIndex");
			save.info.fw.roomIndex = GetVal<int>(fwDict, "roomIndex");
			save.info.fw.set = GetVal<int>(fwDict, "set");
			save.info.fw.tempSwchFlags = GetVal<int>(fwDict, "tempSwchFlags");
			save.info.fw.tempCollectFlags = GetVal<int>(fwDict, "tempCollectFlags");
		}

		// Arrays
		save.info.gsFlags = GetArray<int>(gameData, "gsFlags", 6);
		save.info.highScores = GetArray<int>(gameData, "highScores", 7);
		save.info.eventChkInf = GetArray<ushort>(gameData, "eventChkInf", 14);
		save.info.itemGetInf = GetArray<ushort>(gameData, "itemGetInf", 4);
		save.info.infTable = GetArray<ushort>(gameData, "infTable", 30);

		// Scarecrow Songs
		save.info.scarecrowLongSong = MapScarecrowSong(gameData, "scarecrowLongSong", "scarecrowCustomSong", 0x360);
		save.info.scarecrowSpawnSong = MapScarecrowSong(gameData, "scarecrowSpawnSong", "scarecrowSpawnSong", 0x80);
		save.info.scarecrowLongSongSet = gameData.ContainsKey("scarecrowLongSongSet") ? GetVal<byte>(gameData, "scarecrowLongSongSet") : GetVal<byte>(gameData, "scarecrowCustomSongSet");
		save.info.scarecrowSpawnSongSet = GetVal<byte>(gameData, "scarecrowSpawnSongSet");

		// Horse Data
		var hdDict = GetDict(gameData, "horseData");
		if (hdDict != null) {
			save.info.horseData.sceneId = GetVal<short>(hdDict, "scene");
			var hdPosDict = GetDict(hdDict, "pos");
			save.info.horseData.pos.x = GetVal<short>(hdPosDict, "x");
			save.info.horseData.pos.y = GetVal<short>(hdPosDict, "y");
			save.info.horseData.pos.z = GetVal<short>(hdPosDict, "z");
			save.info.horseData.angle = GetVal<short>(hdDict, "angle");
		}

		// worldMapAreaData
		save.info.worldMapAreaData = GetVal<uint>(gameData, "worldMapAreaData");

		// unk arrays
		save.info.unk_E8C = new byte[0x10];
		save.info.unk_EB4 = new byte[4];
		save.info.unk_F34 = new byte[4];
		save.info.unk_F3C = new byte[4];
		save.info.unk_12A1 = new byte[0x24];
		save.info.unk_1346 = new byte[2];

		byte[] saveBytes = Reader.TypeToByte(save, true);
		Array.Copy(saveBytes, 0, newData, 0x20, saveBytes.Length);
		data = newData;
	}

	private static bool IsJson(byte[] data) {
		if (data == null || data.Length == 0) return false;
		int i = 0;
		while (i < data.Length && (data[i] == 0xEF || data[i] == 0xBB || data[i] == 0xBF || char.IsWhiteSpace((char)data[i]))) {
			i++;
		}
		return i < data.Length && data[i] == '{';
	}

	private static T GetVal<T>(Dictionary<string, object>? dict, string key, T defaultValue = default) {
		if (dict == null || !dict.TryGetValue(key, out object? val) || val == null)
			return defaultValue;
		try {
			if (val is T t)
				return t;
			if (val is bool b) {
				object convertedBool = b ? 1 : 0;
				return (T)System.Convert.ChangeType(convertedBool, typeof(T));
			}
			return (T)System.Convert.ChangeType(val, typeof(T));
		} catch {
			return defaultValue;
		}
	}

	private static T[] GetArray<T>(Dictionary<string, object>? dict, string key, int size) {
		T[] result = new T[size];
		if (dict == null || !dict.TryGetValue(key, out object? val) || val == null)
			return result;

		if (val is List<object> list) {
			for (int i = 0; i < Math.Min(size, list.Count); i++) {
				if (list[i] != null) {
					try {
						result[i] = (T)System.Convert.ChangeType(list[i], typeof(T));
					} catch {
						// Keep default
					}
				}
			}
		}
		return result;
	}

	private static Dictionary<string, object>? GetDict(Dictionary<string, object>? dict, string key) {
		if (dict != null && dict.TryGetValue(key, out object? val) && val is Dictionary<string, object> result)
			return result;
		return null;
	}

	private static byte[] MapScarecrowSong(Dictionary<string, object> dict, string modernKey, string legacyKey, int expectedSize) {
		byte[] result = new byte[expectedSize];
		string key = dict.ContainsKey(modernKey) ? modernKey : legacyKey;

		if (!dict.TryGetValue(key, out object? val) || val == null)
			return result;

		if (val is List<object> list) {
			for (int i = 0; i < list.Count; i++) {
				if (list[i] is Dictionary<string, object> note) {
					int noteSize = 8;
					if (i * noteSize >= expectedSize)
						break;
					ushort unk_02 = GetVal<ushort>(note, "unk_02"); // 16-bit length
					result[i * noteSize + 0] = GetVal<byte>(note, "noteIdx");
					result[i * noteSize + 1] = GetVal<byte>(note, "unk_01"); // Padding
					result[i * noteSize + 2] = (byte)(unk_02 >> 8); // length_msb
					result[i * noteSize + 3] = (byte)(unk_02 & 0xFF); // length_lsb
					result[i * noteSize + 4] = GetVal<byte>(note, "volume");
					result[i * noteSize + 5] = GetVal<byte>(note, "vibrato");
					result[i * noteSize + 6] = GetVal<byte>(note, "tone"); // bend
					result[i * noteSize + 7] = GetVal<byte>(note, "semitone"); // bFlat4Flag
				} else {
					if (i >= expectedSize)
						break;
					byte b = 0;
					try {
						b = (byte)System.Convert.ChangeType(list[i], typeof(byte));
					} catch {
						b = 0;
					}
					int mod = i % 8;
					if (mod == 2) {
						if (i + 1 < expectedSize)
							result[i + 1] = b;
					} else if (mod == 3) {
						result[i - 1] = b;
					} else {
						result[i] = b;
					}
				}
			}
		}
		return result;
	}

	private static string? GetScarecrowSongString(byte setFlag, byte[]? songBytes) {
		if (setFlag == 0 || songBytes == null || songBytes.Length < 128)
			return null;

		var songList = new List<string>();
		for (int i = 0; i < 16; i++) {
			byte noteIdx = songBytes[i * 8];
			if (noteIdx == 0xFF)
				continue;

			string btn = noteIdx switch {
				0 or 1 or 2 or 3 => "Ⓐ",
				4 or 5 or 6 => "▾",
				7 or 8 or 9 => "▸",
				10 or 11 or 12 => "◂",
				13 or 14 or 15 => "▴",
				_ => "?"
			};
			songList.Add(btn);
			if (songList.Count >= 8)
				break;
		}

		if (songList.Count == 0)
			return null;

		return string.Join(" ", songList.ToArray());
	}

	private static void SwapSongLengths(byte[]? songBytes) {
		if (songBytes == null) return;
		for (int i = 0; i < songBytes.Length / 8; i++) {
			byte temp = songBytes[i * 8 + 2];
			songBytes[i * 8 + 2] = songBytes[i * 8 + 3];
			songBytes[i * 8 + 3] = temp;
		}
	}
}