using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace PhigrosLibraryCSharp;
internal static class UtilityExtension
{
	[return: NotNull]
	internal static T EnsureNotNull<T>(this T obj, string? additionalExceptionInfo = null, [CallerArgumentExpression(nameof(obj))] string? paramName = null)
	{
		if (obj == null)
			throw new DebugArgumentNullException(paramName, additionalExceptionInfo);
		return obj;
	}
	internal static T[] QuickCopy<T>(T[] array)
	{
		T[] values = new T[array.Length];
		array.CopyTo(values, 0);
		return values;
	}
	internal static string ToHex(this byte[] bytes)
	{
		StringBuilder sb = new();
		for (int i = 0; i < bytes.Length; i++)
		{
			sb.Append(bytes[i].ToString("x2"));
		}
		return sb.ToString();
	}

	internal static bool HasBit(this int value, int index)
	{
		if (index < 0 || index > 31)
			throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 31.");
		return (value & (1 << index)) != 0;
	}
	internal static bool HasBit(this byte value, int index)
	{
		if (index < 0 || index > 7)
			throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 7.");
		return (value & (1u << index)) != 0;
	}

	extension(SocketsHttpHandler self)
	{
		internal static SocketsHttpHandler CreateFromLifeTime(TimeSpan pooledConnectionLifetime)
		{
			return new SocketsHttpHandler()
			{
				PooledConnectionLifetime = pooledConnectionLifetime,
			};
		}
	}

	extension(BinaryWriter self)
	{
		internal void WritePackedBools(params ReadOnlySpan<bool> values)
		{
			if (values.Length > 8)
				throw new ArgumentOutOfRangeException(nameof(values), "Cannot write more than 8 bools in a single byte.");

			byte value = 0;
			for (int i = 0; i < values.Length; i++)
			{
				value |= (byte)((values[i] ? 1 : 0) << i);
			}

			self.Write(value);
		}
		internal unsafe void WriteEnum<TEnum>(TEnum value) where TEnum : unmanaged, Enum
		{
			Span<byte> buffer = stackalloc byte[sizeof(TEnum)];
			MemoryMarshal.Write(buffer, value);
			self.Write(buffer);
		}
	}

	extension(BinaryReader self)
	{
		internal bool HasMore => self.BaseStream.Position < self.BaseStream.Length;

		internal static BinaryReader FromArray(byte[] data, out MemoryStream createdStream, bool leaveOpen = false)
		{
			createdStream = new(data);
			return new(createdStream, Encoding.UTF8, leaveOpen);
		}

		internal bool ReadFromPackedBoolNoJump(int index)
		{
			// prob not the best way?
			bool value = self.ReadFromPackedBoolThenJump(index);
			self.BaseStream.Seek(-1, SeekOrigin.Current);
			return value;
		}
		internal bool ReadFromPackedBoolThenJump(int index)
		{
			if (index < 0 || index > 7)
				throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 7.");
			byte value = self.ReadByte();
			return (value & (1 << index)) != 0;
		}
		internal unsafe TEnum ReadEnum<TEnum>() where TEnum : unmanaged, Enum
		{
			Span<byte> buffer = stackalloc byte[sizeof(TEnum)];
			self.Read(buffer);
			return MemoryMarshal.Read<TEnum>(buffer);
		}
	}

	extension(IJsonTypeInfoResolver self)
	{
		internal IJsonTypeInfoResolver CombineWith(params IJsonTypeInfoResolver?[] others)
		{
			return JsonTypeInfoResolver.Combine([self, .. others]);
		}
	}
}
