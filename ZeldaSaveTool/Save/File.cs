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
		if (!(ValidSave = IsValid(filePath)))
			return;

		_preConvertedData = PreConvert(IO.GetFileBytes(filePath).ToBigEndian());
		FormatUsed = GetFormat(_preConvertedData);
		SoundMode = _preConvertedData.Byte(0);
		ZTargetingMode = _preConvertedData.Byte(1);
		bool be = FormatUsed != Format.PcPortSav;

		Slot1.DeathCount = _preConvertedData.Get(0x22 + 0x20, 2).ToI16(be);
		Slot1.HeartsTotal = _preConvertedData.Get(0x2E + 0x20, 2).ToI16(be);
		Slot1.HeartsCount = _preConvertedData.Get(0x30 + 0x20, 2).ToI16(be);
		Slot1.DoubleDefense = _preConvertedData.Byte(0x3D + 0x20) != 0;

		Slot2.DeathCount = _preConvertedData.Get(0x22 + 0x1470, 2).ToI16(be);
		Slot2.HeartsTotal = _preConvertedData.Get(0x2E + 0x1470, 2).ToI16(be);
		Slot2.HeartsCount = _preConvertedData.Get(0x30 + 0x1470, 2).ToI16(be);
		Slot2.DoubleDefense = _preConvertedData.Byte(0x3D + 0x1470) != 0;

		Slot3.DeathCount = _preConvertedData.Get(0x22 + 0x28C0, 2).ToI16(be);
		Slot3.HeartsTotal = _preConvertedData.Get(0x2E + 0x28C0, 2).ToI16(be);
		Slot3.HeartsCount = _preConvertedData.Get(0x30 + 0x28C0, 2).ToI16(be);
		Slot2.DoubleDefense = _preConvertedData.Byte(0x3D + 0x28C0) != 0;
	}

	private bool IsOpenOotSave { get; set; }

	private const int MaxSize = 0x8000; // Default save file size.
	private const int MinSize = 0x7A00; // Used in Open Ocarina.

	private static Format GetFormat(byte[] input) =>
		input.Get(0x87, 1)[0] == 0 ? Format.PcPortSav : Format.N64Save;

	public bool IsValid(string path) {
		if (!IO.Exists(path)) return false;

		switch (IO.GetFileLength(path)) {
			case MaxSize:
				return true;

			case MinSize:
				IsOpenOotSave = true;
				return true;

			default:
				Message.New(Message.Level.E, T("Wrong_Size"));
				return false;
		}
	}

	public byte[] PreConvert(byte[] data) {
		if (IsOpenOotSave) Array.Resize(ref data, MaxSize);

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

		_saveData.Set(0x22 + 0x20, ByteArray.FromI16(Slot1.DeathCount, be));
		_saveData.Set(0x2E + 0x20, ByteArray.FromI16(Slot1.HeartsTotal, be));
		_saveData.Set(0x30 + 0x20, ByteArray.FromI16(Slot1.HeartsCount, be));
		_saveData.Set(0x3D + 0x20, Slot1.DoubleDefense);
		_saveData.Set(0xCF + 0x20, Slot1.DoubleDefense ? 0x14 : 0x00);

		_saveData.Set(0x22 + 0x1470, ByteArray.FromI16(Slot2.DeathCount, be));
		_saveData.Set(0x2E + 0x1470, ByteArray.FromI16(Slot2.HeartsTotal, be));
		_saveData.Set(0x30 + 0x1470, ByteArray.FromI16(Slot2.HeartsCount, be));
		_saveData.Set(0x3D + 0x1470, Slot2.DoubleDefense);
		_saveData.Set(0xCF + 0x1470, Slot2.DoubleDefense ? 0x14 : 0x00);

		_saveData.Set(0x22 + 0x28C0, ByteArray.FromI16(Slot3.DeathCount, be));
		_saveData.Set(0x2E + 0x28C0, ByteArray.FromI16(Slot3.HeartsTotal, be));
		_saveData.Set(0x30 + 0x28C0, ByteArray.FromI16(Slot3.HeartsCount, be));
		_saveData.Set(0x3D + 0x28C0, Slot3.DoubleDefense);
		_saveData.Set(0xCF + 0x28C0, Slot3.DoubleDefense ? 0x14 : 0x00);

		if (FormatUsed == Format.N64Save && FormatExport == Format.PcPortSav
			|| FormatUsed == Format.PcPortSav && FormatExport == Format.N64Save) {
			_saveData = FixBytePos(_saveData, 0);
			_saveData = FixBytePos(_saveData, 0x1470 - 0x20);
			_saveData = FixBytePos(_saveData, 0x28C0 - 0x20);
		}

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

	private static byte[] FixBytePos(byte[] save, int offset) {
		int ov = offset + 0x20;

		save.SetSwap(ov);           // Entrance
		save.SetSwap(ov + 0x04);    // Age
		save.SetSwap(ov + 0x08);    // Cutscene
		save.SetSwap(ov + 0x0C, 2); // Time
		save.SetSwap(ov + 0x10);    // Night?
		save.SetSwap(ov + 0x14);    // Days?
		save.SetSwap(ov + 0x18);    // Biggoron days?

		/* MAGICZ (ov + 0x1C, length: 6, skipped) */

		save.SetSwap(ov + 0x22, 2); // Death counter

		/* PLAYERNA (ov + 0x24, length: 8, skipped) */

		save.SetSwap(ov + 0x2C, 2); // Disk flag, is this used for Rando/MQ now?
		save.SetSwap(ov + 0x2E, 2); // Total hearts
		save.SetSwap(ov + 0x30, 2); // Health

		/* Magic meter (ov + 0x32, length: 1, skipped) */
		/* Current magic (ov + 0x33, length: 1, skipped) */

		save.SetSwap(ov + 0x34, 2); // Rupees
		save.SetSwap(ov + 0x36, 2); // Giant's Knife uses?
		save.SetSwap(ov + 0x38, 2); // Navi timer

		/* Magic flag? (ov + 0x3A, length: 1, skipped) */
		/* ?? (ov + 0x3B, length: 1?, skipped) */
		/* Magic flag? (ov + 0x3C, length: 1, skipped) */
		/* Magic flag? (ov + 0x3D, length: 1, skipped) */
		/* Has Biggoron flag? (ov + 0x3E, length: 1, skipped) */
		/* ?? (ov + 0x3F, length: 1, skipped) */

		/* Child equips */
		//save.SetSwap(ov + 0x40);
		//save.SetSwap(ov + 0x40 + 4, 3);
		save.SetSwap(ov + 0x40 + 8, 2);

		/* Adult equips */
		//save.SetSwap(ov + 0x4A);
		//save.SetSwap(ov + 0x4A + 4, 3);
		save.SetSwap(ov + 0x4A + 8, 2);

		/* ??? */
		save.SetSwap(ov + 0x66, 2); // Scene

		/* Equips */
		//save.SetSwap(ov + 0x68);
		//save.SetSwap(ov + 0x68 + 4, 3);
		save.SetSwap(ov + 0x68 + 8, 2);

		/* Inventory */
		//save.SetSwap(ov + 0x74, 24);
		//save.SetSwap(ov + 0x74 + 0x18, 16);
		save.SetSwap(ov + 0x74 + 0x28, 2);
		save.SetSwap(ov + 0x74 + 0x2C);
		save.SetSwap(ov + 0x74 + 0x30);
		//save.SetSwap(ov + 0x74 + 0x34, 20);
		//save.SetSwap(ov + 0x74 + 0x48, 19);
		/* ??? */
		save.SetSwap(ov + 0x74 + 0x5C, 2);

		/* Scene flags loop x124 */
		int sFlagsPos = ov + 0xD4;
		for (int i = 0; i < 124; i++) {
			save.SetSwap(sFlagsPos);
			save.SetSwap(sFlagsPos + 0x04);
			save.SetSwap(sFlagsPos + 0x08);
			save.SetSwap(sFlagsPos + 0x0C);
			save.SetSwap(sFlagsPos + 0x10);
			save.SetSwap(sFlagsPos + 0x14);
			save.SetSwap(sFlagsPos + 0x18);
			sFlagsPos += 0x1C;
		}

		/* Farore's Wind spawn-related? */
		/* s32 Position? */
		save.SetSwap(ov + 0xE64);		// x
		save.SetSwap(ov + 0xE64 + 4);   // y
		save.SetSwap(ov + 0xE64 + 8);   // z
		save.SetSwap(ov + 0xE64 + 12);
		save.SetSwap(ov + 0xE64 + 16);
		save.SetSwap(ov + 0xE64 + 20);
		save.SetSwap(ov + 0xE64 + 24);
		save.SetSwap(ov + 0xE64 + 28);
		save.SetSwap(ov + 0xE64 + 32);
		save.SetSwap(ov + 0xE64 + 36);

		/* Gs x6 */
		int gsPos = ov + 0xE9C;
		for (int i = 0; i < 6; i++) {
			save.SetSwap(gsPos);
			gsPos += 4;
		}

		/* Highscores x7 */
		int hsPos = ov + 0xEB8;
		for (int i = 0; i < 7; i++) {
			save.SetSwap(hsPos);
			hsPos += 4;
		}

		/* Event check info x14 */
		int eventChkPos = ov + 0xED4;
		for (int i = 0; i < 14; i++) {
			save.SetSwap(eventChkPos, 2);
			eventChkPos += 2;
		}

		/* Item get info x4 */
		int infGetPos = ov + 0xEF0;
		for (int i = 0; i < 4; i++) {
			save.SetSwap(infGetPos, 2);
			infGetPos += 2;
		}

		/* Info table loop x30 */
		int infTaPos = ov + 0xEF8;
		for (int i = 0; i < 30; i++) {
			save.SetSwap(infTaPos, 2);
			infTaPos += 2;
		}

		save.SetSwap(ov + 0xF38); // Area arrival

		/* Swap U16 unk_2 (scarecrowSpawnSong) */
		int u2Pos = ov + 0x12C0;
		for (int i = 0; i < 16; i++) {
			save.SetSwap(u2Pos, 2);
			u2Pos += 0x8;
		}

		/* Horse */
		save.SetSwap(ov + 0x1348, 2); // Scene
		/* s16 Position? */
		save.SetSwap(ov + 0x1348 + 0x02, 2); // x
		save.SetSwap(ov + 0x1348 + 0x04, 2); // y
		save.SetSwap(ov + 0x1348 + 0x06, 2); // z
		save.SetSwap(ov + 0x1348 + 0x08, 2);

		return save;
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