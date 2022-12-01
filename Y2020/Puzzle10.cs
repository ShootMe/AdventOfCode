using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Adapter Array")]
    public class Puzzle10 : ASolver {
        private int[] numbers;

        public override void Setup() {
            numbers = Tools.GetInts(Input);
            Array.Sort(numbers);
            int[] newNumbers = new int[numbers.Length + 2];
            for (int i = 0; i < numbers.Length; i++) {
                newNumbers[i + 1] = numbers[i];
            }
            newNumbers[newNumbers.Length - 1] = numbers[numbers.Length - 1] + 3;
            numbers = newNumbers;
        }

        [Description("What is the number of 1-jolt differences multiplied by the number of 3-jolt differences?")]
        public override string SolvePart1() {
            int oneDiff = 0;
            int twoDiff = 0;
            int threeDiff = 0;
            int last = 0;
            for (int i = 1; i < numbers.Length; i++) {
                int num = numbers[i];
                int diff = num - last;
                switch (diff) {
                    case 1: oneDiff++; break;
                    case 2: twoDiff++; break;
                    case 3: threeDiff++; break;
                }
                last = num;
            }
            return $"{oneDiff * threeDiff}";
        }

        [Description("What is the number of distinct ways you can arrange the adapters to your device?")]
        public override string SolvePart2() {
            long[] counts = new long[numbers.Length + 1];
            long count = Count(counts, 0);
            return $"{count}";
        }

        private long Count(long[] counts, int index) {
            long count = 0;
            for (int i = index + 1; i < numbers.Length; i++) {
                if (numbers[i] - numbers[index] <= 3) {
                    if (counts[i] > 0) {
                        count += counts[i];
                    } else {
                        long subCount = Count(counts, i);
                        count += subCount;
                        counts[i] = subCount;
                    }
                } else {
                    break;
                }
            }
            return count == 0 ? 1 : count;
        }
    }
}