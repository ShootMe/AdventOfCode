using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Mirage Maintenance")]
    public class Puzzle09 : ASolver {
        private List<int[]> oasisValues = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                oasisValues.Add(line.ToInts(' '));
            }
        }

        [Description("What is the sum of these extrapolated values?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < oasisValues.Count; i++) {
                int[] values = oasisValues[i];
                bool allZero = false;
                int length = values.Length;
                while (!allZero) {
                    allZero = true;
                    for (int j = 1; j < length; j++) {
                        int newValue = values[j] - values[j - 1];
                        values[j - 1] = newValue;
                        if (newValue != 0) { allZero = false; }
                    }
                    length--;
                }

                for (int j = length + 1; j < values.Length; j++) {
                    values[j] += values[j - 1];
                }
                total += values[^1];
            }
            return $"{total}";
        }

        [Description("What is the sum of these extrapolated values?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < oasisValues.Count; i++) {
                int[] values = oasisValues[i];
                for (int j = 0; j <= values.Length; j++) {
                    for (int k = values.Length - 1; k > 0; k--) {
                        values[k] -= values[k - 1];
                    }
                }

                total += values[^1];
            }
            return $"{total}";
        }
    }
}