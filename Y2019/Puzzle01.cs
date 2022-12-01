using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("The Tyranny of the Rocket Equation")]
    public class Puzzle01 : ASolver {
        private int[] numbers;

        public override void Setup() {
            numbers = Tools.GetInts(Input);
        }

        [Description("What is the sum of the fuel requirements for all of the modules on your spacecraft?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < numbers.Length; i++) {
                total += numbers[i] / 3 - 2;
            }

            return $"{total}";
        }

        [Description("What is the sum of the fuel when also taking into account the mass of the added fuel?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < numbers.Length; i++) {
                int fuel = numbers[i] / 3 - 2;
                while (fuel > 0) {
                    total += fuel;
                    fuel = fuel / 3 - 2;
                }
            }

            return $"{total}";
        }
    }
}