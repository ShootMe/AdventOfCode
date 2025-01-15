using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2023 {
    [Description("Point of Incidence")]
    public class Puzzle13 : ASolver {
        private List<Pattern> patterns = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');

            List<string> pattern = new List<string>();
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                if (string.IsNullOrEmpty(line)) {
                    patterns.Add(new Pattern(pattern));
                    pattern.Clear();
                    continue;
                }
                pattern.Add(line);
            }
            patterns.Add(new Pattern(pattern));
        }

        [Description("What number do you get after summarizing all of your notes?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < patterns.Count; i++) {
                total += patterns[i].GetReflectionValue();
            }
            return $"{total}";
        }

        [Description("What number do you get after summarizing the new reflection line in each pattern in your notes?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < patterns.Count; i++) {
                total += patterns[i].Get2ndReflection();
            }
            return $"{total}";
        }

        private class Pattern {
            public int[] RowValues, ColValues;
            public int Width, Height;
            private int RX = -1, RY = -1;
            public Pattern(List<string> lines) {
                Width = lines[0].Length;
                Height = lines.Count;
                RowValues = new int[Height];
                ColValues = new int[Width];

                for (int i = 0; i < lines.Count; i++) {
                    string line = lines[i];
                    int value = 0;
                    for (int j = 0; j < line.Length; j++) {
                        value <<= 1;
                        int bit = line[j] == '#' ? 1 : 0;
                        value |= bit;
                        ColValues[j] = (ColValues[j] << 1) | bit;
                    }
                    RowValues[i] = value;
                }
            }

            public int Get2ndReflection() {
                int value = CheckReflection(ColValues, Width, RX);
                if (value > 0) { return value; }

                value = CheckReflection(RowValues, Height, RY);
                if (value > 0) { return value * 100; }
                
                return 0;
            }
            public int GetReflectionValue() {
                int value = CheckReflection(ColValues, Width);
                if (value > 0) {
                    RX = value - 1;
                    return value;
                }

                value = CheckReflection(RowValues, Height);
                if (value > 0) {
                    RY = value - 1;
                    return value * 100;
                }

                return 0;
            }
            private int CheckReflection(int[] values, int max) {
                for (int i = 0; i < max - 1; i++) {
                    bool found = true;
                    int minIndex = i + i - max + 2;
                    if (minIndex < 0) { minIndex = 0; }

                    for (int k = i, j = i + 1; k >= minIndex; k--, j++) {
                        int diff = values[k] ^ values[j];
                        if (diff != 0) { found = false; break; }
                    }

                    if (found) { return i + 1; }
                }

                return 0;
            }
            private int CheckReflection(int[] values, int max, int ignoreIndex) {
                for (int i = 0; i < max - 1; i++) {
                    if (i == ignoreIndex) { continue; }

                    bool found = true;
                    bool firstDiff = false;
                    int minIndex = i + i - max + 2;
                    if (minIndex < 0) { minIndex = 0; }

                    for (int k = i, j = i + 1; k >= minIndex; k--, j++) {
                        int diff = values[k] ^ values[j];
                        if (diff != 0) {
                            if (!firstDiff && BitOperations.PopCount((uint)diff) == 1) {
                                firstDiff = true;
                                continue;
                            }
                            found = false;
                            break;
                        }
                    }

                    if (found) { return i + 1; }
                }

                return 0;
            }
            public override string ToString() {
                return $"{Width},{Height}";
            }
        }
    }
}