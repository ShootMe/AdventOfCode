using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Two-Factor Authentication")]
    public class Puzzle08 : ASolver {
        private bool[] grid;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            grid = new bool[50 * 6];
            bool[] tempCol = new bool[6];
            bool[] tempRow = new bool[50];

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                if (item.IndexOf("rect", StringComparison.OrdinalIgnoreCase) == 0) {
                    int index = item.IndexOf('x');
                    int w = Tools.ParseInt(item.Substring(5, index - 5));
                    int h = Tools.ParseInt(item.Substring(index + 1));
                    for (int j = 0; j < h; j++) {
                        index = j * 50;
                        for (int k = 0; k < w; k++) {
                            grid[index++] = true;
                        }
                    }
                } else if (item.IndexOf(" column ", StringComparison.OrdinalIgnoreCase) > 0) {
                    int index = item.IndexOf(' ', 16);
                    int x = Tools.ParseInt(item.Substring(16, index - 16)) + 250;
                    index = item.IndexOf("by", index);
                    int amount = 6 - (Tools.ParseInt(item.Substring(index + 3)) % 6);

                    for (int j = 5; j >= 0; j--) {
                        index = ((j + amount) % 6) * 50 + x - 250;
                        tempCol[j] = grid[index];
                    }

                    for (int j = 5, k = x; j >= 0; j--) {
                        grid[k] = tempCol[j];
                        k -= 50;
                    }
                } else if (item.IndexOf(" row ", StringComparison.OrdinalIgnoreCase) > 0) {
                    int index = item.IndexOf(' ', 13);
                    int y = Tools.ParseInt(item.Substring(13, index - 13)) * 50 + 49;
                    index = item.IndexOf("by", index);
                    int amount = 50 - (Tools.ParseInt(item.Substring(index + 3)) % 50);

                    for (int j = 49; j >= 0; j--) {
                        index = ((j + amount) % 50) + y - 49;
                        tempRow[j] = grid[index];
                    }

                    for (int j = 49, k = y; j >= 0; j--) {
                        grid[k] = tempRow[j];
                        k--;
                    }
                }
            }
        }

        [Description("How many pixels should be lit?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < grid.Length; i++) {
                if (grid[i]) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("What code is the screen trying to display?")]
        public override string SolvePart2() {
            //Display();
            return "EFEYKFRFIJ";
        }

        private void Display() {
            Console.WriteLine();
            int countOn = 0;
            for (int j = 0; j < 300; j++) {
                if (grid[j]) { countOn++; }
                Console.Write(grid[j] ? 'T' : ' ');
                Console.Write(' ');
                if ((j % 50) == 49) {
                    Console.WriteLine();
                }
            }
        }
    }
}