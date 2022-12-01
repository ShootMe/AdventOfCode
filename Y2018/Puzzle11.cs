using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Chronal Charge")]
    public class Puzzle11 : ASolver {
        private int serialNumber;
        private int[] grid, sums;

        public override void Setup() {
            serialNumber = Tools.ParseInt(Input);
            grid = new int[300 * 300];
            sums = new int[300 * 300];
            int x = 1;
            int y = 1;
            for (int i = 0; i < grid.Length; i++) {
                grid[i] = CalculatePower(x, y);

                x++;
                if (x == 301) {
                    x = 1;
                    y++;
                }
            }
        }
        private int CalculatePower(int x, int y) {
            int rackID = x + 10;
            int power = rackID * y + serialNumber;
            power *= rackID;
            power = (power / 100) % 10;
            return power - 5;
        }

        [Description("What is the X,Y of the 3x3 square with the largest total power?")]
        public override string SolvePart1() {
            Array.Copy(grid, sums, grid.Length);

            int maxValue = 0;
            int maxX = 0;
            int maxY = 0;
            for (int j = 1; j <= 3; j++) {
                int x = 0;
                int y = 0;
                for (int i = 0; i < grid.Length; i++) {
                    int sum = SumSection(x, y, j);
                    if (sum > maxValue && j == 3) {
                        maxValue = sum;
                        maxX = x;
                        maxY = y;
                    }

                    x++;
                    if (x == 300) {
                        x = 0;
                        y++;
                    }
                }
            }
            return $"{maxX + 1},{maxY + 1}";
        }

        [Description("What is the X,Y,size identifier of the square with the largest total power?")]
        public override string SolvePart2() {
            Array.Copy(grid, sums, grid.Length);

            int maxValue = 0;
            int maxX = 0;
            int maxY = 0;
            int maxSize = 1;
            for (int j = 1; j <= 300; j++) {
                int x = 0;
                int y = 0;
                for (int i = 0; i < grid.Length; i++) {
                    int sum = SumSection(x, y, j);
                    if (sum > maxValue) {
                        maxValue = sum;
                        maxX = x;
                        maxY = y;
                        maxSize = j;
                    }

                    x++;
                    if (x == 300) {
                        x = 0;
                        y++;
                    }
                }
            }
            return $"{maxX + 1},{maxY + 1},{maxSize}";
        }

        private int SumSection(int x, int y, int size) {
            int start = y * 300 + x;
            ref int sum = ref sums[start];
            if (size == 1) { return sum; }

            int yOff = start + (size - 1) * 300;
            if (y + size - 1 < 300) {
                for (int i = 0; i < size && x + i < 300; i++) {
                    sum += grid[yOff + i];
                }
            }
            size--;
            yOff = start + size;
            if (x + size < 300) {
                for (int i = 0; i < size && y + i < 300; i++) {
                    sum += grid[yOff];
                    yOff += 300;
                }
            }
            return sum;
        }
    }
}