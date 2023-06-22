namespace ZeldaSaveTool.Save;

public struct Slot {
	public short DeathCount { get; set; }
	public string? Name { get; set; }
	public short HeartsTotal { get; set; }
	public short HeartsCount { get; set; }
	public bool DoubleDefense { get; set; }
	//public bool IsMQ { get; set; } // For JSON .sav SoH only?
}