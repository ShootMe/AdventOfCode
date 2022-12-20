using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Grove Positioning System")]
    public class Puzzle20 : ASolver {
        private List<int> numbers = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                numbers.Add(line.ToInt());
            }
        }

        [Description("What is the sum of the three numbers that form the grove coordinates?")]
        public override string SolvePart1() {
            WrappingList<(int num, int index)> list = new(numbers.Count);
            for (int i = 0; i < numbers.Count; i++) {
                int number = numbers[i];
                list.AddBefore((number, i));
            }

            for (int i = 0; i < numbers.Count; i++) {
                int number = numbers[i];
                if (number > 0) {
                    (int num, int index) = list.Remove();
                    list.IncreasePosition(num - 1);
                    list.AddAfter((num, index));
                } else if (number < 0) {
                    (int num, int index) = list.Remove();
                    list.DecreasePosition(-num);
                    list.AddBefore((num, index));
                }

                while (i + 1 < numbers.Count && list.Current.index != i + 1) {
                    list.IncreasePosition();
                }
            }

            while (list.Current.num != 0) {
                list.IncreasePosition();
            }

            list.IncreasePosition(1000);
            int coordinates = list.Current.num;
            list.IncreasePosition(1000);
            coordinates += list.Current.num;
            list.IncreasePosition(1000);
            coordinates += list.Current.num;
            return $"{coordinates}";
        }
        
        [Description("What is the sum of the three numbers that form the grove coordinates?")]
        public override string SolvePart2() {
            WrappingList<(long num, int index)> list = new(numbers.Count);
            for (int i = 0; i < numbers.Count; i++) {
                int number = numbers[i];
                list.AddBefore(((long)number * 811589153, i));
            }

            for (int j = 0; j < 10; j++) {
                while (list.Current.index != 0) {
                    list.DecreasePosition();
                }

                for (int i = 0; i < numbers.Count; i++) {
                    int number = numbers[i];
                    if (number > 0) {
                        (long num, int index) = list.Remove();
                        list.IncreasePosition((int)((num - 1) % list.Count));
                        list.AddAfter((num, index));
                    } else if (number < 0) {
                        (long num, int index) = list.Remove();
                        list.DecreasePosition((int)(-num % list.Count));
                        list.AddBefore((num, index));
                    }
                    while (i + 1 < numbers.Count && list.Current.index != i + 1) {
                        list.IncreasePosition();
                    }
                }
            }

            while (list.Current.num != 0) {
                list.DecreasePosition();
            }

            list.IncreasePosition(1000);
            long coordinates = list.Current.num;
            list.IncreasePosition(1000);
            coordinates += list.Current.num;
            list.IncreasePosition(1000);
            coordinates += list.Current.num;
            return $"{coordinates}";
        }
    }
}