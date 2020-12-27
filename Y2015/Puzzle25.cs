using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle25 : ASolver {
        private int row, column;
        public Puzzle25(string input) : base(input) { Name = "Let It Snow"; }

        public override void Setup() {
            row = Tools.ParseInt(Input, "at row ", ",");
            column = Tools.ParseInt(Input, "column ", ".");
        }

        [Description("What code do you give the machine?")]
        public override string SolvePart1() {
            int step = 1;
            int current = 0;
            int rowNum = 1;
            int colNum = 1;
            long value = 20151125;
            while (true) {
                current++;
                if (rowNum == row && colNum == column) {
                    return $"{value}";
                }

                if (rowNum == 1) {
                    step++;
                    rowNum = step;
                    colNum = 1;
                } else {
                    rowNum--;
                    colNum++;
                }

                value = (value * 252533) % 33554393;
            }
        }
    }
}