using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

using __m128i = System.Runtime.Intrinsics.Vector128<int>;
using _DWORD = uint;

namespace PhigrosLibraryCSharp.AccountCreator;
public static unsafe class NativeHelper
{
	private static readonly byte[] ToSet = new byte[]
	{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
	private static readonly int _stack_chk_guard = 0x11451419;

	public static unsafe void Test()
	{
		const int length = 104;

		byte[] input = Encoding.ASCII.GetBytes("X-UA=V=1&PN=TapTap&VN_CODE=206012000&LOC=TW&LANG=en_US&CH=default&UID=cd3da46d-5147-4946-aaf2-20922592994a&action=callback&android_id=60e6dc18c7bdc2da&cpu=armeabi-v7a&model=SM-G998B&name=samsung&nonce=mtps7&pn=TapTap&push_id=bcbef0fda23b4187a83ef8015fb9e182&screen=1024x768&supplier=1&time=1725791668&uuid=cd3da46d-5147-4946-aaf2-20922592994a&version=9");
		byte[] key = Encoding.ASCII.GetBytes("PeCkE6Fu0B10Vm9BKfPfANwCUAn5POcs");

		byte* buffer = stackalloc byte[length];
		FillBuffer01234567(buffer);

		HMACMD5 md5 = new(key);
		Console.WriteLine(md5.ComputeHash(md5.ComputeHash(input)).ToHex());

		fixed (byte* inputPtr = input)
		{
			fixed (byte* keyPtr = key)
			{
				ProcessBuffer(buffer, inputPtr, 0x160);
				//string first = Encoding.ASCII.GetString(new Span<byte>(buffer, length));
				Console.WriteLine(new Span<byte>(buffer, length).ToArray().ToHex());
				ProcessBuffer(buffer, keyPtr, 0x20);
				//string second = Encoding.ASCII.GetString(new Span<byte>(buffer, length));
				Console.WriteLine(new Span<byte>(buffer, length).ToArray().ToHex());
			}
		}

	}
	// doesnt work :(
	internal static unsafe void FillBuffer01234567(byte* buffer)
	{
		for (int i = 0; i < ToSet.Length; i++)
			buffer[i] = ToSet[i];
	}

	internal static unsafe void ProcessBuffer(byte* buffer, byte* bytesDataStartPtr, uint bytesDataLength)
	{
		__m128i xmmword_40F0 = __m128i.Zero
			.WithElement(0, unchecked((int)0xFF00FF00))
			.WithElement(1, unchecked((int)0xFF00FF00))
			.WithElement(2, unchecked((int)0xFF00FF00))
			.WithElement(3, unchecked((int)0xFF00FF00));

		byte* v3; // esi
		uint v4; // edx
		uint v5; // eax
		uint v6; // ecx
		__m128i v7; // xmm6
		__m128i v8; // xmm1
		__m128i v9; // xmm7
		__m128i v10; // xmm3
		__m128i v11; // xmm4
		__m128i v12; // xmm6
		__m128i v13; // xmm1
		__m128i v14; // xmm0
		__m128i v15; // xmm7
		__m128i v16; // xmm5
		__m128i v17; // xmm3
		__m128i v18; // xmm1
		__m128i v19; // xmm0
		__m128i v20; // xmm3
		__m128i v21; // xmm4
		__m128i v22; // xmm5
		__m128i v23; // xmm3
		__m128i v24; // xmm1
		__m128i v25; // xmm4
		__m128i v26; // xmm2
		__m128i v27; // xmm7
		__m128i v28; // xmm6
		__m128i v29; // [esp-40h] [ebp-9Ch] BYREF
		__m128i v30; // [esp-30h] [ebp-8Ch] BYREF
		__m128i v31; // [esp-20h] [ebp-7Ch] BYREF
		byte* bytesDataEndPtr; // [esp-4h] [ebp-60h]
		__m128i* a2 = stackalloc __m128i[4]; // [esp+0h] [ebp-5Ch] BYREF
		int v34; // [esp+4Ch] [ebp-10h]

		v3 = bytesDataStartPtr;
		v4 = *(_DWORD*)buffer + (8 * bytesDataLength);
		v5 = (*(_DWORD*)buffer >> 3) & 0x3F;
		v6 = *((_DWORD*)buffer + 1);
		if (*(_DWORD*)buffer > v4)
			++v6;
		*(_DWORD*)buffer = v4;

		v34 = _stack_chk_guard;

		*((_DWORD*)buffer + 1) = v6 + (bytesDataLength >> 0x1D);
		bytesDataEndPtr = &bytesDataStartPtr[bytesDataLength];
		if (bytesDataStartPtr != &bytesDataStartPtr[bytesDataLength])
		{
			do
			{
				buffer[v5 + 0x18] = *v3++;
				if (v5 == 0x3F)
				{
					v7 = _mm_loadu_si128((__m128i*)(buffer + 0x18));
					v8 = _mm_loadu_si128((__m128i*)(buffer + 0x28));
					v9 = _mm_loadu_si128((__m128i*)(buffer + 0x48));
					v10 = _mm_loadu_si128((__m128i*)(buffer + 0x38));
					v11 = _mm_packus_epi16(
						_mm_and_si128(_mm_load_si128(&xmmword_40F0), v7),
					_mm_and_si128(_mm_load_si128(&xmmword_40F0), v8));
					v12 = _mm_packus_epi16(_mm_srli_epi16(v7, 8), _mm_srli_epi16(v8, 8));
					v13 = _mm_packus_epi16(
						_mm_and_si128(_mm_load_si128(&xmmword_40F0), v10),
					_mm_and_si128(_mm_load_si128(&xmmword_40F0), v9));
					v14 = v9;
					v15 = _mm_packus_epi16(
						_mm_and_si128(_mm_load_si128(&xmmword_40F0), v11),
					_mm_and_si128(_mm_load_si128(&xmmword_40F0), v13));
					v16 = _mm_srli_epi16(v13, 8);
					v17 = _mm_packus_epi16(_mm_srli_epi16(v10, 8), _mm_srli_epi16(v14, 8));
					v18 = _mm_packus_epi16(
						_mm_and_si128(_mm_load_si128(&xmmword_40F0), v12),
					_mm_and_si128(_mm_load_si128(&xmmword_40F0), v17));
					v19 = _mm_packus_epi16(_mm_srli_epi16(v12, 8), _mm_srli_epi16(v17, 8));
					v20 = _mm_packus_epi16(_mm_srli_epi16(v11, 8), v16);
					v21 = _mm_unpackhi_epi8(v20, __m128i.Zero);
					v31 = _mm_unpacklo_epi8(v18, __m128i.Zero);
					v22 = _mm_unpacklo_epi8(v19, __m128i.Zero);
					v23 = _mm_unpacklo_epi8(v20, __m128i.Zero);
					v29 = v21;
					v24 = _mm_unpackhi_epi8(v18, __m128i.Zero);
					v30 = _mm_unpackhi_epi8(v19, __m128i.Zero);
					v25 = _mm_unpacklo_epi8(v15, __m128i.Zero);
					v26 = _mm_unpackhi_epi8(v15, __m128i.Zero);
					a2[0] = _mm_or_si128(
						_mm_slli_epi32(_mm_unpacklo_epi16(_mm_load_si128(&v31), __m128i.Zero), 8),
						_mm_or_si128(
							_mm_or_si128(
								_mm_slli_epi32(_mm_unpacklo_epi16(v22, __m128i.Zero), 0x18),
								_mm_slli_epi32(_mm_unpacklo_epi16(v23, __m128i.Zero), 0x10)),
							_mm_unpacklo_epi16(v25, __m128i.Zero)));
					v27 = _mm_load_si128(&v30);
					v28 = _mm_load_si128(&v29);
					a2[1] = _mm_or_si128(
						_mm_or_si128(
							_mm_unpackhi_epi16(v25, __m128i.Zero),
							_mm_or_si128(
								_mm_slli_epi32(_mm_unpackhi_epi16(v23, __m128i.Zero), 0x10),
								_mm_slli_epi32(_mm_unpackhi_epi16(v22, __m128i.Zero), 0x18))),
							_mm_slli_epi32(_mm_unpackhi_epi16(_mm_load_si128(&v31), __m128i.Zero), 8));
					a2[2] = _mm_or_si128(
						_mm_or_si128(
							_mm_or_si128(
								_mm_slli_epi32(_mm_unpacklo_epi16(v28, __m128i.Zero), 0x10),
								_mm_slli_epi32(_mm_unpacklo_epi16(v27, __m128i.Zero), 0x18)),
							_mm_unpacklo_epi16(v26, __m128i.Zero)),
						_mm_slli_epi32(_mm_unpacklo_epi16(v24, __m128i.Zero), 8));
					a2[3] = _mm_or_si128(
						_mm_or_si128(
							_mm_unpackhi_epi16(v26, __m128i.Zero),
							_mm_or_si128(
								_mm_slli_epi32(_mm_unpackhi_epi16(v28, __m128i.Zero), 0x10),
								_mm_slli_epi32(_mm_unpackhi_epi16(v27, __m128i.Zero), 0x18))),
							_mm_slli_epi32(_mm_unpackhi_epi16(v24, __m128i.Zero), 8));
					ProcessChunk((uint*)buffer + 2, (uint*)a2);
					v5 = 0;
				}
				else
				{
					++v5;
				}
			}
			while (v3 != bytesDataEndPtr);
		}
		if (v34 != _stack_chk_guard)
			Environment.FailFast(null);
	}
	internal static __m128i _mm_loadu_si128(__m128i* ptr)
	{
		return Sse2.LoadVector128((int*)ptr);
	}
	internal static __m128i _mm_load_si128(__m128i* ptr)
	{
		return Sse2.LoadAlignedVector128((int*)ptr);
	}
	internal static __m128i _mm_or_si128(__m128i a, __m128i b)
	{
		return Sse2.Or(a, b);
	}
	internal static __m128i _mm_and_si128(__m128i a, __m128i b)
	{
		return Sse2.And(a, b);
	}
	internal static __m128i _mm_packus_epi16(__m128i a, __m128i b)
	{
		return Sse2.PackUnsignedSaturate(a.AsInt16(), b.AsInt16()).AsInt32();
	}
	internal static __m128i _mm_unpacklo_epi16(__m128i a, __m128i b)
	{
		return Sse2.UnpackLow(a.AsInt16(), b.AsInt16()).AsInt32();
	}
	internal static __m128i _mm_unpackhi_epi16(__m128i a, __m128i b)
	{
		return Sse2.UnpackHigh(a.AsInt16(), b.AsInt16()).AsInt32();
	}
	internal static __m128i _mm_unpacklo_epi8(__m128i a, __m128i b)
	{
		return Sse2.UnpackLow(a.AsByte(), b.AsByte()).AsInt32();
	}
	internal static __m128i _mm_unpackhi_epi8(__m128i a, __m128i b)
	{
		return Sse2.UnpackHigh(a.AsByte(), b.AsByte()).AsInt32();
	}
	internal static __m128i _mm_slli_epi32(__m128i a, [ConstantExpected] byte immediate)
	{
		return Sse2.ShiftLeftLogical(a, immediate);
	}
	internal static __m128i _mm_srli_epi16(__m128i a, [ConstantExpected] byte immediate)
	{
		return Sse2.ShiftRightLogical(a.AsUInt16(), immediate).AsInt32();
	}
	/*internal static unsafe void ProcessBuffer(byte* buffer, byte* bytesDataStartPtr, uint bytesDataLength)
	{
		const int _stack_chk_guard = 1145141919;

		uint v5; // r1
		byte* arrayEndPtr; // r6 // sbyte?
		uint v7; // r0
		int v8; // r3
		int v9; // r3
		int v10; // r2
		byte v11; // t1
		byte* v12; // r3
		int i; // r0
		int v14; // r2
		int* v15 = stackalloc int[23]; // [sp+4h] [bp-5Ch] BYREF

		v5 = *(uint*)buffer;
		arrayEndPtr = &bytesDataStartPtr[bytesDataLength];
		v7 = *(uint*)buffer + (8 * bytesDataLength); // add uint cast
		v8 = _stack_chk_guard;
		*(uint*)buffer = v7;
		v15[0x10] = v8;
		v9 = (int)((v5 >> 3) & 0x3F); // add int cast
		if (v7 < v5)
			++*((uint*)buffer + 1);
		*((uint*)buffer + 1) += bytesDataLength >> 0x1D;
		while (bytesDataStartPtr != arrayEndPtr)
		{
			v10 = v9 + 1;
			v11 = *bytesDataStartPtr++;
			buffer[v9 + 0x18] = v11;
			if (v9 == 0x3F)
			{
				v12 = buffer;
				for (i = 0; i != 0x10; ++i)
				{
					v14 = v12[0x1A];
					v12 += 4;
					v15[i] = v12[0x14] | (v14 << 0x10) | (v12[0x17] << 0x18) | (v12[0x15] << 8);
				}
				sub_1DEC((int*)buffer + 2, v15); // uint to int
				v10 = 0;
			}
			v9 = v10;
		}
	}*/
	internal static unsafe void ProcessChunk(uint* result, uint* a2)
	{
		_DWORD v2; // eax
		_DWORD v4; // edx
		_DWORD v5; // edi
		_DWORD v6; // esi
		_DWORD v7; // ecx
		_DWORD v8; // eax
		_DWORD v9; // edi
		_DWORD v10; // esi
		_DWORD v11; // ecx
		_DWORD v12; // eax
		_DWORD v13; // edi
		_DWORD v14; // esi
		_DWORD v15; // ecx
		_DWORD v16; // edx
		_DWORD v17; // edi
		_DWORD v18; // esi
		_DWORD v19; // ecx
		_DWORD v20; // edx
		_DWORD v21; // edi
		_DWORD v22; // eax
		_DWORD v23; // esi
		_DWORD v24; // ecx
		_DWORD v25; // edx
		_DWORD v26; // edi
		_DWORD v27; // eax
		_DWORD v28; // ecx
		_DWORD v29; // esi
		_DWORD v30; // edx
		_DWORD v31; // eax
		_DWORD v32; // edi
		_DWORD v33; // ecx
		_DWORD v34; // esi
		_DWORD v35; // eax
		_DWORD v36; // edi
		_DWORD v37; // edx
		_DWORD v38; // ecx
		_DWORD v39; // esi
		_DWORD v40; // eax
		_DWORD v41; // edi
		_DWORD v42; // ecx
		_DWORD v43; // edx
		_DWORD v44; // esi
		_DWORD v45; // edi
		_DWORD v46; // eax
		_DWORD v47; // ecx
		_DWORD v48; // esi
		_DWORD v49; // edx
		_DWORD v50; // edi
		_DWORD v51; // ecx
		_DWORD v52; // eax
		_DWORD v53; // esi
		_DWORD v54; // edx
		_DWORD v55; // edi
		_DWORD v56; // ecx
		_DWORD v57; // esi
		_DWORD v58; // edx
		_DWORD v59; // eax
		_DWORD v60; // edi
		_DWORD v61; // ecx
		_DWORD v62; // edx
		_DWORD v63; // eax
		_DWORD v64; // esi
		_DWORD v65; // ecx
		_DWORD v66; // [esp+0h] [ebp-68h]
		_DWORD v67; // [esp+4h] [ebp-64h]
		_DWORD v68; // [esp+8h] [ebp-60h]
		_DWORD v69; // [esp+Ch] [ebp-5Ch]
		_DWORD v70; // [esp+10h] [ebp-58h]
		_DWORD v71; // [esp+14h] [ebp-54h]
		_DWORD v72; // [esp+18h] [ebp-50h]
		_DWORD v73; // [esp+1Ch] [ebp-4Ch]
		_DWORD v74; // [esp+20h] [ebp-48h]
		_DWORD v75; // [esp+24h] [ebp-44h]
		_DWORD v76; // [esp+28h] [ebp-40h]
		_DWORD v77; // [esp+2Ch] [ebp-3Ch]
		_DWORD v78; // [esp+30h] [ebp-38h]
		_DWORD v79; // [esp+34h] [ebp-34h]
		_DWORD v80; // [esp+38h] [ebp-30h]
		_DWORD v81; // [esp+3Ch] [ebp-2Ch]
		_DWORD v82; // [esp+40h] [ebp-28h]
		_DWORD v83; // [esp+44h] [ebp-24h]
		_DWORD v84; // [esp+48h] [ebp-20h]
		_DWORD v85; // [esp+4Ch] [ebp-1Ch]

		v82 = *result;
		v84 = result[3];
		v85 = *a2;
		v83 = result[2];
		v67 = result[1];
		v2 = v67 + __ROL4__(*result + *a2 - 0x28955B88 + ((v67 & v83) | (v84 & ~v67)), 7);
		v68 = a2[1];
		v69 = a2[2];
		v4 = v2 + __ROL4__(v84 + v68 - 0x173848AA + ((v2 & v67) | (v83 & ~v2)), 0xC);
		v70 = a2[3];
		v5 = v4 + __ROR4__(v83 + v69 + 0x242070DB + ((v2 & v4) | (~v4 & v67)), 0xF);
		v6 = v5 + __ROR4__(v67 + v70 - 0x3E423112 + ((v4 & v5) | (v2 & ~v5)), 0xA);
		v71 = a2[4];
		v7 = v6 + __ROL4__(v2 + v71 - 0xA83F051 + ((v5 & v6) | (v4 & ~v6)), 7);
		v72 = a2[5];
		v8 = v7 + __ROL4__(v4 + v72 + 0x4787C62A + ((v6 & v7) | (v5 & ~v7)), 0xC);
		v73 = a2[6];
		v9 = v8 + __ROR4__(v5 + v73 - 0x57CFB9ED + ((v7 & v8) | (v6 & ~v8)), 0xF);
		v74 = a2[7];
		v10 = v9 + __ROR4__(v6 + v74 - 0x2B96AFF + ((v8 & v9) | (v7 & ~v9)), 0xA);
		v75 = a2[8];
		v11 = v10 + __ROL4__(v7 + v75 + 0x698098D8 + ((v9 & v10) | (v8 & ~v10)), 7);
		v76 = a2[9];
		v12 = v11 + __ROL4__(v8 + v76 - 0x74BB0851 + ((v10 & v11) | (v9 & ~v11)), 0xC);
		v77 = a2[0xA];
		v13 = v12 + __ROR4__(v9 + v77 - 0xA44F + ((v11 & v12) | (v10 & ~v12)), 0xF);
		v78 = a2[0xB];
		v14 = v13 + __ROR4__(v10 + v78 - 0x76A32842 + ((v12 & v13) | (v11 & ~v13)), 0xA);
		v79 = a2[0xC];
		v15 = v14 + __ROL4__(v11 + v79 + 0x6B901122 + ((v13 & v14) | (v12 & ~v14)), 7);
		v80 = a2[0xD];
		v16 = v15 + __ROL4__(v12 + v80 - 0x2678E6D + ((v14 & v15) | (v13 & ~v15)), 0xC);
		v81 = a2[0xE];
		v17 = v16 + __ROR4__(((v14 & ~v16) | (v15 & v16)) + v13 + v81 - 0x5986BC72, 0xF);
		v66 = a2[0xF];
		v18 = v17 + __ROR4__(v14 + v66 + 0x49B40821 + ((v16 & v17) | (v15 & ~v17)), 0xA);
		v19 = v18 + __ROL4__(v68 + v15 - 0x9E1DA9E + ((v16 & v18) | (v17 & ~v16)), 5);
		v20 = v19 + __ROL4__(v73 + v16 - 0x3FBF4CC0 + ((v17 & v19) | (v18 & ~v17)), 9);
		v21 = v20 + __ROL4__(v78 + v17 + 0x265E5A51 + ((v19 & ~v18) | (v18 & v20)), 0xE);
		v22 = v21 + __ROR4__(v85 + v18 - 0x16493856 + ((v20 & ~v19) | (v19 & v21)), 0xC);
		v23 = v22 + __ROL4__(v72 + v19 - 0x29D0EFA3 + ((v21 & ~v20) | (v20 & v22)), 5);
		v24 = v23 + __ROL4__(v77 + v20 + 0x2441453 + ((v22 & ~v21) | (v21 & v23)), 9);
		v25 = v24 + __ROL4__(v66 + v21 - 0x275E197F + ((v23 & ~v22) | (v22 & v24)), 0xE);
		v26 = v25 + __ROR4__(v71 + v22 - 0x182C0438 + ((v24 & ~v23) | (v23 & v25)), 0xC);
		v27 = v26 + __ROL4__(v76 + v23 + 0x21E1CDE6 + ((v25 & ~v24) | (v24 & v26)), 5);
		v28 = v27 + __ROL4__(v81 + v24 - 0x3CC8F82A + ((v26 & ~v25) | (v25 & v27)), 9);
		v29 = v28 + __ROL4__(v70 + v25 - 0xB2AF279 + ((v27 & ~v26) | (v26 & v28)), 0xE);
		v30 = v29 + __ROR4__(v75 + v26 + 0x455A14ED + ((v28 & ~v27) | (v27 & v29)), 0xC);
		v31 = v30 + __ROL4__(v80 + v27 - 0x561C16FB + ((v29 & ~v28) | (v28 & v30)), 5);
		v32 = v31 + __ROL4__(v69 + v28 - 0x3105C08 + ((v30 & ~v29) | (v29 & v31)), 9);
		v33 = v32 + __ROL4__(v74 + v29 + 0x676F02D9 + ((v31 & ~v30) | (v30 & v32)), 0xE);
		v34 = v33 + __ROR4__(((v32 & ~v31) | (v31 & v33)) + v79 + v30 - 0x72D5B376, 0xC);
		v35 = v34 + __ROL4__(v72 + v31 - 0x5C6BE + (v34 ^ v32 ^ v33), 4);
		v36 = v35 + __ROL4__(v75 + v32 - 0x788E097F + (v35 ^ v33 ^ v34), 0xB);
		v37 = v36 + __ROL4__(v78 + v33 + 0x6D9D6122 + (v36 ^ v34 ^ v35), 0x10);
		v38 = v37 + __ROR4__(v81 + v34 - 0x21AC7F4 + (v37 ^ v35 ^ v36), 9);
		v39 = v38 + __ROL4__(v68 + v35 - 0x5B4115BC + (v38 ^ v36 ^ v37), 4);
		v40 = v39 + __ROL4__(v71 + v36 + 0x4BDECFA9 + (v39 ^ v37 ^ v38), 0xB);
		v41 = v40 + __ROL4__(v74 + v37 - 0x944B4A0 + (v40 ^ v38 ^ v39), 0x10);
		v42 = v41 + __ROR4__(v77 + v38 - 0x41404390 + (v41 ^ v39 ^ v40), 9);
		v43 = v42 + __ROL4__(v80 + v39 + 0x289B7EC6 + (v42 ^ v40 ^ v41), 4);
		v44 = v43 + __ROL4__(v85 + v40 - 0x155ED806 + (v43 ^ v41 ^ v42), 0xB);
		v45 = v44 + __ROL4__(v70 + v41 - 0x2B10CF7B + (v44 ^ v42 ^ v43), 0x10);
		v46 = v45 + __ROR4__(v73 + v42 + 0x4881D05 + (v45 ^ v43 ^ v44), 9);
		v47 = v46 + __ROL4__(v76 + v43 - 0x262B2FC7 + (v46 ^ v44 ^ v45), 4);
		v48 = v47 + __ROL4__(v79 + v44 - 0x1924661B + (v47 ^ v45 ^ v46), 0xB);
		v49 = v48 + __ROL4__(v66 + v45 + 0x1FA27CF8 + (v48 ^ v46 ^ v47), 0x10);
		v50 = v49 + __ROR4__(v69 + v46 - 0x3B53A99B + (v49 ^ v47 ^ v48), 9);
		v51 = v50 + __ROL4__(v85 + v47 - 0xBD6DDBC + (v49 ^ (v50 | ~v48)), 6);
		v52 = v51 + __ROL4__(v74 + v48 + 0x432AFF97 + (v50 ^ (v51 | ~v49)), 0xA);
		v53 = v52 + __ROL4__(v81 + v49 - 0x546BDC59 + (v51 ^ (v52 | ~v50)), 0xF);
		v54 = v53 + __ROR4__(v72 + v50 - 0x36C5FC7 + (v52 ^ (v53 | ~v51)), 0xB);
		v55 = v54 + __ROL4__(v79 + v51 + 0x655B59C3 + (v53 ^ (v54 | ~v52)), 6);
		v56 = v55 + __ROL4__(v70 + v52 - 0x70F3336E + (v54 ^ (v55 | ~v53)), 0xA);
		v57 = v56 + __ROL4__(v77 + v53 - 0x100B83 + (v55 ^ (v56 | ~v54)), 0xF);
		v58 = v57 + __ROR4__(v68 + v54 - 0x7A7BA22F + (v56 ^ (v57 | ~v55)), 0xB);
		v59 = v58 + __ROL4__(v75 + v55 + 0x6FA87E4F + (v57 ^ (v58 | ~v56)), 6);
		v60 = v59 + __ROL4__(v66 + v56 - 0x1D31920 + (v58 ^ (v59 | ~v57)), 0xA);
		v61 = v60 + __ROL4__(v73 + v57 - 0x5CFEBCEC + (v59 ^ (v60 | ~v58)), 0xF);
		v62 = v61 + __ROR4__(v80 + v58 + 0x4E0811A1 + (v60 ^ (v61 | ~v59)), 0xB);
		v63 = v62 + __ROL4__(v71 + v59 - 0x8AC817E + (v61 ^ (v62 | ~v60)), 6);
		v64 = v63 + __ROL4__(v78 + v60 - 0x42C50DCB + (v62 ^ (v63 | ~v61)), 0xA);
		v65 = v64 + __ROL4__(v69 + v61 + 0x2AD7D2BB + (v63 ^ (v64 | ~v62)), 0xF);
		*result = v82 + v63;
		result[3] = v84 + v64;
		result[1] = v65 + v67 + __ROR4__(v76 + v62 - 0x14792C6F + (v64 ^ (v65 | ~v63)), 0xB);
		result[2] = v83 + v65;
	}
	/*internal static unsafe void ProcessChunk(int* result, int* a2)
	{
		int v2; // r11
		int v3; // r7
		int v4; // t2
		int v5; // lr
		int v6; // t2
		int v7; // r10
		int v8; // t2
		int v9; // r6
		int v10; // t2
		int v11; // r7
		int v12; // t2
		int v13; // lr
		int v14; // t2
		int v15; // r8
		int v16; // r10
		int v17; // r3
		int v18; // t2
		int v19; // r8
		int v20; // t2
		int v21; // r7
		int v22; // t2
		int v23; // r5
		int v24; // lr
		int v25; // r6
		int v26; // t2
		int v27; // r3
		int v28; // t2
		int v29; // r4
		int v30; // r8
		int v31; // r4
		int v32; // t2
		int v33; // r9
		int v34; // r7
		int v35; // t2
		int v36; // r6
		int v37; // t2
		int v38; // r2
		int v39; // r12
		int v40; // r5
		int v41; // t2
		int v42; // r1
		int v43; // r7
		int v44; // t2
		int v45; // r3
		int v46; // r2
		int v47; // t2
		int v48; // r5
		int v49; // r3
		int v50; // t2
		int v51; // r1
		int v52; // r7
		int v53; // t2
		int v54; // r1
		int v55; // r2
		int v56; // t2
		int v57; // r5
		int v58; // t2
		int v59; // r3
		int v60; // r1
		int v61; // t2
		int v62; // r7
		int v63; // t2
		int v64; // r2
		int v65; // t2
		int v66; // r3
		int v67; // t2
		int v68; // r5
		int v69; // r1
		int v70; // t2
		int v71; // r7
		int v72; // r4
		int v73; // t2
		int v74; // r2
		int v75; // t2
		int v76; // r1
		int v77; // t2
		int v78; // r7
		int v79; // r3
		int v80; // t2
		int v81; // r4
		int v82; // t2
		int v83; // r2
		int v84; // r1
		int v85; // t2
		int v86; // r2
		int v87; // r7
		int v88; // t2
		int v89; // r4
		int v90; // t2
		int v91; // r3
		int v92; // t2
		int v93; // r1
		int v94; // t2
		int v95; // r7
		int v96; // t2
		int v97; // r2
		int v98; // r4
		int v99; // r2
		int v100; // t2
		int v101; // r4
		int v102; // r1
		int v103; // t2
		int v104; // r3
		int v105; // t2
		int v106; // r7
		int v107; // t2
		int v108; // r10
		int v109; // t2
		int v110; // r1
		int v111; // t2
		int v112; // r3
		int v113; // t2
		int v114; // r7
		int v115; // t2
		int v116; // r2
		int v117; // t2
		int v118; // r1
		int v119; // t2
		int v120; // r3
		int v121; // t2
		int v122; // r7
		int v123; // t2
		int v124; // r2
		int v125; // t2
		int v126; // r1
		int v127; // t2
		int v128; // r3
		int v129; // t2
		int v130; // r6
		int v131; // t2
		int v132; // r2
		int v133; // t2
		int v134; // t2
		int v135; // r3
		int v136; // [sp+0h] [bp-58h]
		int v137; // [sp+4h] [bp-54h]
		int v138; // [sp+8h] [bp-50h]
		int v139; // [sp+Ch] [bp-4Ch]
		int v140; // [sp+10h] [bp-48h]
		int v141; // [sp+14h] [bp-44h]
		int v142; // [sp+18h] [bp-40h]
		int v143; // [sp+1Ch] [bp-3Ch]
		int v144; // [sp+20h] [bp-38h]
		int v145; // [sp+24h] [bp-34h]
		int v146; // [sp+28h] [bp-30h]
		int v147; // [sp+2Ch] [bp-2Ch]

		v136 = *a2;
		v2 = result[1];
		v4 = __ROR4__(*a2 - 0x28955B88 + *result + ((result[3] & ~v2) | (result[2] & v2)), 0x19);
		v3 = v2 + v4;
		v137 = a2[1];
		v6 = __ROR4__(v137 - 0x173848AA + result[3] + ((result[2] & ~(v2 + v4)) | ((v2 + v4) & v2)), 0x14);
		v5 = v3 + v6;
		v138 = a2[2];
		v8 = __ROR4__(v138 + 0x242070DB + result[2] + ((v2 & ~(v3 + v6)) | ((v3 + v6) & v3)), 0xF);
		v7 = v5 + v8;
		v139 = a2[3];
		v10 = __ROR4__(((v3 & ~(v5 + v8)) | ((v5 + v8) & v5)) + v139 - 0x3E423112 + v2, 0xA);
		v9 = v7 + v10;
		v140 = a2[4];
		v141 = a2[5];
		v12 = __ROR4__(((v5 & ~(v7 + v10)) | ((v7 + v10) & v7)) + v140 - 0xA83F051 + v3, 0x19);
		v11 = v9 + v12;
		v14 = __ROR4__(((v7 & ~(v9 + v12)) | ((v9 + v12) & v9)) + v141 + 0x4787C62A + v5, 0x14);
		v13 = v11 + v14;
		v142 = a2[6];
		v15 = v142 - 0x57CFB9ED + v7;
		v16 = a2[7];
		v18 = __ROR4__((((v11 + v14) & v11) | (v9 & ~(v11 + v14))) + v15, 0xF);
		v17 = v13 + v18;
		v20 = __ROR4__(((v11 & ~(v13 + v18)) | ((v13 + v18) & v13)) + v16 - 0x2B96AFF + v9, 0xA);
		v19 = v17 + v20;
		v143 = a2[8];
		v22 = __ROR4__(((v13 & ~(v17 + v20)) | ((v17 + v20) & v17)) + v143 + 0x698098D8 + v11, 0x19);
		v21 = v19 + v22;
		v144 = a2[9];
		v23 = v144 - 0x74BB0851 + v13;
		v24 = a2[0xA];
		v26 = __ROR4__(((v17 & ~(v19 + v22)) | ((v19 + v22) & v19)) + v23, 0x14);
		v25 = v21 + v26;
		v145 = a2[0xB];
		v28 = __ROR4__((((v21 + v26) & v21) | (v19 & ~(v21 + v26))) + v24 - 0xA44F + v17, 0xF);
		v27 = v25 + v28;
		v29 = ((v21 & ~(v25 + v28)) | ((v25 + v28) & v25)) + v19 + v145 - 0x76A32842;
		v30 = a2[0xC];
		v32 = __ROR4__(v29, 0xA);
		v31 = v27 + v32;
		v33 = a2[0xE];
		v146 = a2[0xD];
		v35 = __ROR4__(((v25 & ~(v27 + v32)) | ((v27 + v32) & v27)) + v30 + 0x6B901122 + v21, 0x19);
		v34 = v31 + v35;
		v37 = __ROR4__(((v27 & ~(v31 + v35)) | ((v31 + v35) & v31)) + v146 - 0x2678E6D + v25, 0x14);
		v36 = v34 + v37;
		v38 = ~(v34 + v37);
		v39 = a2[0xF];
		v41 = __ROR4__(((v38 & v31) | ((v34 + v37) & v34)) + v27 + v33 - 0x5986BC72, 0xF);
		v40 = v36 + v41;
		v147 = ~(v36 + v41);
		v42 = v36 + v41 + __ROR4__(v39 + 0x49B40821 + v31 + (((v36 + v41) & v36) | (v147 & v34)), 0xA);
		v44 = __ROR4__(v34 + v137 - 0x9E1DA9E + ((v42 & v36) | (v38 & (v36 + v41))), 0x1B);
		v43 = v42 + v44;
		v45 = (v42 + v44) & ~v42;
		v47 = __ROR4__((((v42 + v44) & v40) | (v147 & v42)) + v36 + v142 - 0x3FBF4CC0, 0x17);
		v46 = v43 + v47;
		v48 = v43 + v47 + __ROR4__(v40 + v145 + 0x265E5A51 + (((v43 + v47) & v42) | v45), 0x12);
		v50 = __ROR4__(((v48 & v43) | ((v43 + v47) & ~v43)) + v42 + v136 - 0x16493856, 0xC);
		v49 = v48 + v50;
		v51 = (v48 + v50) & ~v48;
		v53 = __ROR4__(v43 + v141 - 0x29D0EFA3 + (((v48 + v50) & v46) | (v48 & ~v46)), 0x1B);
		v52 = v49 + v53;
		v54 = v49 + v53 + __ROR4__((v51 | ((v49 + v53) & v48)) + v46 + v24 + 0x2441453, 0x17);
		v56 = __ROR4__((((v49 + v53) & ~v49) | (v54 & v49)) + v48 + v39 - 0x275E197F, 0x12);
		v55 = v54 + v56;
		v58 = __ROR4__((((v54 + v56) & v52) | (v54 & ~v52)) + v49 + v140 - 0x182C0438, 0xC);
		v57 = v55 + v58;
		v59 = v55 + v58 + __ROR4__(((v55 & ~v54) | ((v55 + v58) & v54)) + v52 + v144 + 0x21E1CDE6, 0x1B);
		v61 = __ROR4__(v54 + v33 - 0x3CC8F82A + (((v55 + v58) & ~v55) | (v59 & v55)), 0x17);
		v60 = v59 + v61;
		v63 = __ROR4__((((v59 + v61) & v57) | (v59 & ~v57)) + v55 + v139 - 0xB2AF279, 0x12);
		v62 = v60 + v63;
		v65 = __ROR4__(((v60 & ~v59) | ((v60 + v63) & v59)) + v57 + v143 + 0x455A14ED, 0xC);
		v64 = v62 + v65;
		v67 = __ROR4__(v59 + v146 - 0x561C16FB + ((v62 & ~v60) | ((v62 + v65) & v60)), 0x1B);
		v66 = v64 + v67;
		v68 = (v64 + v67) & ~v64;
		v70 = __ROR4__(v60 + v138 - 0x3105C08 + (((v64 + v67) & v62) | (v64 & ~v62)), 0x17);
		v69 = v66 + v70;
		v71 = v66 + v70 + __ROR4__(v62 + v16 + 0x676F02D9 + (((v66 + v70) & v64) | v68), 0x12);
		v73 = __ROR4__(((v71 & v66) | ((v66 + v70) & ~v66)) + v64 + v30 - 0x72D5B376, 0xC);
		v72 = v71 + v73;
		v75 = __ROR4__((v71 ^ v69 ^ (v71 + v73)) + v66 + v141 - 0x5C6BE, 0x1C);
		v74 = v72 + v75;
		v77 = __ROR4__(v69 + v143 - 0x788E097F + (v72 ^ v71 ^ (v72 + v75)), 0x15);
		v76 = v74 + v77;
		v78 = v74 + v77 + __ROR4__(v71 + v145 + 0x6D9D6122 + (v74 ^ v72 ^ (v74 + v77)), 0x10);
		v80 = __ROR4__(((v74 + v77) ^ v74 ^ v78) + v72 + v33 - 0x21AC7F4, 9);
		v79 = v78 + v80;
		v82 = __ROR4__((v78 ^ v76 ^ (v78 + v80)) + v74 + v137 - 0x5B4115BC, 0x1C);
		v81 = v79 + v82;
		v83 = (v79 + v82) ^ v79;
		v85 = __ROR4__(v76 + v140 + 0x4BDECFA9 + (v79 ^ v78 ^ (v79 + v82)), 0x15);
		v84 = v81 + v85;
		v86 = v81 + v85 + __ROR4__((v83 ^ (v81 + v85)) + v78 + v16 - 0x944B4A0, 0x10);
		v88 = __ROR4__(((v81 + v85) ^ v81 ^ v86) + v79 + v24 - 0x41404390, 9);
		v87 = v86 + v88;
		v90 = __ROR4__(v81 + v146 + 0x289B7EC6 + (v86 ^ v84 ^ (v86 + v88)), 0x1C);
		v89 = v87 + v90;
		v92 = __ROR4__((v87 ^ v86 ^ (v87 + v90)) + v84 + v136 - 0x155ED806, 0x15);
		v91 = v89 + v92;
		v94 = __ROR4__((v89 ^ v87 ^ (v89 + v92)) + v86 + v139 - 0x2B10CF7B, 0x10);
		v93 = v91 + v94;
		v96 = __ROR4__(v87 + v142 + 0x4881D05 + (v91 ^ v89 ^ (v91 + v94)), 9);
		v95 = v93 + v96;
		v97 = (v93 ^ v91 ^ (v93 + v96)) + v89 + v144 - 0x262B2FC7;
		v98 = (v93 + v96) ^ v93;
		v100 = __ROR4__(v97, 0x1C);
		v99 = v95 + v100;
		v101 = v95 + v100 + __ROR4__((v98 ^ (v95 + v100)) + v91 + v30 - 0x1924661B, 0x15);
		v103 = __ROR4__(v93 + v39 + 0x1FA27CF8 + ((v95 + v100) ^ v95 ^ v101), 0x10);
		v102 = v101 + v103;
		v105 = __ROR4__((v101 ^ v99 ^ (v101 + v103)) + v95 + v138 - 0x3B53A99B, 9);
		v104 = v102 + v105;
		v107 = __ROR4__((((v102 + v105) | ~v101) ^ v102) + v99 + v136 - 0xBD6DDBC, 0x1A);
		v106 = v104 + v107;
		v109 = __ROR4__((((v104 + v107) | ~v102) ^ v104) + v101 + v16 + 0x432AFF97, 0x16);
		v108 = v106 + v109;
		v111 = __ROR4__(v102 + v33 - 0x546BDC59 + (((v106 + v109) | ~v104) ^ v106), 0x11);
		v110 = v108 + v111;
		v113 = __ROR4__(v104 + v141 - 0x36C5FC7 + (((v108 + v111) | ~v106) ^ v108), 0xB);
		v112 = v110 + v113;
		v115 = __ROR4__(v106 + v30 + 0x655B59C3 + (((v110 + v113) | ~v108) ^ v110), 0x1A);
		v114 = v112 + v115;
		v117 = __ROR4__(v139 - 0x70F3336E + v108 + (((v112 + v115) | ~v110) ^ v112), 0x16);
		v116 = v114 + v117;
		v119 = __ROR4__(v110 + v24 - 0x100B83 + (((v114 + v117) | ~v112) ^ v114), 0x11);
		v118 = v116 + v119;
		v121 = __ROR4__(v112 + v137 - 0x7A7BA22F + (((v116 + v119) | ~v114) ^ v116), 0xB);
		v120 = v118 + v121;
		v123 = __ROR4__(v114 + v143 + 0x6FA87E4F + (((v118 + v121) | ~v116) ^ v118), 0x1A);
		v122 = v120 + v123;
		v125 = __ROR4__(v116 + v39 - 0x1D31920 + (((v120 + v123) | ~v118) ^ v120), 0x16);
		v124 = v122 + v125;
		v127 = __ROR4__(v118 + v142 - 0x5CFEBCEC + (((v122 + v125) | ~v120) ^ v122), 0x11);
		v126 = v124 + v127;
		v129 = __ROR4__(v120 + v146 + 0x4E0811A1 + (((v124 + v127) | ~v122) ^ v124), 0xB);
		v128 = v126 + v129;
		v131 = __ROR4__((((v126 + v129) | ~v124) ^ v126) + v122 + v140 - 0x8AC817E, 0x1A);
		v130 = v128 + v131;
		v133 = __ROR4__(v124 + v145 - 0x42C50DCB + (((v128 + v131) | ~v126) ^ v128), 0x16);
		v132 = v130 + v133;
		v134 = __ROR4__(v126 + v138 + 0x2AD7D2BB + (((v130 + v133) | ~v128) ^ v130), 0x11);
		*result += v130;
		result[1] = v132 + v134 + v2 + __ROR4__(v128 + v144 - 0x14792C6F + (((v132 + v134) | ~v130) ^ v132), 0xB);
		v135 = result[3];
		result[2] += v132 + v134;
		result[3] = v132 + v135;
	}
	*/
	internal static uint __ROR4__(uint value, int shift)
	{
		return BitOperations.RotateRight(value, shift);
		//return (value >>> shift) | ((32 - shift) >>> value);

		//int dst = value;
		//int count = shift & 31;

		//do
		//{
		//	int tmp = SetBit(0, 31, GetBit(dst, 0));

		//	dst = (dst >> 1) | tmp;
		//	count--;
		//}
		//while (count > 0);

		//return dst;
	}
	internal static uint __ROL4__(_DWORD value, int shift)
	{
		return BitOperations.RotateLeft(value, shift);
		//return (shift >>> value) | (value >>> (32 - shift));
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool GetBit(int b, int bitNumber)
	{
		return (b & (1 << bitNumber)) != 0;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int SetBit(int value, int bitPosition, bool toSet)
	{
		return value | ((toSet ? 1 : 0) << bitPosition);
	}
}
