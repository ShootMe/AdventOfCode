using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Seating System")]
    public class Puzzle11 : ASolver {
        private byte[] grid, tempState, original;
        private int width, height;

        public override void Setup() {
            List<string> items = Input.Lines();

            height = items.Count;
            width = items[0].Length;
            grid = new byte[height * width];
            tempState = new byte[height * width];
            original = new byte[height * width];
            int index = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                for (int j = 0; j < item.Length; j++) {
                    char s = item[j];
                    grid[index++] = s == '.' ? (byte)0 : s == 'L' ? (byte)2 : (byte)1;
                }
            }
            Array.Copy(grid, tempState, grid.Length);
            Array.Copy(grid, original, grid.Length);
        }

        [Description("How many seats end up occupied?")]
        public override string SolvePart1() {
            Array.Copy(original, grid, grid.Length);
            while (Simulate(4)) { }

            return $"{CountFilled()}";
        }

        [Description("How many seats end up occupied with the new rules?")]
        public override string SolvePart2() {
            Array.Copy(original, grid, grid.Length);
            while (Simulate(5, true)) { }

            return $"{CountFilled()}";
        }

        private int CountFilled() {
            int count = 0;
            for (int i = 0; i < grid.Length; i++) {
                if (grid[i] == 1) {
                    count++;
                }
            }
            return count;
        }
        private bool Simulate(int maxCount, bool tillFound = false) {
            int x = 0;
            int y = 0;
            bool changedState = false;
            for (int i = 0; i < grid.Length; i++) {
                byte pos = grid[i];
                if (pos != 0) {
                    byte tl = GetSeat(x, y, -1, -1, tillFound);
                    byte tm = GetSeat(x, y, 0, -1, tillFound);
                    byte tr = GetSeat(x, y, 1, -1, tillFound);
                    byte ml = GetSeat(x, y, -1, 0, tillFound);
                    byte mr = GetSeat(x, y, 1, 0, tillFound);
                    byte bl = GetSeat(x, y, -1, 1, tillFound);
                    byte bm = GetSeat(x, y, 0, 1, tillFound);
                    byte br = GetSeat(x, y, 1, 1, tillFound);

                    int count = tl + tm + tr + ml + mr + bl + bm + br;

                    if (pos == 2 && count == 0) {
                        tempState[i] = 1;
                        changedState = true;
                    } else if (pos == 1 && count >= maxCount) {
                        tempState[i] = 2;
                        changedState = true;
                    }
                }

                x++;
                if (x == width) {
                    x = 0;
                    y++;
                }
            }

            Array.Copy(tempState, grid, grid.Length);
            return changedState;
        }
        private byte GetSeat(int x, int y, int moveX, int moveY, bool tillFound) {
            byte seat = 0;
            while ((x += moveX) >= 0 && (y += moveY) >= 0 && x < width && y < height) {
                seat = grid[y * width + x];
                if (seat != 0 || !tillFound) { break; }
            }
            return (byte)(seat & 1);
        }
        private void Display() {
            int x = 0;
            int y = 0;
            for (int i = 0; i < grid.Length; i++) {
                Console.Write(grid[i] == 0 ? '.' : grid[i] == 1 ? '#' : 'L');
                x++;
                if (x == width) {
                    x = 0;
                    y++;
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}