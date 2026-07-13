/* Licensed under the Open Software License version 3.0 */
// From OpenOcarinaBuilder.

using System.IO;
using System.IO.Compression;

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

	public static byte[] DecompressGzip(byte[] compressed) {
		using MemoryStream input = new(compressed);
		using GZipStream gz = new(input, CompressionMode.Decompress);
		using MemoryStream output = new();
		byte[] buffer = new byte[81920];
		int n;
		while ((n = gz.Read(buffer, 0, buffer.Length)) > 0)
			output.Write(buffer, 0, n);
		return output.ToArray();
	}

	public static byte[] DecompressZip(byte[] compressed) {
		using MemoryStream input = new(compressed);
#if NET20
		BinaryReader reader = new(input);
		input.Seek(26, SeekOrigin.Begin);
		ushort fileNameLength = reader.ReadUInt16();
		ushort extraFieldLength = reader.ReadUInt16();
		int dataOffset = 30 + fileNameLength + extraFieldLength;
		input.Seek(dataOffset, SeekOrigin.Begin);

		using MemoryStream output = new();
		using DeflateStream deflate = new(input, CompressionMode.Decompress);
		byte[] buffer = new byte[81920];
		int n;
		while ((n = deflate.Read(buffer, 0, buffer.Length)) > 0)
			output.Write(buffer, 0, n);
		return output.ToArray();
#else
		using ZipArchive archive = new(input, ZipArchiveMode.Read);
		if (archive.Entries.Count > 0) {
			using Stream unzippedEntryStream = archive.Entries[0].Open();
			using MemoryStream output = new();
			unzippedEntryStream.CopyTo(output);
			return output.ToArray();
		}
		return Array.Empty<byte>();
#endif
	}

	public static void SaveToFile(byte[] bytes, string output) {
		using FileStream fs = File.Open(output, FileMode.Create);
		fs.Write(bytes, 0, bytes.Length);
	}
}