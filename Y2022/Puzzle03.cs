using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Rucksack Reorganization")]
    public class Puzzle03 : ASolver {
        [Description("What is the sum of the priorities of those item types?")]
        public override string SolvePart1() {
            int total = 0;
            byte[] counts = new byte[53];
            foreach (string line in Input.Split('\n')) {
                int halfway = line.Length / 2;
                for (int i = 0; i < halfway; i++) {
                    counts[ItemValue(line[i])] = 1;
                }

                for (int i = halfway; i < line.Length; i++) {
                    int value = ItemValue(line[i]);
                    if (counts[value] != 0) {
                        total += value;
                        break;
                    }
                }
                Array.Clear(counts);
            }
            return $"{total}";
        }

        [Description("What is the sum of the priorities of those item types?")]
        public override string SolvePart2() {
            int total = 0;
            byte[] counts = new byte[53];
            string[] lines = Input.Split('\n');
            for (int i = 0; i < lines.Length; i += 3) {
                string line = lines[i];
                for (int j = 0; j < line.Length; j++) {
                    counts[ItemValue(line[j])] = 1;
                }

                line = lines[i + 1];
                for (int j = 0; j < line.Length; j++) {
                    counts[ItemValue(line[j])] |= 2;
                }

                line = lines[i + 2];
                for (int j = 0; j < line.Length; j++) {
                    int value = ItemValue(line[j]);
                    if (counts[value] == 3) {
                        total += value;
                        break;
                    }
                }
                Array.Clear(counts);
            }
            return $"{total}";
        }

        private int ItemValue(char c) {
            return (c & 0x1f) + (c >= 'a' ? 0 : 26);
        }
    }
}