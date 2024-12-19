using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Linen Layout")]
    public class Puzzle19 : ASolver {
        private string[] patterns;
        private string[] designs;
        private Dictionary<string, long> counts = new();

        public override void Setup() {
            designs = Input.Split('\n');
            patterns = designs[0].Split(", ");
            Array.Sort(patterns, (left, right) => {
                int comp = left.Length.CompareTo(right.Length);
                if (comp != 0) { return comp; }
                return left.CompareTo(right);
            });
        }

        [Description("How many designs are possible?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 2; i < designs.Length; i++) {
                string design = designs[i];
                total += IsPossible(design) > 0 ? 1 : 0;
            }
            return $"{total}";
        }

        [Description("What do you get if you add up the number of different ways you could make each design?")]
        public override string SolvePart2() {
            long total = 0;
            for (int i = 2; i < designs.Length; i++) {
                string design = designs[i];
                total += IsPossible(design);
            }
            return $"{total}";
        }

        private long IsPossible(string design) {
            if (design.Length == 0) { return 1; }
            if (counts.TryGetValue(design, out long count)) { return count; }
            count = 0;

            for (int i = 0; i < patterns.Length; i++) {
                string pattern = patterns[i];
                int index = design.IndexOf(pattern);
                if (index == 0) {
                    count += IsPossible(design.Remove(index, pattern.Length));
                }
            }

            counts.Add(design, count);
            return count;
        }
    }
}