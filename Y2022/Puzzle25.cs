using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2022 {
    [Description("Full of Hot Air")]
    public class Puzzle25 : ASolver {
        private List<long> numbers = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                long amount = 0;
                for (int i = 0; i < line.Length; i++) {
                    char c = line[i];
                    int digit = c switch { '2' => 2, '1' => 1, '-' => -1, '=' => -2, _ => 0 };
                    amount = amount * 5 + digit;
                }
                numbers.Add(amount);
            }
        }

        [Description("The Elves are starting to get cold. What SNAFU number do you supply to Bob's console?")]
        public override string SolvePart1() {
            long total = 0;
            for (int i = 0; i < numbers.Count; i++) {
                total += numbers[i];
            }

            string characters = "012=-";
            StringBuilder result = new();
            while (total > 0) {
                int rem = (int)(total % 5);
                result.Insert(0, characters[rem]);
                total = (total + rem) / 5;
            }
            return $"{result}";
        }
    }
}