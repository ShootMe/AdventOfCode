using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("The Treachery of Whales")]
    public class Puzzle07 : ASolver {
        private int[] numbers;
        private int mean, median;

        public override void Setup() {
            numbers = Input.ToInts(',');
            Array.Sort(numbers);

            for (int i = 0; i < numbers.Length; i++) {
                mean += numbers[i];
            }
            mean /= numbers.Length;

            median = numbers[numbers.Length / 2];
            if ((numbers.Length & 1) == 0) {
                median = (median + numbers[numbers.Length / 2 - 1]) / 2;
            }
        }

        [Description("How much fuel must they spend to align to that position?")]
        public override string SolvePart1() {
            return $"{DetermineMinimumFuel(median - 1, median + 1, false)}";
        }

        [Description("How much fuel must they spend to align to that position?")]
        public override string SolvePart2() {
            return $"{DetermineMinimumFuel(mean - 1, mean + 1, true)}";
        }

        private int DetermineMinimumFuel(int minPosition, int maxPosition, bool nonConstantBurn) {
            int minSum = int.MaxValue;
            for (int i = minPosition; i <= maxPosition; i++) {
                int sum = CalculateFuelNeeded(i, nonConstantBurn);

                if (sum < minSum) { minSum = sum; }
            }

            return minSum;
        }
        private int CalculateFuelNeeded(int moveToPosition, bool nonConstantBurn) {
            int sum = 0;
            for (int i = 0; i < numbers.Length; i++) {
                int crabPosition = numbers[i];

                int distance = Math.Abs(moveToPosition - crabPosition);
                if (nonConstantBurn) {
                    sum += distance * (distance + 1) / 2;
                } else {
                    sum += distance;
                }
            }
            return sum;
        }
    }
}