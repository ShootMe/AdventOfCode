using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Grove Positioning System")]
    public class Puzzle20 : ASolver {
        private List<long> numbers = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                numbers.Add(line.ToInt());
            }
        }

        [Description("What is the sum of the three numbers that form the grove coordinates?")]
        public override string SolvePart1() {
            return $"{DecryptCoordinates()}";
        }

        [Description("What is the sum of the three numbers that form the grove coordinates?")]
        public override string SolvePart2() {
            for (int i = 0; i < numbers.Count; i++) {
                numbers[i] *= 811589153;
            }

            return $"{DecryptCoordinates(10)}";
        }

        private long DecryptCoordinates(int times = 1) {
            List<int> indexes = new(numbers.Count);
            for (int i = 0; i < numbers.Count; i++) { indexes.Add(i); }
            int len = numbers.Count - 1;

            for (int i = 0; i < times; i++) {
                for (int j = 0; j < numbers.Count; j++) {
                    int from = indexes.IndexOf(j);
                    indexes.RemoveAt(from);
                    int num = (int)((from + numbers[j]) % len);
                    int to = num < 0 ? num + len : num;
                    indexes.Insert(to, j);
                }
            }
            int position = indexes.IndexOf(numbers.IndexOf(0)) + 1000;
            return numbers[indexes[position % numbers.Count]] +
                numbers[indexes[(position + 1000) % numbers.Count]] +
                numbers[indexes[(position + 2000) % numbers.Count]];
        }
    }
}