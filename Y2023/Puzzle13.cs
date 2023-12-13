using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Description("Find the line of reflection in each of the patterns in your notes. What number do you get after summarizing all of your notes?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < patterns.Count; i++) {
                Pattern pattern = patterns[i];
                total += pattern.GetReflectionValue();
            }
            return $"{total}";
        }

        [Description("In each pattern, fix the smudge and find the different line of reflection. What number do you get after summarizing the new reflection line in each pattern in your notes?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < patterns.Count; i++) {
                Pattern pattern = patterns[i];
                total += pattern.Get2ndReflection();
            }
            return $"{total}";
        }

        private class Pattern {
            public bool[] Values;
            public int Width, Height;
            private int RX = -1, RY = -1;
            public Pattern(List<string> lines) {
                Width = lines[0].Length;
                Height = lines.Count;
                Values = new bool[lines.Count * lines[0].Length];
                int index = 0;
                for (int i = 0; i < lines.Count; i++) {
                    string line = lines[i];
                    for (int j = 0; j < line.Length; j++) {
                        Values[index++] = line[j] == '#';
                    }
                }
            }

            public int Get2ndReflection() {
                for (int i = 0; i < Values.Length; i++) {
                    Values[i] = !Values[i];
                    int value = GetReflectionValue();
                    if(value != 0) {
                        return value;
                    }
                    Values[i] = !Values[i];
                }
                return 0;
            }
            public int GetReflectionValue() {
                for (int i = 0; i < Width - 1; i++) {
                    if (i == RX) { continue; }

                    bool found = true;
                    int minIndex = i + i - Width + 2;
                    if (minIndex < 0) { minIndex = 0; }
                    for (int k = i; k >= minIndex && found; k--) {
                        for (int j = 0; j < Values.Length; j += Width) {
                            if (Values[j + k] != Values[j + i + i - k + 1]) {
                                found = false;
                                break;
                            }
                        }
                    }
                    if (found) {
                        RX = i;
                        return i + 1;
                    }
                }

                for (int i = 0; i < Height - 1; i++) {
                    if (i == RY) { continue; }

                    bool found = true;
                    int minIndex = i - Height + i + 2;
                    if (minIndex < 0) { minIndex = 0; }
                    for (int k = i; k >= minIndex && found; k--) {
                        for (int j = 0; j < Width; j++) {
                            if (Values[k * Width + j] != Values[(i + i - k + 1) * Width + j]) {
                                found = false;
                                break;
                            }
                        }
                    }
                    if (found) {
                        RY = i;
                        return (i + 1) * 100;
                    }
                }

                return 0;
            }
            public override string ToString() {
                return $"{Width},{Height}";
            }
        }
    }
}