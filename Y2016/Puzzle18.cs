using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Like a Rogue")]
    public class Puzzle18 : ASolver {
        [Description("How many safe tiles are there?")]
        public override string SolvePart1() {
            return $"{SumSafeTiles(40)}";
        }

        [Description("How many safe tiles are there in a total of 400000 rows?")]
        public override string SolvePart2() {
            return $"{SumSafeTiles(400000)}";
        }

        private int SumSafeTiles(int rows) {
            bool[] nextRow = new bool[Input.Length];
            bool[] row = new bool[Input.Length];
            int safeTiles = 0;
            for (int j = 0; j < row.Length; j++) {
                bool safe = Input[j] == '.';
                row[j] = safe;
                if (safe) {
                    safeTiles++;
                }
            }

            for (int i = 1; i < rows; i++) {
                bool right = true;
                bool current = row[row.Length - 1];

                for (int j = row.Length - 1; j >= 0; j--) {
                    bool left = j > 0 ? row[j - 1] : true;

                    if (left ^ right) {
                        nextRow[j] = false;
                    } else {
                        nextRow[j] = true;
                        safeTiles++;
                    }

                    right = current;
                    current = left;
                }

                bool[] temp = row;
                row = nextRow;
                nextRow = temp;
            }

            return safeTiles;
        }
    }
}