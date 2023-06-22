/* Licensed under the Open Software License version 3.0 */
// From OpenOcarinaBuilder.

using System.IO;

namespace ZeldaSaveTool.Utility;

internal static class IO {
	public static byte[] Get(this byte[] input, int start, int length) {
		byte[] bytes = new byte[length];

		using MemoryStream s = new(input); s.Seek(start, SeekOrigin.Begin);
		_ = s.Read(bytes, 0, length);

		return bytes;
	}

	public static void Set(this byte[] array, int offset, object newData) {
		using MemoryStream s = new(array);
		s.Seek(offset, SeekOrigin.Begin);

		switch (newData) {
			case byte[] bytes:
				for (int i = 0; i < bytes.Length; i++)
					s.WriteByte(bytes[i]);
				break;

			case byte data:
				s.WriteByte(data);
				break;

			default:
				s.WriteByte(Convert.ToByte(newData));
				break;
		}
	}

	public static byte[] GetFileBytes(string path) => File.ReadAllBytes(path);
	public static long GetFileLength(string path) => new FileInfo(path).Length;
	public static string BaseName(string path) => Path.GetFileName(path);
	public static bool Exists(string path) => File.Exists(path);

	public static void SaveToFile(byte[] bytes, string output) {
		using FileStream fs = File.Open(output, FileMode.Create);
		fs.Write(bytes, 0, bytes.Length);
	}
}