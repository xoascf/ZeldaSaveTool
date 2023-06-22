/* Licensed under the Open Software License version 3.0 */

using ZeldaSaveTool.Utility;

namespace ZeldaSaveTool.Save;

internal static class Convert {
	private static readonly byte[] SaveMagic = { 0x5A, 0x45, 0x4C, 0x44 }; /* ZELD */

	private static ByteOrder Identify(byte[] input, byte[] magic) {
		if (magic.Length != input.Length)
			return ByteOrder.Unknown;

		if (magic.Matches(input))
			return ByteOrder.BigEndian;

		foreach (ByteOrder f in Enum.GetValues(typeof(ByteOrder))) {
			if (f is ByteOrder.BigEndian or ByteOrder.Unknown)
				continue;

			if (magic.Matches(input.CopyAs(f)))
				return f;
		}

		return ByteOrder.Unknown;
	}

	public static byte[] ToBigEndian(this byte[] bytes) {
		ByteOrder order = Identify(bytes.Get(60, 4), SaveMagic);

		return order switch {
			ByteOrder.Unknown => throw new(T("Invalid")),
			ByteOrder.BigEndian => bytes, /* Just return the bytes! */
			ByteOrder.LittleEndian or ByteOrder.ByteSwapped or
			ByteOrder.WordSwapped => bytes.ToBigEndian(from: order),
			_ => Zero
		};
	}
}