/* Licensed under the Open Software License version 3.0 */

using static System.BitConverter;

namespace ZeldaSaveTool.Utility;

internal static class ByteArray {
	private static bool ShouldRev(bool big)
		=> (IsLittleEndian && big) || (!IsLittleEndian && !big);

	public static byte[] Reverse(this byte[] data) { Array.Reverse(data); return data; }

	public static byte[] FromI16(short value, bool big = true) // 2 bytes
		=> ShouldRev(big) ? Reverse(GetBytes(value)) : GetBytes(value);
	
	public static byte[] FromU16(ushort value, bool big = true) // 2 bytes
		=> ShouldRev(big) ? Reverse(GetBytes(value)) : GetBytes(value);

	public static byte[] FromI32(int value, bool big = true) // 4 bytes
		=> ShouldRev(big) ? Reverse(GetBytes(value)) : GetBytes(value);

	public static byte[] FromU32(uint value, bool big = true) // 4 bytes
		=> ShouldRev(big) ? Reverse(GetBytes(value)) : GetBytes(value);

	public static ushort ToU16(this byte[] data, int start = 0, bool big = true)
		=> ShouldRev(big) ? ToUInt16(Reverse(data), start) : ToUInt16(data, start);

	public static short ToI16(this byte[] data, bool big = true)
		=> ShouldRev(big) ? ToInt16(Reverse(data), 0) : ToInt16(data, 0);

	public static int ToI32(this byte[] data, bool big = true)
		=> ShouldRev(big) ? ToInt32(Reverse(data), 0) : ToInt32(data, 0);

	public static uint ToU32(this byte[] data, bool big = true)
		=> ShouldRev(big) ? ToUInt32(Reverse(data), 0) : ToUInt32(data, 0);

	public static bool Matches(this byte[] a, byte[] b) {
		if (a.Length != b.Length)
			return false;

		for (int i = 0; i < a.Length; i++)
			if (a[i] != b[i])
				return false;

		return true;
	}

	public static byte[] DataTo(this byte[] b, ByteOrder f, int s = 0, int l = 4) {
		b.Set(s, b.Get(s, l).ToBigEndian(f));

		return b;
	}

	public static byte[] CopyAs(this byte[] a, ByteOrder f, int s = 0, int l = 4) {
		byte[] b = new byte[l];
		Buffer.BlockCopy(a, 0, b, 0, l);

		return b.DataTo(f, s, l);
	}

	public static void SetSwap(this byte[] input, int from, int length = 4) {
		input.Set(from, Reverse(input.Get(from, length)));
	}
}