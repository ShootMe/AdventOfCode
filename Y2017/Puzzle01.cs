using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Inverse Captcha")]
    public class Puzzle01 : ASolver {
        [Description("What is the solution to your captcha?")]
        public override string SolvePart1() {
            int sum = GetSum(Input, 1);

            return $"{sum}";
        }

        [Description("What is the solution to your new captcha?")]
        public override string SolvePart2() {
            int distance = Input.Length / 2;
            int sum = GetSum(Input, distance);

            return $"{sum}";
        }

        private int GetSum(string number, int distance) {
            int sum = 0;
            for (int i = 0; i < number.Length; i++) {
                char digit = number[i];
                char nextDigit = number[(i + distance) % number.Length];
                if (digit == nextDigit) {
                    sum += (byte)digit - 0x30;
                }
            }
            return sum;
        }
    }
}