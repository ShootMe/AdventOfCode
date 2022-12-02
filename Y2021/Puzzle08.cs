using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2021 {
    [Description("Seven Segment Search")]
    public class Puzzle08 : ASolver {
        private List<string[]> segments;

        public override void Setup() {
            List<string> items = Input.Lines();
            segments = new List<string[]>();
            for (int i = 0; i < items.Count; i++) {
                string[] codes = items[i].SplitOn(" ", " ", " ", " ", " ", " ", " ", " ", " ", " | ", " ", " ", " ");
                segments.Add(codes);
            }
        }

        [Description("In the output values, how many times do digits 1, 4, 7, or 8 appear?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < segments.Count; i++) {
                string[] codes = segments[i];
                for (int j = 10; j < 14; j++) {
                    switch (codes[j].Length) {
                        case 2:
                        case 3:
                        case 4:
                        case 7: count++; break;
                    }
                }
            }
            return $"{count}";
        }

        [Description("What do you get if you add up all of the output values?")]
        public override string SolvePart2() {
            int sum = 0;
            Dictionary<int, int> mapping = new Dictionary<int, int>();
            List<(int, int)> remaining = new List<(int, int)>();

            for (int i = 0; i < segments.Count; i++) {
                string[] codes = segments[i];

                mapping.Clear(); remaining.Clear();
                int one = 0, four = 0;
                for (int j = 0; j < 10; j++) {
                    (int code, int length) = GetCodeValue(codes[j]);
                    switch (length) {
                        case 2: mapping[code] = 1; one = code; break;
                        case 3: mapping[code] = 7; break;
                        case 4: mapping[code] = 4; four = code; break;
                        case 7: mapping[code] = 8; break;
                        case 5:
                        case 6: remaining.Add((code, length)); break;
                    }
                }

                for (int j = remaining.Count - 1; j >= 0; j--) {
                    (int code, int length) = remaining[j];
                    int countFour = CountOccurrences(four, code);
                    int countOne = CountOccurrences(one, code);
                    switch (length) {
                        case 5: mapping[code] = countFour == 2 ? 2 : (countOne == 2 ? 3 : 5); break;
                        case 6: mapping[code] = countFour == 4 ? 9 : (countOne == 1 ? 6 : 0); break;
                    }
                }

                int value = 0;
                for (int j = 10; j < 14; j++) {
                    (int code, _) = GetCodeValue(codes[j]);
                    value = value * 10 + mapping[code];
                }
                sum += value;
            }

            return $"{sum}";
        }
        private (int, int) GetCodeValue(string code) {
            int value = 0;
            for (int i = 0; i < code.Length; i++) {
                value |= 1 << (code[i] - 'a');
            }
            return (value, code.Length);
        }
        private int CountOccurrences(int left, int right) {
            return BitOperations.PopCount((uint)(left & right));
        }
    }
}