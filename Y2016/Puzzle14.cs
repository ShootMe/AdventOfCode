using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace AdventOfCode.Y2016 {
    [Description("One-Time Pad")]
    public class Puzzle14 : ASolver {
        private List<HashInfo> hashes = new List<HashInfo>(11000);

        [Description("What index produces your 64th one-time pad key?")]
        public override string SolvePart1() {
            hashes.Clear();

            int foundKeys = 0;
            int lastIndex = 0;
            int index = 0;
            while (foundKeys < 64) {
                if (index >= hashes.Count) {
                    HashKeys(false);
                }

                HashInfo hash = hashes[index];
                if (hash.ThreeConsecutive != 0) {
                    for (int i = 1; i < 1001; i++) {
                        if (index + i >= hashes.Count) {
                            HashKeys(false);
                        }

                        HashInfo next = hashes[index + i];
                        if (next.FiveConsecutive == hash.ThreeConsecutive) {
                            foundKeys++;
                            lastIndex = index;
                            break;
                        }
                    }
                }
                index++;
            }

            return $"{lastIndex}";
        }

        [Description("What index now produces your 64th one-time pad key?")]
        public override string SolvePart2() {
            hashes.Clear();

            int foundKeys = 0;
            int lastIndex = 0;
            int index = 0;
            while (foundKeys < 64) {
                if (index >= hashes.Count) {
                    HashKeys(true);
                }

                HashInfo hash = hashes[index];
                if (hash.ThreeConsecutive != 0) {
                    for (int i = 1; i < 1001; i++) {
                        if (index + i >= hashes.Count) {
                            HashKeys(true);
                        }

                        HashInfo next = hashes[index + i];
                        if (next.FiveConsecutive == hash.ThreeConsecutive) {
                            foundKeys++;
                            lastIndex = index;
                            break;
                        }
                    }
                }
                index++;
            }

            return $"{lastIndex}";
        }

        private void HashKeys(bool extraHashes) {
            int cpuCount = Environment.ProcessorCount;
            int toAdd = 5000 / cpuCount + 1;
            int end = hashes.Count + toAdd * cpuCount;
            int suffix = hashes.Count;
            for (int i = suffix; i < end; i++) {
                hashes.Add(new HashInfo(null));
            }

            Parallel.For(0, cpuCount, x => {
                byte[] data = new byte[Input.Length + 8];
                for (int i = 0; i < Input.Length; i++) {
                    data[i] = (byte)Input[i];
                }
                byte[] stringData = new byte[32];

                for (int j = 0; j < toAdd; j++) {
                    int index = Interlocked.Increment(ref suffix) - 1;
                    int val = index;
                    int div = (int)Math.Pow(10, val == 0 ? 0 : (int)Math.Log10(val));
                    int pos = Input.Length;
                    while (div > 0) {
                        data[pos++] = (byte)((val / div) + 0x30);
                        val = val - (val / div) * div;
                        div /= 10;
                    }

                    byte[] hash = MD5.Compute(data, pos);
                    HashInfo info = hashes[index];
                    info.Hash = hash;
                    if (extraHashes) {
                        for (int i = 0; i < 2016; i++) {
                            info.Stringify(stringData);
                            info.Hash = MD5.Compute(stringData);
                        }
                    }
                    info.ConsecutiveCount();
                }
            });
        }

        private class HashInfo {
            private static readonly byte[] Values = new byte[256];
            private static readonly byte[] Hex = { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66 };

            static HashInfo() {
                for (byte i = 0; i < 16; i++) {
                    Values[i] = Hex[i];
                    Values[i << 4] = Hex[i];
                }
            }

            public byte[] Hash;
            public byte ThreeConsecutive;
            public byte FiveConsecutive;

            public HashInfo(byte[] hash) {
                Hash = hash;

                ThreeConsecutive = 0;
                FiveConsecutive = 0;
            }
            public void Stringify(byte[] data) {
                byte mask = 0xf0;
                int index = 0;
                for (int i = 0; i < 32; i++) {
                    data[i] = Values[Hash[index] & mask];

                    mask = (byte)((mask << 4) | (mask >> 4));
                    if (mask == 0xf0) {
                        index++;
                    }
                }
            }
            public void ConsecutiveCount() {
                byte mask = 0xf0;
                byte lastVal = 16;
                int index = 0;
                int currentCount = 1;

                for (int i = 0; i < 32; i++) {
                    byte val = Values[Hash[index] & mask];

                    if (lastVal == val) {
                        currentCount++;
                        if (currentCount == 3 && ThreeConsecutive == 0) {
                            ThreeConsecutive = val;
                        } else if (currentCount == 5 && FiveConsecutive == 0) {
                            FiveConsecutive = val;
                            break;
                        }
                    } else {
                        currentCount = 1;
                    }

                    lastVal = val;
                    mask = (byte)((mask << 4) | (mask >> 4));
                    if (mask == 0xf0) {
                        index++;
                    }
                }
            }
            public override string ToString() {
                StringBuilder hex = new StringBuilder(32);
                for (int i = 0; i < 16; i++) {
                    hex.Append($"{Hash[i]:x}");
                }
                return hex.ToString();
            }
        }
    }
}