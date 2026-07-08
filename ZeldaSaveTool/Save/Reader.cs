using System.Reflection;
using System.Runtime.InteropServices;

namespace ZeldaSaveTool.Save;

internal class Reader {
	public static T ByteToType<T>(byte[] bytes, bool isBigEndian) where T : struct {
		Type type = typeof(T);

		if (isBigEndian)
			SwapData(bytes, type);

		GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		T str = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
		handle.Free();

		return str;
	}

	public static byte[] TypeToByte<T>(T str, bool isBigEndian) where T : struct {
		int size = Marshal.SizeOf(str);
		byte[] bytes = new byte[size];
		IntPtr ptr = Marshal.AllocHGlobal(size);
		Marshal.StructureToPtr(str, ptr, true);
		Marshal.Copy(ptr, bytes, 0, size);
		Marshal.FreeHGlobal(ptr);

		if (isBigEndian)
			SwapData(bytes, typeof(T));

		return bytes;
	}

	private static void SwapData(byte[] bytes, Type type, int startOffset = 0) {
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		foreach (FieldInfo field in fields) {
			int offset = startOffset + Marshal.OffsetOf(type, field.Name).ToInt32();

			Type fieldType = field.FieldType;

			if (fieldType.IsArray) {
				Type elementType = fieldType.GetElementType();
				var marshalAs = (MarshalAsAttribute)Attribute.GetCustomAttribute(field, typeof(MarshalAsAttribute));
				int numElements = marshalAs != null ? marshalAs.SizeConst : 0;
				if (numElements == 0) continue;

				int elementSize = Marshal.SizeOf(elementType);

				if (elementType == typeof(byte) || elementType == typeof(sbyte)) {
					// Do nothing
				}
				else if (elementType == typeof(short) || elementType == typeof(ushort)) {
					for (int i = 0; i < numElements; i++)
						Array.Reverse(bytes, offset + i * 2, 2);
				}
				else if (elementType == typeof(int) || elementType == typeof(uint) || elementType == typeof(float)) {
					for (int i = 0; i < numElements; i++)
						Array.Reverse(bytes, offset + i * 4, 4);
				}
				else if (elementType == typeof(long) || elementType == typeof(ulong) || elementType == typeof(double)) {
					for (int i = 0; i < numElements; i++)
						Array.Reverse(bytes, offset + i * 8, 8);
				}
				else if (elementType.IsValueType) {
					// Array of structs
					for (int i = 0; i < numElements; i++)
						SwapData(bytes, elementType, offset + i * elementSize);
				}
			}
			else {
				if (fieldType == typeof(byte) || fieldType == typeof(sbyte) || fieldType == typeof(string)) {
					// Do nothing
				}
				else if (fieldType == typeof(short) || fieldType == typeof(ushort)) {
					Array.Reverse(bytes, offset, 2);
				}
				else if (fieldType == typeof(int) || fieldType == typeof(uint) || fieldType == typeof(float)) {
					Array.Reverse(bytes, offset, 4);
				}
				else if (fieldType == typeof(long) || fieldType == typeof(ulong) || fieldType == typeof(double)) {
					Array.Reverse(bytes, offset, 8);
				}
				else if (fieldType.IsValueType) {
					// Nested struct
					SwapData(bytes, fieldType, offset);
				}
				else {
					throw new Exception("Unsupported field type " + fieldType.ToString());
				}
			}
		}
	}
}