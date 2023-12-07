using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("If You Give A Seed A Fertilizer")]
    public class Puzzle05 : ASolver {
        private List<Seed> seeds1, seeds2;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            ParseSeeds(lines[0]);

            int lineIndex = 3;
            for (int level = 0; level < 7; level++) {
                MapDetails(lines, ref lineIndex, level);
            }
        }
        private void ParseSeeds(string line) {
            long[] numbers = line.Substring(7).ToLongs(' ');

            seeds1 = new List<Seed>(numbers.Length);
            for (int i = 0; i < numbers.Length; i++) {
                seeds1.Add(new Seed(0, numbers[i], 1));
            }

            seeds2 = new List<Seed>(numbers.Length / 2);
            for (int i = 0; i < numbers.Length; i += 2) {
                seeds2.Add(new Seed(0, numbers[i], numbers[i + 1]));
            }
        }
        private void MapDetails(string[] lines, ref int lineIndex, int level) {
            for (; lineIndex < lines.Length; lineIndex++) {
                string line = lines[lineIndex];
                if (string.IsNullOrEmpty(line)) { lineIndex += 2; break; }

                string[] mapping = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                MapDetails(seeds1, level, mapping[0].ToLong(), mapping[1].ToLong(), mapping[2].ToLong());
                MapDetails(seeds2, level, mapping[0].ToLong(), mapping[1].ToLong(), mapping[2].ToLong());
            }
        }
        private void MapDetails(List<Seed> seedsToMap, int level, long destStart, long sourceStart, long length) {
            for (int i = 0; i < seedsToMap.Count; i++) {
                Seed seed = seedsToMap[i];
                //Check intersection
                if (seed.Level > level || seed.Value >= sourceStart + length || seed.Value + seed.Length <= sourceStart) { continue; }

                //If seed range extends beyond mapping range
                if (seed.Value + seed.Length > sourceStart + length) {
                    long newLength = seed.Value + seed.Length - sourceStart - length;
                    Seed newSeed = new Seed(level, sourceStart + length, newLength);
                    seedsToMap.Add(newSeed);
                    seed.Length -= newLength;
                }

                //If seed range extends before mapping range
                if (seed.Value < sourceStart) {
                    long newLength = sourceStart - seed.Value;
                    Seed newSeed = new Seed(level, seed.Value, newLength);
                    seedsToMap.Add(newSeed);
                    seed.Value += newLength;
                    seed.Length -= newLength;
                }

                //Change seeds value to new mapping value
                seed.Value += destStart - sourceStart;
                seed.Level = level + 1;
            }
        }

        [Description("What is the lowest location number that corresponds to any of the initial seed numbers?")]
        public override string SolvePart1() {
            long minLocation = long.MaxValue;
            for (int i = 0; i < seeds1.Count; i++) {
                Seed seed = seeds1[i];
                if (seed.Value < minLocation) {
                    minLocation = seed.Value;
                }
            }
            return $"{minLocation}";
        }

        [Description("What is the lowest location number that corresponds to any of the initial seed numbers?")]
        public override string SolvePart2() {
            long minLocation = long.MaxValue;
            for (int i = 0; i < seeds2.Count; i++) {
                Seed seed = seeds2[i];
                if (seed.Value < minLocation) {
                    minLocation = seed.Value;
                }
            }
            return $"{minLocation}";
        }

        private class Seed {
            public int Level;
            public long Value;
            public long Length;
            public Seed(int level, long id, long length) {
                Level = level;
                Value = id;
                Length = length;
            }
            public override string ToString() {
                return $"{Value}->{Value - 1 + Length}";
            }
        }
    }
}