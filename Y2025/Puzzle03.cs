using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Lobby")]
    public class Puzzle03 : ASolver {
        private List<string> banks = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                banks.Add(line);
            }
        }

        [Description("What is the total output joltage?")]
        public override string SolvePart1() {
            return $"{SumJoltage(2)}";
        }

        [Description("What is the new total output joltage?")]
        public override string SolvePart2() {
            return $"{SumJoltage(12)}";
        }

        public long SumJoltage(int digits) {
            long total = 0;
            for (int i = 0; i < banks.Count; i++) {
                string batteries = banks[i];
                long joltage = Joltage(batteries, digits);
                total += joltage;
            }
            return total;
        }
        public long Joltage(string batteries, int digits) {
            int end = batteries.Length - digits + 1;
            char[] maxVals = new char[digits];

            for (int i = 0; i < batteries.Length; i++) {
                char c = batteries[i];
                for (int j = i >= end ? i - end + 1 : 0; j < digits; j++) {
                    if (c > maxVals[j]) {
                        maxVals[j] = c;
                        if (j + 1 < digits) { maxVals[j + 1] = '\0'; }
                        break;
                    }
                }
            }
            return new string(maxVals).ToLong();
        }
    }
}