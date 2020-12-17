using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace AdventOfCode.Common {
	public class MD5 {
        public static byte[] Compute(byte[] data, int lengthToUse = 0) {
            lengthToUse = lengthToUse == 0 ? data.Length : lengthToUse;
            ulong bits = (ulong)lengthToUse << 3;

            MD5State state = MD5State.Create();

            int index = 0;
            while (lengthToUse >= 64) {
                lengthToUse -= 64;
                state.Transform(data, index);
                index += 64;
            }

            return state.TransformFinal(data, index, lengthToUse, bits);
        }

        [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 80)]
        public unsafe struct MD5State {
            private const int S00 = 7, S01 = 12, S02 = 17, S03 = 22;
            private const int S10 = 5, S11 = 9, S12 = 14, S13 = 20;
            private const int S20 = 4, S21 = 11, S22 = 16, S23 = 23;
            private const int S30 = 6, S31 = 10, S32 = 15, S33 = 21;

            [FieldOffset(0)]
            public fixed byte Hash[16];
            [FieldOffset(0)]
            public uint A;
            [FieldOffset(4)]
            public uint B;
            [FieldOffset(8)]
            public uint C;
            [FieldOffset(12)]
            public uint D;
            [FieldOffset(16)]
            public fixed uint Calc[16];

            public static MD5State Create() {
                return new MD5State() {
                    A = 0x67452301,
                    B = 0xefcdab89,
                    C = 0x98badcfe,
                    D = 0x10325476
                };
            }

            public void Transform(byte[] data, int startIndex) {
                fixed (uint* ptrHash = Calc) {
                    fixed (byte* ptrData = data) {
                        Buffer.MemoryCopy(ptrData + startIndex, ptrHash, 64, 64);
                    }
                }

                Update();
            }
            public byte[] TransformFinal(byte[] data, int startIndex, int length, ulong totalBits) {
                fixed (uint* ptrCalc = Calc) {
                    fixed (byte* ptrData = data) {
                        Span<uint> calc = new Span<uint>(ptrCalc, 16);
                        calc.Clear();

                        uint padding = 128u << ((length & 3) << 3);
                        calc[length >> 2] |= padding;
                        if (length > 0) {
                            Buffer.MemoryCopy(ptrData + startIndex, ptrCalc, length, length);
                        }

                        if (length >= 56) {
                            Update();
                            calc.Clear();
                        }

                        calc[14] = (uint)totalBits;
                        calc[15] = (uint)(totalBits >> 32);
                    }
                }

                Update();

                byte[] hash = new byte[16];
                fixed (byte* ptrResult = hash) {
                    fixed (byte* ptrHash = Hash) {
                        Buffer.MemoryCopy(ptrHash, ptrResult, 16, 16);
                    }
                }
                return hash;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Update() {
                uint a = A; uint b = B;
                uint c = C; uint d = D;
                UpdateF();
                UpdateG();
                UpdateH();
                UpdateI();
                A += a; B += b;
                C += c; D += d;
            }
            private void UpdateF() {
                A += ((B & C) | (~B & D)) + Calc[0] + 0xd76aa478u;
                A = ((A << S00) | (A >> (32 - S00))) + B;
                D += ((A & B) | (~A & C)) + Calc[1] + 0xe8c7b756u;
                D = ((D << S01) | (D >> (32 - S01))) + A;
                C += ((D & A) | (~D & B)) + Calc[2] + 0x242070dbu;
                C = ((C << S02) | (C >> (32 - S02))) + D;
                B += ((C & D) | (~C & A)) + Calc[3] + 0xc1bdceeeu;
                B = ((B << S03) | (B >> (32 - S03))) + C;

                A += ((B & C) | (~B & D)) + Calc[4] + 0xf57c0fafu;
                A = ((A << S00) | (A >> (32 - S00))) + B;
                D += ((A & B) | (~A & C)) + Calc[5] + 0x4787c62au;
                D = ((D << S01) | (D >> (32 - S01))) + A;
                C += ((D & A) | (~D & B)) + Calc[6] + 0xa8304613u;
                C = ((C << S02) | (C >> (32 - S02))) + D;
                B += ((C & D) | (~C & A)) + Calc[7] + 0xfd469501u;
                B = ((B << S03) | (B >> (32 - S03))) + C;

                A += ((B & C) | (~B & D)) + Calc[8] + 0x698098d8u;
                A = ((A << S00) | (A >> (32 - S00))) + B;
                D += ((A & B) | (~A & C)) + Calc[9] + 0x8b44f7afu;
                D = ((D << S01) | (D >> (32 - S01))) + A;
                C += ((D & A) | (~D & B)) + Calc[10] + 0xffff5bb1u;
                C = ((C << S02) | (C >> (32 - S02))) + D;
                B += ((C & D) | (~C & A)) + Calc[11] + 0x895cd7beu;
                B = ((B << S03) | (B >> (32 - S03))) + C;

                A += ((B & C) | (~B & D)) + Calc[12] + 0x6b901122u;
                A = ((A << S00) | (A >> (32 - S00))) + B;
                D += ((A & B) | (~A & C)) + Calc[13] + 0xfd987193u;
                D = ((D << S01) | (D >> (32 - S01))) + A;
                C += ((D & A) | (~D & B)) + Calc[14] + 0xa679438eu;
                C = ((C << S02) | (C >> (32 - S02))) + D;
                B += ((C & D) | (~C & A)) + Calc[15] + 0x49b40821u;
                B = ((B << S03) | (B >> (32 - S03))) + C;
            }
            private void UpdateG() {
                A += ((D & B) | (~D & C)) + Calc[1] + 0xf61e2562u;
                A = ((A << S10) | (A >> (32 - S10))) + B;
                D += ((C & A) | (~C & B)) + Calc[6] + 0xc040b340u;
                D = ((D << S11) | (D >> (32 - S11))) + A;
                C += ((B & D) | (~B & A)) + Calc[11] + 0x265e5a51u;
                C = ((C << S12) | (C >> (32 - S12))) + D;
                B += ((A & C) | (~A & D)) + Calc[0] + 0xe9b6c7aau;
                B = ((B << S13) | (B >> (32 - S13))) + C;

                A += ((D & B) | (~D & C)) + Calc[5] + 0xd62f105du;
                A = ((A << S10) | (A >> (32 - S10))) + B;
                D += ((C & A) | (~C & B)) + Calc[10] + 0x02441453u;
                D = ((D << S11) | (D >> (32 - S11))) + A;
                C += ((B & D) | (~B & A)) + Calc[15] + 0xd8a1e681u;
                C = ((C << S12) | (C >> (32 - S12))) + D;
                B += ((A & C) | (~A & D)) + Calc[4] + 0xe7d3fbc8u;
                B = ((B << S13) | (B >> (32 - S13))) + C;

                A += ((D & B) | (~D & C)) + Calc[9] + 0x21e1cde6u;
                A = ((A << S10) | (A >> (32 - S10))) + B;
                D += ((C & A) | (~C & B)) + Calc[14] + 0xc33707d6u;
                D = ((D << S11) | (D >> (32 - S11))) + A;
                C += ((B & D) | (~B & A)) + Calc[3] + 0xf4d50d87u;
                C = ((C << S12) | (C >> (32 - S12))) + D;
                B += ((A & C) | (~A & D)) + Calc[8] + 0x455a14edu;
                B = ((B << S13) | (B >> (32 - S13))) + C;

                A += ((D & B) | (~D & C)) + Calc[13] + 0xa9e3e905u;
                A = ((A << S10) | (A >> (32 - S10))) + B;
                D += ((C & A) | (~C & B)) + Calc[2] + 0xfcefa3f8u;
                D = ((D << S11) | (D >> (32 - S11))) + A;
                C += ((B & D) | (~B & A)) + Calc[7] + 0x676f02d9u;
                C = ((C << S12) | (C >> (32 - S12))) + D;
                B += ((A & C) | (~A & D)) + Calc[12] + 0x8d2a4c8au;
                B = ((B << S13) | (B >> (32 - S13))) + C;
            }
            private void UpdateH() {
                A += (B ^ C ^ D) + Calc[5] + 0xfffa3942u;
                A = ((A << S20) | (A >> (32 - S20))) + B;
                D += (A ^ B ^ C) + Calc[8] + 0x8771f681u;
                D = ((D << S21) | (D >> (32 - S21))) + A;
                C += (D ^ A ^ B) + Calc[11] + 0x6d9d6122u;
                C = ((C << S22) | (C >> (32 - S22))) + D;
                B += (C ^ D ^ A) + Calc[14] + 0xfde5380cu;
                B = ((B << S23) | (B >> (32 - S23))) + C;

                A += (B ^ C ^ D) + Calc[1] + 0xa4beea44u;
                A = ((A << S20) | (A >> (32 - S20))) + B;
                D += (A ^ B ^ C) + Calc[4] + 0x4bdecfa9u;
                D = ((D << S21) | (D >> (32 - S21))) + A;
                C += (D ^ A ^ B) + Calc[7] + 0xf6bb4b60u;
                C = ((C << S22) | (C >> (32 - S22))) + D;
                B += (C ^ D ^ A) + Calc[10] + 0xbebfbc70u;
                B = ((B << S23) | (B >> (32 - S23))) + C;

                A += (B ^ C ^ D) + Calc[13] + 0x289b7ec6u;
                A = ((A << S20) | (A >> (32 - S20))) + B;
                D += (A ^ B ^ C) + Calc[0] + 0xeaa127fau;
                D = ((D << S21) | (D >> (32 - S21))) + A;
                C += (D ^ A ^ B) + Calc[3] + 0xd4ef3085u;
                C = ((C << S22) | (C >> (32 - S22))) + D;
                B += (C ^ D ^ A) + Calc[6] + 0x04881d05u;
                B = ((B << S23) | (B >> (32 - S23))) + C;

                A += (B ^ C ^ D) + Calc[9] + 0xd9d4d039u;
                A = ((A << S20) | (A >> (32 - S20))) + B;
                D += (A ^ B ^ C) + Calc[12] + 0xe6db99e5u;
                D = ((D << S21) | (D >> (32 - S21))) + A;
                C += (D ^ A ^ B) + Calc[15] + 0x1fa27cf8u;
                C = ((C << S22) | (C >> (32 - S22))) + D;
                B += (C ^ D ^ A) + Calc[2] + 0xc4ac5665u;
                B = ((B << S23) | (B >> (32 - S23))) + C;
            }
            private void UpdateI() {
                A += (C ^ (B | ~D)) + Calc[0] + 0xf4292244u;
                A = ((A << S30) | (A >> (32 - S30))) + B;
                D += (B ^ (A | ~C)) + Calc[7] + 0x432aff97u;
                D = ((D << S31) | (D >> (32 - S31))) + A;
                C += (A ^ (D | ~B)) + Calc[14] + 0xab9423a7u;
                C = ((C << S32) | (C >> (32 - S32))) + D;
                B += (D ^ (C | ~A)) + Calc[5] + 0xfc93a039u;
                B = ((B << S33) | (B >> (32 - S33))) + C;

                A += (C ^ (B | ~D)) + Calc[12] + 0x655b59c3u;
                A = ((A << S30) | (A >> (32 - S30))) + B;
                D += (B ^ (A | ~C)) + Calc[3] + 0x8f0ccc92u;
                D = ((D << S31) | (D >> (32 - S31))) + A;
                C += (A ^ (D | ~B)) + Calc[10] + 0xffeff47du;
                C = ((C << S32) | (C >> (32 - S32))) + D;
                B += (D ^ (C | ~A)) + Calc[1] + 0x85845dd1u;
                B = ((B << S33) | (B >> (32 - S33))) + C;

                A += (C ^ (B | ~D)) + Calc[8] + 0x6fa87e4fu;
                A = ((A << S30) | (A >> (32 - S30))) + B;
                D += (B ^ (A | ~C)) + Calc[15] + 0xfe2ce6e0u;
                D = ((D << S31) | (D >> (32 - S31))) + A;
                C += (A ^ (D | ~B)) + Calc[6] + 0xa3014314u;
                C = ((C << S32) | (C >> (32 - S32))) + D;
                B += (D ^ (C | ~A)) + Calc[13] + 0x4e0811a1u;
                B = ((B << S33) | (B >> (32 - S33))) + C;

                A += (C ^ (B | ~D)) + Calc[4] + 0xf7537e82u;
                A = ((A << S30) | (A >> (32 - S30))) + B;
                D += (B ^ (A | ~C)) + Calc[11] + 0xbd3af235u;
                D = ((D << S31) | (D >> (32 - S31))) + A;
                C += (A ^ (D | ~B)) + Calc[2] + 0x2ad7d2bbu;
                C = ((C << S32) | (C >> (32 - S32))) + D;
                B += (D ^ (C | ~A)) + Calc[9] + 0xeb86d391u;
                B = ((B << S33) | (B >> (32 - S33))) + C;
            }
        }
    }
}