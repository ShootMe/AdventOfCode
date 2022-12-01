using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Spiral Memory")]
    public class Puzzle03 : ASolver {
        [Description("How many steps are required to carry the data to the access port?")]
        public override string SolvePart1() {
            int number = Tools.ParseInt(Input);
            int x = 0;
            int y = 0;
            int pos = 1;
            int width = 0;
            int height = 0;
            int traveled = 0;
            int dir = 1;
            while (pos < number) {
                pos++;
                traveled++;
                switch (dir) {
                    case 0:
                        y--;
                        if (traveled > height) {
                            traveled = 0;
                            height++;
                            dir--;
                        }
                        break;
                    case 1:
                        x++;
                        if (traveled > width) {
                            traveled = 0;
                            width++;
                            dir--;
                        }
                        break;
                    case 2:
                        y++;
                        if (traveled > height) {
                            traveled = 0;
                            height++;
                            dir--;
                        }
                        break;
                    case 3:
                        x--;
                        if (traveled > width) {
                            traveled = 0;
                            width++;
                            dir--;
                        }
                        break;
                }

                if (dir < 0) {
                    dir = 3;
                }
            }
            return $"{Math.Abs(x) + Math.Abs(y)}";
        }

        [Description("What is the first value written that is larger than your puzzle input?")]
        public override string SolvePart2() {
            int[] grid = new int[256];
            int number = Tools.ParseInt(Input);
            int x = 8;
            int y = 8;
            grid[y * 16 + x] = 1;

            int pos = 1;
            int width = 0;
            int height = 0;
            int traveled = 0;
            int dir = 1;
            while (true) {
                pos++;
                traveled++;

                switch (dir) {
                    case 0:
                        y--;
                        if (traveled > height) {
                            traveled = 0;
                            height++;
                            dir--;
                        }
                        break;
                    case 1:
                        x++;
                        if (traveled > width) {
                            traveled = 0;
                            width++;
                            dir--;
                        }
                        break;
                    case 2:
                        y++;
                        if (traveled > height) {
                            traveled = 0;
                            height++;
                            dir--;
                        }
                        break;
                    case 3:
                        x--;
                        if (traveled > width) {
                            traveled = 0;
                            width++;
                            dir--;
                        }
                        break;
                }

                if (dir < 0) {
                    dir = 3;
                }

                int sum = SumAndSet(grid, x, y);
                if (sum > number) {
                    return $"{sum}";
                }
            }
        }

        private int SumAndSet(int[] grid, int x, int y) {
            int sum = grid[y * 16 + x - 1];
            sum += grid[y * 16 + x + 1];
            sum += grid[(y - 1) * 16 + x - 1];
            sum += grid[(y - 1) * 16 + x];
            sum += grid[(y - 1) * 16 + x + 1];
            sum += grid[(y + 1) * 16 + x - 1];
            sum += grid[(y + 1) * 16 + x];
            sum += grid[(y + 1) * 16 + x + 1];
            grid[y * 16 + x] = sum;
            return sum;
        }
    }
}