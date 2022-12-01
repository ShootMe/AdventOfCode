using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Chronal Calibration")]
    public class Puzzle01 : ASolver {
        private int[] numbers;

        public override void Setup() {
            numbers = Tools.GetInts(Input);
        }

        [Description("What is the resulting frequency after all of the changes in frequency have been applied?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < numbers.Length; i++) {
                total += numbers[i];
            }
            return $"{total}";
        }

        [Description("What is the first frequency your device reaches twice?")]
        public override string SolvePart2() {
            HashSet<int> reached = new HashSet<int>();
            reached.Add(0);
            int total = 0;
            while (true) {
                for (int i = 0; i < numbers.Length; i++) {
                    total += numbers[i];
                    if (!reached.Add(total)) {
                        return $"{total}";
                    }
                }
            }
        }
    }
}