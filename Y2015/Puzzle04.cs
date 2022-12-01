using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
namespace AdventOfCode.Y2015 {
    [Description("The Ideal Stocking Stuffer")]
    public class Puzzle04 : ASolver {
        [Description("Find MD5 hash that starts with five zeroes.")]
        public override string SolvePart1() {
            int suffix = -1;
            int answer = 0;

            Parallel.For(0, 1, x => {
                byte[] data = new byte[Input.Length + 8];
                for (int i = 0; i < Input.Length; i++) {
                    data[i] = (byte)Input[i];
                }

                do {
                    int val = Interlocked.Increment(ref suffix);
                    int original = val;
                    int div = (int)Math.Pow(10, val == 0 ? 0 : (int)Math.Log10(val));
                    int pos = Input.Length;
                    while (div > 0) {
                        data[pos++] = (byte)((val / div) + 0x30);
                        val = val - (val / div) * div;
                        div /= 10;
                    }

                    if (GetMD5Hash(data, pos, 5)) {
                        answer = original;
                        return;
                    }
                } while (answer == 0);
            });

            return $"{answer}";
        }

        [Description("Find one that starts with six zeroes.")]
        public override string SolvePart2() {
            int suffix = -1;
            int answer = 0;

            Parallel.For(0, Environment.ProcessorCount, x => {
                byte[] data = new byte[Input.Length + 8];
                for (int i = 0; i < Input.Length; i++) {
                    data[i] = (byte)Input[i];
                }

                do {
                    int val = Interlocked.Increment(ref suffix);
                    int original = val;
                    int div = (int)Math.Pow(10, val == 0 ? 0 : (int)Math.Log10(val));
                    int pos = Input.Length;
                    while (div > 0) {
                        data[pos++] = (byte)((val / div) + 0x30);
                        val = val - (val / div) * div;
                        div /= 10;
                    }

                    if (GetMD5Hash(data, pos, 6)) {
                        answer = original;
                        return;
                    }
                } while (answer == 0);
            });

            return $"{answer}";
        }

        private bool GetMD5Hash(byte[] input, int length, int zeroLength) {
            byte[] hash = MD5.Compute(input, length);
            if (zeroLength == 6) {
                return hash[0] == 0 && hash[1] == 0 && hash[2] == 0;
            }
            return hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xf0) == 0;
        }
    }
}