/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal class SaveFile
{
	private const int MaxSize = 0x8000; // Default save file size.
	private const int MinSize = 0x7A00; // Used in Open Ocarina.

	public static bool Validate(string filePath)
	{
		if (!Io.ValidateFile(filePath)) return false;

		switch (new FileInfo(filePath).Length)
		{
			case MaxSize:
				return true;

			case MinSize:
				Message.New(Message.Level.W, _("OOOT_Size"));
				return false;

			default:
				Message.New(Message.Level.E, _("Wrong_Size"));
				return false;
		}
	}

	public static byte[] ToPcPortSave(string filePath)
	{
		var saveData = Convert.ToBigEndian(Io.GetDataBytes(filePath));

		saveData = FixPlayerName(saveData, 0x0044);
		saveData = FixPlayerName(saveData, 0x1494);
		saveData = FixPlayerName(saveData, 0x28E4);

		saveData = FixBytePos(saveData, 0);
		saveData = FixBytePos(saveData, 0x1470 - 0x20);
		saveData = FixBytePos(saveData, 0x28C0 - 0x20);

		saveData = CopyBackupSaves(saveData);

		return saveData;
	}

	private static byte[] FixPlayerName(byte[] saveBytes, int offset)
	{
		var nameData = Io.GetFromBytes(saveBytes, offset, 8);
		var newNameData = new byte[8];

		for (var i = 0; i < nameData.Length; i++)
		{
			var b = nameData[i];
			if (b > 0x09 && b != 0x3E) // Not a number or empty.
				if (b >= 0xAB && b <= 0xDF) // Has USA charset.
					b -= 0xA1;
			newNameData[i] += b;
		}

		return Io.SetFromBytes(saveBytes, offset, newNameData);
	}

	private static ushort GetChecksum(byte[] saveBytes, int offset)
	{
		ushort checksum = 0;
		for (var i = 0; i < 0x9A9; i++)
			checksum += BitConverter.ToUInt16(saveBytes, offset + 0x20 + i * 2);

		return checksum;
	}

	private static byte[] FixBytePos(byte[] saveBytes, int offset)
	{
		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.LittleEndian, saveBytes, offset + 0x20);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.LittleEndian, saveBytes, offset + 0x24);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.LittleEndian, saveBytes, offset + 0x28);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x2C);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x4C);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x50);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x52);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x84);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x90);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0xBC);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.LittleEndian, saveBytes, offset + 0xC0, 0xEA0);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.LittleEndian, saveBytes, offset + 0xEC);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xEF4);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xEF8);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xEFC);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xF00, 0x10);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xF10, 0x10);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xF20, 0x10);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xF30, 0x10);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xF40, 0x10);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.WordSwapped, saveBytes, offset + 0xF50);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x1368);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x136C);

		saveBytes = Convert.SelectedBytesTo
			(ByteOrder.Format.ByteSwapped, saveBytes, offset + 0x1370);

		saveBytes = Io.SetFromBytes
		(saveBytes, 0x1372 + offset, // And checksum bytes
			BitConverter.GetBytes(GetChecksum(saveBytes, offset)));

		return saveBytes;
	}

	/*   This function will overwrite save data backups!   */
	/* Be aware! Emulator saves shouldn't have this issue! */
	private static byte[] CopyBackupSaves(byte[] saveBytes)
	{
		var save1 = Io.GetFromBytes(saveBytes, 0x0020, 0x1450);
		var save2 = Io.GetFromBytes(saveBytes, 0x1470, 0x1450);
		var save3 = Io.GetFromBytes(saveBytes, 0x28C0, 0x1450);

		saveBytes = Io.SetFromBytes(saveBytes, 0x3D10, save1);
		saveBytes = Io.SetFromBytes(saveBytes, 0x5160, save2);
		saveBytes = Io.SetFromBytes(saveBytes, 0x65B0, save3);

		return saveBytes;
	}
}