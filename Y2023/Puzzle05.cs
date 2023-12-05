using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2023 {
    [Description("If You Give A Seed A Fertilizer")]
    public class Puzzle05 : ASolver {
        private List<Seed> seeds1, seeds2;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            ParseSeeds(lines[0]);

            int i = 3;
            for (int j = 0; j < 7; j++) {
                MapDetails(lines, ref i, j);
            }
        }
        private void ParseSeeds(string line) {
            string[] numbers = line.Substring(7).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            seeds1 = new List<Seed>(numbers.Length);
            seeds2 = new List<Seed>(numbers.Length / 2);
            for (int i = 0; i < numbers.Length; i++) {
                long id = numbers[i].ToLong();
                seeds1.Add(new Seed(id, 1));
            }
            for (int i = 0; i < numbers.Length; i += 2) {
                long id = numbers[i].ToLong();
                long length = numbers[i + 1].ToLong();
                seeds2.Add(new Seed(id, length));
            }
        }
        private void MapDetails(string[] lines, ref int i, int level) {
            for (; i < lines.Length; i++) {
                string line = lines[i];
                if (string.IsNullOrEmpty(line)) { i += 2; break; }

                string[] mapping = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                MapDetails(seeds1, level, mapping[0].ToLong(), mapping[1].ToLong(), mapping[2].ToLong());
                MapDetails(seeds2, level, mapping[0].ToLong(), mapping[1].ToLong(), mapping[2].ToLong());
            }
        }
        private void MapDetails(List<Seed> seedsToMap, int level, long destStart, long sourceStart, long length) {
            for (int i = 0; i < seedsToMap.Count; i++) {
                Seed seed = seedsToMap[i];
                long detail = seed.Details[level];
                long detailLength = seed.Lengths[level];
                if (detail < sourceStart + length && detail + detailLength > sourceStart) {
                    if (detail + detailLength > sourceStart + length) {
                        long newLength = detail + detailLength - sourceStart - length;
                        Seed newSeed = new Seed(seed);
                        newSeed.Details[level] = sourceStart + length;
                        newSeed.Lengths[level] = newLength;
                        seedsToMap.Add(newSeed);
                        seed.Lengths[level] -= newLength;
                    }
                    if (detail < sourceStart) {
                        long newLength = sourceStart - detail;
                        Seed newSeed = new Seed(seed);
                        newSeed.Lengths[level] = newLength;
                        seedsToMap.Add(newSeed);
                        detail += newLength;
                        seed.Details[level] += newLength;
                        seed.Lengths[level] -= newLength;
                    }
                    Array.Fill(seed.Details, detail - sourceStart + destStart, level + 1, 7 - level);
                    Array.Fill(seed.Lengths, seed.Lengths[level], level + 1, 7 - level);
                }
            }
        }

        [Description("What is the lowest location number that corresponds to any of the initial seed numbers?")]
        public override string SolvePart1() {
            long minLocation = long.MaxValue;
            for (int i = 0; i < seeds1.Count; i++) {
                Seed seed = seeds1[i];
                if (seed.Details[7] < minLocation) {
                    minLocation = seed.Details[7];
                }
            }
            return $"{minLocation}";
        }

        [Description("What is the lowest location number that corresponds to any of the initial seed numbers?")]
        public override string SolvePart2() {
            long minLocation = long.MaxValue;
            for (int i = 0; i < seeds2.Count; i++) {
                Seed seed = seeds2[i];
                if (seed.Details[7] < minLocation) {
                    minLocation = seed.Details[7];
                }
            }
            return $"{minLocation}";
        }

        private class Seed {
            public long[] Details = new long[8];
            public long[] Lengths = new long[8];
            public Seed(long id, long length) {
                Array.Fill(Details, id);
                Array.Fill(Lengths, length);
            }
            public Seed(Seed copy) {
                Array.Copy(copy.Details, Details, 8);
                Array.Copy(copy.Lengths, Lengths, 8);
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 8; i++) {
                    if (Lengths[i] > 1) {
                        sb.Append($"({Details[i]}->{Details[i] + Lengths[i] - 1})->");
                    } else {
                        sb.Append($"{Details[i]}->");
                    }
                }
                sb.Length -= 2;
                return sb.ToString();
            }
        }
    }
}