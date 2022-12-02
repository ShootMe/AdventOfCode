using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Encoding Error")]
    public class Puzzle09 : ASolver {
        private const int RollingAmount = 25;
        private int[] numbers;

        public override void Setup() {
            numbers = Input.ToInts();
        }

        [Description("What is the first number that does not have this property?")]
        public override string SolvePart1() {
            int[] rolling25 = new int[RollingAmount];
            int rollingIndex = 0;
            Array.Copy(numbers, rolling25, RollingAmount);

            for (int i = RollingAmount; i < numbers.Length; i++) {
                int number = numbers[i];
                if (!CanSum(rolling25, number)) {
                    return $"{number}";
                }
                rolling25[rollingIndex++] = number;
                if (rollingIndex >= RollingAmount) {
                    rollingIndex = 0;
                }
            }

            return string.Empty;
        }

        [Description("What is the encryption weakness in your XMAS-encrypted list of numbers?")]
        public override string SolvePart2() {
            int[] rolling25 = new int[RollingAmount];
            int rollingIndex = 0;
            Array.Copy(numbers, rolling25, RollingAmount);

            for (int i = RollingAmount; i < numbers.Length; i++) {
                int number = numbers[i];
                if (!CanSum(rolling25, number)) {
                    int sum = 0;
                    int start = 0;
                    int end = 0;
                    while (start < numbers.Length && end < numbers.Length) {
                        while (end < numbers.Length && sum < number) {
                            sum += numbers[end++];
                        }
                        if (sum > number) {
                            sum -= numbers[--end];
                        }

                        if (sum == number) {
                            int smallest = int.MaxValue;
                            int largest = int.MinValue;
                            for (int j = start; j < end; j++) {
                                if (numbers[j] < smallest) {
                                    smallest = numbers[j];
                                }
                                if (numbers[j] > largest) {
                                    largest = numbers[j];
                                }
                            }
                            return $"{smallest + largest}";
                        }

                        sum -= numbers[start++];
                    }
                    break;
                }

                rolling25[rollingIndex++] = number;
                if (rollingIndex >= RollingAmount) {
                    rollingIndex = 0;
                }
            }

            return string.Empty;
        }

        private bool CanSum(int[] rolling25, int number) {
            for (int i = 0; i < RollingAmount; i++) {
                int val1 = rolling25[i];

                for (int j = 0; j < RollingAmount; j++) {
                    int val2 = rolling25[j];

                    if (i == j || val1 == val2) { continue; }

                    if (val1 + val2 == number) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}