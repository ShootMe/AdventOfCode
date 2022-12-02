using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Sonar Sweep")]
    public class Puzzle01 : ASolver {
        private List<int> depths;
        public override void Setup() {
            List<string> items = Input.Lines();
            depths = new List<int>();
            for (int i = 0; i < items.Count; i++) {
                depths.Add(items[i].ToInt());
            }
        }

        [Description("How many measurements are larger than the previous measurement?")]
        public override string SolvePart1() {
            int last = int.MaxValue;
            int total = 0;
            for (int i = 0; i < depths.Count; i++) {
                int value = depths[i];
                if (value > last) {
                    total++;
                }
                last = value;
            }
            return $"{total}";
        }

        [Description("How many sums are larger than the previous sum?")]
        public override string SolvePart2() {
            int total = 0;
            int sum = depths[0] + depths[1] + depths[3];
            int last = sum;
            for (int i = 3; i < depths.Count; i++) {
                sum += depths[i] - depths[i - 3];

                if (sum > last) {
                    total++;
                }

                last = sum;
            }
            return $"{total}";
        }
    }
}