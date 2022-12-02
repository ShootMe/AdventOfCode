using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Inventory Management System")]
    public class Puzzle02 : ASolver {
        private List<string> items;

        public override void Setup() {
            items = Input.Lines();
        }

        [Description("What is the checksum for your list of box IDs?")]
        public override string SolvePart1() {
            int[] counts = new int[26];
            int threes = 0;
            int twos = 0;
            for (int i = 0; i < items.Count; i++) {
                string code = items[i];
                for (int j = 0; j < code.Length; j++) {
                    counts[(byte)char.ToUpper(code[j]) - 0x41]++;
                }

                bool hasThree = false;
                bool hasTwo = false;
                for (int j = 0; j < counts.Length; j++) {
                    int count = counts[j];
                    if (!hasThree && count == 3) {
                        threes++;
                        hasThree = true;
                    } else if (!hasTwo && count == 2) {
                        twos++;
                        hasTwo = true;
                    }
                }

                Array.Clear(counts, 0, counts.Length);
            }
            return $"{twos * threes}";
        }

        [Description("What letters are common between the two correct box IDs?")]
        public override string SolvePart2() {
            int strLength = items[0].Length;
            for (int k = 0; k < strLength; k++) {
                HashSet<string> reduced = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < items.Count; i++) {
                    string code = items[i].Remove(k, 1);

                    if (!reduced.Add(code)) {
                        return code;
                    }
                }
            }
            return string.Empty;
        }
    }
}