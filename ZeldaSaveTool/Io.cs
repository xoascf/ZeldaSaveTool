/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal class Io
{
	public static byte[] GetFromBytes(byte[] input, int offset, int count)
	{
		var bytes = new byte[count];

		using MemoryStream s = new(input);
		s.Seek(offset, SeekOrigin.Begin);
		var unused = s.Read(bytes, 0, count);

		return bytes;
	}

	public static byte[] SetFromBytes(byte[] input, int offset, byte[] newBytes)
	{
		using MemoryStream s = new(input);
		s.Seek(offset, SeekOrigin.Begin);

		for (var i = 0; i < newBytes.Length; i++)
			s.WriteByte(newBytes[i]);

		return s.ToArray();
	}

	public static byte[] GetDataBytes(string path)
	{
		return File.ReadAllBytes(path);
	}

	public static bool ValidateFile(string path)
	{
		return File.Exists(path);
	}

	public static void SaveToFile(byte[] bytes, string output)
	{
		using var fs = File.Open(output, FileMode.Create);
		fs.Write(bytes, 0, bytes.Length);
	}
}