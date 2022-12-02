using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Toboggan Trajectory")]
    public class Puzzle03 : ASolver {
        private bool[] grid;
        private int width, height;

        public override void Setup() {
            List<string> items = Input.Lines();
            width = items[0].Length;
            height = items.Count;
            grid = new bool[width * height];
            for (int i = 0; i < items.Count; i++) {
                string row = items[i];
                for (int j = 0; j < row.Length; j++) {
                    char c = row[j];
                    grid[i * width + j] = c != '.';
                }
            }
        }

        [Description("How many trees would you encounter?")]
        public override string SolvePart1() {
            int count = CountTrees(grid, width, height, 3, 1);
            return $"{count}";
        }

        [Description("What do you get if you multiply together the number of trees encountered on each of the slopes?")]
        public override string SolvePart2() {
            int count1 = CountTrees(grid, width, height, 1, 1);
            int count2 = CountTrees(grid, width, height, 3, 1);
            int count3 = CountTrees(grid, width, height, 5, 1);
            int count4 = CountTrees(grid, width, height, 7, 1);
            int count5 = CountTrees(grid, width, height, 1, 2);
            return $"{(long)count1 * count2 * count3 * count4 * count5}";
        }

        private int CountTrees(bool[] grid, int width, int height, int slopeR, int slopeD) {
            int x = 0;
            int y = 0;
            int count = 0;
            while (y < height) {
                if (grid[y * width + x]) {
                    count++;
                }
                x += slopeR;
                y += slopeD;
                if (x >= width) { x -= width; }
            }
            return count;
        }
    }
}