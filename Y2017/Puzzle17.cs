using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Spinlock")]
    public class Puzzle17 : ASolver {
        [Description("What is the value after 2017 in your completed circular buffer?")]
        public override string SolvePart1() {
            int moveAmount = Input.ToInt();
            WrappingList<int> list = new WrappingList<int>(2018);
            list.AddAfter(0);
            for (int i = 1; i <= 2017; i++) {
                list.IncreasePosition(moveAmount);
                list.AddAfter(i, true);
            }
            return $"{list.Next}";
        }

        [Description("What is the value after 0 the moment 50000000 is inserted?")]
        public override string SolvePart2() {
            int moveAmount = Input.ToInt();
            int sum = 0;
            int lastValue = 0;
            for (int i = 1; i < 50000000; i++) {
                sum = (sum + moveAmount + 1) % i;
                if (sum == 0) {
                    lastValue = i;
                }
            }
            return $"{lastValue}";
        }
    }
}