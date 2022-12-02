using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Report Repair")]
    public class Puzzle01 : ASolver {
        private int[] numbers;

        public override void Setup() {
            numbers = Input.ToInts();
        }

        [Description("What do you get if you multiply them together?")]
        public override string SolvePart1() {
            for (int i = 0; i < numbers.Length; i++) {
                int num1 = numbers[i];

                for (int j = i + 1; j < numbers.Length; j++) {
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

                for (int j = i + 1; j < numbers.Length; j++) {
                    int num2 = numbers[j];

                    for (int k = j + 1; k < numbers.Length; k++) {
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