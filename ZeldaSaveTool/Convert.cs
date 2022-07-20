/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal class Convert
{
	public static byte[] ToBigEndian(byte[] bytes)
	{
		var format = ByteOrder.IdentifyFormat
				(Io.GetFromBytes(bytes, 60, 4));

		if (format == ByteOrder.Format.Unknown)
			throw new IOException(_("Invalid"));

		switch (format)
		{
			case ByteOrder.Format.BigEndian:
				// We don't convert anything.
				// But return the bytes anyway!
				return bytes;

			case ByteOrder.Format.LittleEndian:
			case ByteOrder.Format.ByteSwapped:
			case ByteOrder.Format.WordSwapped:
				return ByteOrder.ToBigEndian(bytes, format);
		}

		return null!;
	}

	public static byte[] SelectedBytesTo
		(ByteOrder.Format format, byte[] input, int offset, int count = 4)
	{
		return Io.SetFromBytes(input, offset,
			ByteOrder.ToBigEndian
				(Io.GetFromBytes(input, offset, count), format));
	}
}