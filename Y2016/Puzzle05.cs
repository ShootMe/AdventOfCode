using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
namespace AdventOfCode.Y2016 {
    [Description("How About a Nice Game of Chess?")]
    public class Puzzle05 : ASolver {
        private static char[] intToHex = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        [Description("Given the actual Door ID, what is the password?")]
        public override string SolvePart1() {
            int suffix = -1;
            int length = 0;
            char[] password = new char[8];

            Parallel.For(0, Environment.ProcessorCount, x => {
                byte[] data = new byte[Input.Length + 8];
                for (int i = 0; i < Input.Length; i++) {
                    data[i] = (byte)Input[i];
                }

                do {
                    int val = Interlocked.Increment(ref suffix);
                    int div = (int)Math.Pow(10, val == 0 ? 0 : (int)Math.Log10(val));
                    int pos = Input.Length;
                    while (div > 0) {
                        data[pos++] = (byte)((val / div) + 0x30);
                        val = val - (val / div) * div;
                        div /= 10;
                    }

                    int hash = GetMD5Hash(data, pos);
                    if (hash >= 0) {
                        if (length < 8) {
                            password[length] = intToHex[hash];
                        } else {
                            return;
                        }
                        Interlocked.Increment(ref length);
                    }
                } while (length < 8);
            });

            if (length == 8) {
                return new string(password);
            }

            return string.Empty;
        }

        [Description("Given the actual Door ID and this new method, what is the password?")]
        public override string SolvePart2() {
            int suffix = -1;
            int length = 0;
            char[] password = new char[8];

            Parallel.For(0, Environment.ProcessorCount, x => {
                byte[] data = new byte[Input.Length + 8];
                for (int i = 0; i < Input.Length; i++) {
                    data[i] = (byte)Input[i];
                }

                do {
                    int val = Interlocked.Increment(ref suffix);
                    int div = (int)Math.Pow(10, val == 0 ? 0 : (int)Math.Log10(val));
                    int pos = Input.Length;
                    while (div > 0) {
                        data[pos++] = (byte)((val / div) + 0x30);
                        val = val - (val / div) * div;
                        div /= 10;
                    }

                    if (GetMD5Hash(data, password, pos)) {
                        Interlocked.Increment(ref length);
                    }
                } while (length < 8);
            });

            if (length == 8) {
                return new string(password);
            }

            return string.Empty;
        }

        private int GetMD5Hash(byte[] input, int length) {
            byte[] hash = MD5.Compute(input, length);
            if (hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xf0) == 0) {
                return hash[2] & 0xf;
            }
            return -1;
        }
        private bool GetMD5Hash(byte[] input, char[] output, int length) {
            byte[] hash = MD5.Compute(input, length);
            if (hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xf0) == 0) {
                int pos = hash[2] & 0xf;
                if (pos < 8 && output[pos] == 0) {
                    output[pos] = intToHex[(hash[3] & 0xf0) >> 4];
                    return true;
                }
            }
            return false;
        }
    }
}