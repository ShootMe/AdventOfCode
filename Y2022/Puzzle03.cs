using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Rucksack Reorganization")]
    public class Puzzle03 : ASolver {
        [Description("What is the sum of the priorities of those item types?")]
        public override string SolvePart1() {
            int total = 0;
            foreach (string line in Input.Split('\n')) {
                int halfway = line.Length / 2;
                HashSet<char> items = new();
                for (int i = 0; i < halfway; i++) {
                    items.Add(line[i]);
                }
                for (int i = halfway; i < line.Length; i++) {
                    if (items.Contains(line[i])) {
                        total += line[i] - (line[i] >= 'a' ? 'a' - 1 : 'A' - 27);
                        break;
                    }
                }
            }
            return $"{total}";
        }

        [Description("What is the sum of the priorities of those item types?")]
        public override string SolvePart2() {
            int total = 0;
            string[] lines = Input.Split('\n');
            for (int i = 0; i < lines.Length; i += 3) {
                HashSet<char> items = new(lines[i]);
                HashSet<char> other = new(lines[i + 1]);
                items.IntersectWith(other);
                other = new(lines[i + 2]);
                items.IntersectWith(other);
                foreach (char c in items) {
                    total += c - (c >= 'a' ? 'a' - 1 : 'A' - 27);
                }
            }
            return $"{total}";
        }
    }
}