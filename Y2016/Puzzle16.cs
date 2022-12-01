using AdventOfCode.Core;
using System;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2016 {
    [Description("Dragon Checksum")]
    public class Puzzle16 : ASolver {
        [Description("What is the correct checksum?")]
        public override string SolvePart1() {
            return GetChecksum(272);
        }

        [Description("What is the correct checksum for this disk?")]
        public override string SolvePart2() {
            return GetChecksum(35651584);
        }

        private string GetChecksum(int length) {
            byte[] disk = new byte[length << 1];

            for (int i = 0; i < Input.Length; i++) {
                disk[i] = (byte)(Input[i] - '0');
            }

            int count = Input.Length;
            while (count < length) {
                Array.Reverse(disk, 0, count);
                Array.Copy(disk, 0, disk, count + 1, count);
                for (int i = count + count; i > count; i--) {
                    disk[i] ^= 1;
                }
                Array.Reverse(disk, 0, count);
                count = (count << 1) + 1;
            }

            int checksum = length;
            while ((checksum & 1) == 0) {
                int index = 0;
                int compare = 0;
                while (compare < checksum) {
                    disk[index++] = (byte)(~(disk[compare] ^ disk[compare + 1]) & 1);
                    compare += 2;
                }
                checksum >>= 1;
            }

            for (int i = 0; i < checksum; i++) {
                disk[i] += 0x30;
            }
            return Encoding.ASCII.GetString(disk, 0, checksum);
        }
    }
}