using System.Runtime.InteropServices;

namespace ZeldaSaveTool.Structs.Oot;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ItemEquips {
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] buttonItems;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public byte[] cButtonSlots;
	public byte padding_07;
	public ushort equipment;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SavePlayerData {
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public byte[] newf;
	public ushort deaths;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] playerName;
	public short n64ddFlag;
	public short healthCapacity;
	public short health;
	public sbyte magicLevel;
	public sbyte magic;
	public short rupees;
	public ushort swordHealth;
	public ushort naviTimer;
	public byte isMagicAcquired;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] public byte[] unk_3B;
	public byte isDoubleMagicAcquired;
	public byte isDoubleDefenseAcquired;
	public byte bgsFlag;
	public byte ocarinaGameRoundNum;
	public ItemEquips childEquips;
	public ItemEquips adultEquips;
	public uint unk_54;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0E)] public byte[] unk_58;
	public short savedSceneId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Inventory {
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)] public byte[] items;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public sbyte[] ammo;
	public ushort equipment;
	public short padding_2A; // 2 bytes
	public uint upgrades;
	public uint questItems;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] dungeonItems;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)] public sbyte[] dungeonKeys;
	public sbyte defenseHearts;
	public short gsTokens;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SavedSceneFlags {
	public uint chest;
	public uint swch;
	public uint clear;
	public uint collect;
	public uint unk;
	public uint rooms;
	public uint floors;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vec3i {
	public int x;
	public int y;
	public int z;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FaroresWindData {
	public Vec3i pos;
	public int yaw;
	public int playerParams;
	public int entranceIndex;
	public int roomIndex;
	public int set;
	public int tempSwchFlags;
	public int tempCollectFlags;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vec3s {
	public short x;
	public short y;
	public short z;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HorseData {
	public short sceneId;
	public Vec3s pos;
	public short angle;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Checksum {
	public ushort value;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SaveInfo {
	public SavePlayerData playerData;
	public ItemEquips equips;
	public short padding_56; // 2 bytes padding at 0x56
	public Inventory inventory;
	public short padding_B6; // 2 bytes padding at 0xB6

	// SavedSceneFlags is 7 uints. 124 * 7 = 868 uints.
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 868)] public uint[] sceneFlags;

	public FaroresWindData fw;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)] public byte[] unk_E8C;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public int[] gsFlags;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] unk_EB4;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public int[] highScores;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)] public ushort[] eventChkInf;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public ushort[] itemGetInf;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)] public ushort[] infTable;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] unk_F34;
	public uint worldMapAreaData;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] unk_F3C;
	public byte scarecrowLongSongSet;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x360)] public byte[] scarecrowLongSong;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24)] public byte[] unk_12A1;
	public byte scarecrowSpawnSongSet;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)] public byte[] scarecrowSpawnSong;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public byte[] unk_1346;
	public HorseData horseData;
	public Checksum checksum;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Save {
	public int entranceIndex;
	public int linkAge;
	public int cutsceneIndex;
	public ushort dayTime;
	public short padding_0E; // 2 bytes padding
	public int nightFlag;
	public int totalDays;
	public int bgsDayCount;
	public SaveInfo info;
}