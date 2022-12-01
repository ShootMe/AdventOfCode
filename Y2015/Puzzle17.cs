using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("No Such Thing as Too Much")]
    public class Puzzle17 : ASolver {
        private int[] numbers;

        public override void Setup() {
            numbers = Tools.GetInts(Input);
        }

        [Description("How many different combinations can exactly fit all 150 liters of eggnog?")]
        public override string SolvePart1() {
            return $"{CountCombinations(0, 0)}";
        }

        [Description("How many different ways can you fill the minimum number of containers with 150 litres?")]
        public override string SolvePart2() {
            int minCount = FindMinContainers(0, 0, 0, int.MaxValue);
            return $"{CountCombinations(0, 0, 0, minCount)}";
        }

        private int FindMinContainers(int index, int sum, int count, int min) {
            count++;
            for (int i = index; i < numbers.Length; i++) {
                int number = numbers[i];
                int newSum = number + sum;
                if (newSum == 150) {
                    if (count < min) {
                        min = count;
                    }
                } else if (newSum < 150 && count < min) {
                    int newMin = FindMinContainers(i + 1, newSum, count, min);
                    if (newMin < min) {
                        min = newMin;
                    }
                }
            }
            return min;
        }
        private int CountCombinations(int index, int sum, int containers = 0, int limit = int.MaxValue) {
            int count = 0;
            containers++;
            for (int i = index; i < numbers.Length; i++) {
                int number = numbers[i];
                int newSum = number + sum;
                if (newSum == 150) {
                    count++;
                } else if (newSum < 150 && containers < limit) {
                    count += CountCombinations(i + 1, newSum, containers, limit);
                }
            }
            return count;
        }
    }
}