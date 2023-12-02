using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Trebuchet?!")]
    public class Puzzle01 : ASolver {
        private List<string> calibrations = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                calibrations.Add(line);
            }
        }

        [Description("What is the sum of all of the calibration values?")]
        public override string SolvePart1() {
            return $"{FindTotalDigitSum(false)}";
        }

        [Description("What is the sum of all of the calibration values?")]
        public override string SolvePart2() {
            return $"{FindTotalDigitSum(true)}";
        }
        private int FindTotalDigitSum(bool includeLetters) {
            int total = 0;
            for (int i = 0; i < calibrations.Count; i++) {
                string calibration = calibrations[i];
                DigitWindow window = new DigitWindow();
                for (int j = 0; j < calibration.Length; j++) {
                    char c = calibration[j];
                    int value = window.FindStart(c, includeLetters);
                    if (value > 0) { total += value * 10; break; }
                }
                window = new DigitWindow();
                for (int j = calibration.Length - 1; j >= 0; j--) {
                    char c = calibration[j];
                    int value = window.FindEnd(c, includeLetters);
                    if (value > 0) { total += value; break; }
                }
            }
            return total;
        }
        private struct DigitWindow {
            public char D1, D2, D3, D4, D5;
            public int FindStart(char current, bool includeLetters = false) {
                D5 = D4;
                D4 = D3;
                D3 = D2;
                D2 = D1;
                D1 = current;
                if (char.IsDigit(D1)) { return D1 & 15; }
                if (!includeLetters) { return 0; }

                return (D1, D2, D3, D4, D5) switch {
                    ('e', 'n', 'o', _, _) => 1,
                    ('o', 'w', 't', _, _) => 2,
                    ('e', 'e', 'r', 'h', 't') => 3,
                    ('r', 'u', 'o', 'f', _) => 4,
                    ('e', 'v', 'i', 'f', _) => 5,
                    ('x', 'i', 's', _, _) => 6,
                    ('n', 'e', 'v', 'e', 's') => 7,
                    ('t', 'h', 'g', 'i', 'e') => 8,
                    ('e', 'n', 'i', 'n', _) => 9,
                    _ => 0
                };
            }
            public int FindEnd(char current, bool includeLetters = false) {
                D5 = D4;
                D4 = D3;
                D3 = D2;
                D2 = D1;
                D1 = current;
                if (char.IsDigit(D1)) { return D1 & 15; }
                if (!includeLetters) { return 0; }

                return (D1, D2, D3, D4, D5) switch {
                    ('o', 'n', 'e', _, _) => 1,
                    ('t', 'w', 'o', _, _) => 2,
                    ('t', 'h', 'r', 'e', 'e') => 3,
                    ('f', 'o', 'u', 'r', _) => 4,
                    ('f', 'i', 'v', 'e', _) => 5,
                    ('s', 'i', 'x', _, _) => 6,
                    ('s', 'e', 'v', 'e', 'n') => 7,
                    ('e', 'i', 'g', 'h', 't') => 8,
                    ('n', 'i', 'n', 'e', _) => 9,
                    _ => 0
                };
            }
        }
    }
}