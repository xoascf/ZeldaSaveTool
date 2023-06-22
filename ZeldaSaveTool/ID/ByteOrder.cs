namespace ZeldaSaveTool.ID;

public static class ByteOrder {
	public enum Type {
		Unknown = -1,

		BigEndian,    /* Z64 */
		LittleEndian, /* N64 */
		ByteSwapped,  /* V64 */
		WordSwapped,  /* U64 */
	}

	private static void MoveBytes(byte[] data, int[] order) {
		if (data.Length % 4 != 0)
			throw new ArgumentException("Invalid input data length.", nameof(data));

		for (int i = 0; i < data.Length / 4; i++) {
			byte t0 = data[i * 4 + order[0]];
			byte t1 = data[i * 4 + order[1]];
			byte t2 = data[i * 4 + order[2]];
			byte t3 = data[i * 4 + order[3]];

			data[i * 4 + 0] = t0;
			data[i * 4 + 1] = t1;
			data[i * 4 + 2] = t2;
			data[i * 4 + 3] = t3;
		}
	}

	public static byte[] ToBigEndian(this byte[] data, Type from) {
		switch (from) {
			case Type.LittleEndian:
				MoveBytes(data, new[] { 3, 2, 1, 0 });
				break;

			case Type.ByteSwapped:
				MoveBytes(data, new[] { 1, 0, 3, 2 });
				break;

			case Type.WordSwapped:
				MoveBytes(data, new[] { 2, 3, 0, 1 });
				break;
		}

		return data;
	}
}