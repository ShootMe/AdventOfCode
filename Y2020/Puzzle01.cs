using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    public class Puzzle01 : ASolver {
        private int[] numbers;
        public Puzzle01(string input) : base(input) { Name = "Report Repair"; }

        public override void Setup() {
            numbers = Tools.GetNumbers(Input);
        }

        [Description("What do you get if you multiply them together?")]
        public override string SolvePart1() {
            for (int i = 0; i < numbers.Length; i++) {
                int num1 = numbers[i];

                for (int j = 0; j < numbers.Length; j++) {
                    if (i == j) { continue; }

                    int num2 = numbers[j];
                    if (num1 + num2 == 2020) {
                        return $"{num1 * num2}";
                    }
                }
            }

            return string.Empty;
        }

        [Description("What is the product of the three entries that sum to 2020?")]
        public override string SolvePart2() {
            for (int i = 0; i < numbers.Length; i++) {
                int num1 = numbers[i];

                for (int j = 0; j < numbers.Length; j++) {
                    if (i == j) { continue; }

                    int num2 = numbers[j];

                    for (int k = 0; k < numbers.Length; k++) {
                        if (k == j || k == i) { continue; }

                        int num3 = numbers[k];
                        if (num1 + num2 + num3 == 2020) {
                            return $"{num1 * num2 * num3}";
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}