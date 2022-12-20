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
            WrappingList<(long num, int index)> list = new(numbers.Count);
            for (int i = 0; i < numbers.Count; i++) {
                long number = numbers[i];
                list.AddBefore((number, i));
            }

            Mix(list);

            return $"{FindCoordinates(list)}";
        }

        [Description("What is the sum of the three numbers that form the grove coordinates?")]
        public override string SolvePart2() {
            WrappingList<(long num, int index)> list = new(numbers.Count);
            for (int i = 0; i < numbers.Count; i++) {
                long number = numbers[i] * 811589153;
                numbers[i] = number;
                list.AddBefore((number, i));
            }

            for (int j = 0; j < 10; j++) {
                Mix(list);
            }

            return $"{FindCoordinates(list)}";
        }
        private void Mix(WrappingList<(long num, int index)> list) {
            for (int i = 0; i < numbers.Count; i++) {
                while (list.Current.index != i) {
                    list.IncreasePosition();
                }

                long number = numbers[i];
                list.InsertCurrent((int)(number % (numbers.Count - 1)));
            }
        }
        private long FindCoordinates(WrappingList<(long num, int index)> list) {
            while (list.Current.num != 0) {
                list.IncreasePosition();
            }

            list.IncreasePosition(1000);
            long coordinates = list.Current.num;
            list.IncreasePosition(1000);
            coordinates += list.Current.num;
            list.IncreasePosition(1000);
            coordinates += list.Current.num;
            return coordinates;
        }
    }
}