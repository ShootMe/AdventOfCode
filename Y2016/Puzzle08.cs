using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Two-Factor Authentication")]
    public class Puzzle08 : ASolver {
        private bool[,] grid;

        public override void Setup() {
            List<string> items = Input.Lines();
            grid = new bool[6, 50];
            bool[] tempCol = new bool[6];
            bool[] tempRow = new bool[50];

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                if (item.IndexOf("rect", StringComparison.OrdinalIgnoreCase) == 0) {
                    int index = item.IndexOf('x');
                    int w = item.Substring(5, index - 5).ToInt();
                    int h = item.Substring(index + 1).ToInt();
                    for (int j = 0; j < h; j++) {
                        for (int k = 0; k < w; k++) {
                            grid[j, k] = true;
                        }
                    }
                } else if (item.IndexOf(" column ", StringComparison.OrdinalIgnoreCase) > 0) {
                    int index = item.IndexOf(' ', 16);
                    int x = item.Substring(16, index - 16).ToInt();
                    index = item.IndexOf("by", index);
                    int amount = 6 - (item.Substring(index + 3).ToInt() % 6);

                    for (int j = 5; j >= 0; j--) {
                        tempCol[j] = grid[(j + amount) % 6, x];
                    }

                    for (int j = 5; j >= 0; j--) {
                        grid[j, x] = tempCol[j];
                    }
                } else if (item.IndexOf(" row ", StringComparison.OrdinalIgnoreCase) > 0) {
                    int index = item.IndexOf(' ', 13);
                    int y = item.Substring(13, index - 13).ToInt();
                    index = item.IndexOf("by", index);
                    int amount = 50 - (item.Substring(index + 3).ToInt() % 50);

                    for (int j = 49; j >= 0; j--) {
                        tempRow[j] = grid[y, (j + amount) % 50];
                    }

                    for (int j = 49; j >= 0; j--) {
                        grid[y, j] = tempRow[j];
                    }
                }
            }
        }

        [Description("How many pixels should be lit?")]
        public override string SolvePart1() {
            int count = 0;
            for (int y = 0; y < 6; y++) {
                for (int x = 0; x < 50; x++) {
                    if (grid[y, x]) { count++; }
                }
            }
            return $"{count}";
        }

        [Description("What code is the screen trying to display?")]
        public override string SolvePart2() {
            //Display();
            return Extensions.FindStringInGrid(grid);
        }

        private void Display() {
            Console.WriteLine();
            for (int y = 0; y < 6; y++) {
                for (int x = 0; x < 50; x++) {
                    Console.Write(grid[y, x] ? '#' : ' ');
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }
    }
}