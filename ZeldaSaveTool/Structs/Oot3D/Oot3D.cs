using System.Runtime.InteropServices;

namespace ZeldaSaveTool.Structs.Oot3D;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ItemEquips {
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] public byte[] buttonItems;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] buttonSlots;
	public byte padding_09;
	public ushort equipment;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SavePlayerData {
	public int entranceIndex;
	public int linkAge;
	public int cutsceneIndex;
	public ushort dayTime;
	public byte masterQuestFlag;
	public byte padding_0F;
	public int nightFlag;
	public int unk_14;
	public int unk_18;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] playerName; // UTF-16, 8 chars
	public byte playerNameLength;
	public byte zTargetingSetting;
	public short unk_2E;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public byte[] newf;
	public ushort deaths; // saveCount in z3Dsave
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public byte[] unk_38;
	public ushort healthCapacity;
	public short health;
	public sbyte magicLevel;
	public sbyte magic;
	public short rupees;
	public ushort bgsHitsLeft;
	public ushort naviTimer;
	public byte isMagicAcquired;
	public byte unk_4F;
	public byte isDoubleMagicAcquired;
	public byte isDoubleDefenseAcquired;
	public byte bgsFlag;
	public byte padding_53;
	public ItemEquips childEquips;
	public ItemEquips adultEquips;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x12)] public byte[] unk_6C;
	public ushort savedSceneId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Inventory {
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)] public byte[] items;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public sbyte[] ammo;
	public ushort equipment;
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
	public uint rooms1;
	public uint rooms2;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FaroresWindData {
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public int[] pos;
	public int yaw;
	public int playerParams;
	public int entranceIndex;
	public int roomIndex;
	public int set;
	public int tempSwchFlags;
	public int tempCollectFlags;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SaveInfo {
	public SavePlayerData playerData;
	public ItemEquips equips;
	public Inventory inventory;
	public short padding_EA;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 124)] public SavedSceneFlags[] sceneFlags;
	public FaroresWindData fw;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] unk_EA4;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)] public byte[] gsFlags;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public byte[] unk_ECA;
	public uint horsebackArcheryHighscore;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] unk_ED4;
	public uint horseRaceRecordTime;
	public uint marathonRaceRecordTime;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] unk_EE4;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)] public ushort[] eventChkInf;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public ushort[] itemGetInf;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)] public ushort[] infTable;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] unk_F4C;
	public uint worldMapAreaData;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Save {
	public SaveInfo info;
}