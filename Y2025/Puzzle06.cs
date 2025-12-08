using System;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Trash Compactor")]
    public class Puzzle06 : ASolver {
        private string[] lines;
        private int width, height;
        private char[] operations;

        public override void Setup() {
            lines = Input.Split('\n');
            height = lines.Length - 1;
            string[] ops = lines[height].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            width = ops.Length;
            operations = new char[width];
            for (int i = 0; i < width; i++) {
                operations[i] = ops[i][0];
            }
        }

        [Description("What is the grand total found by adding together all of the answers to the individual problems?")]
        public override string SolvePart1() {
            int[,] numbers = new int[width, height];
            for (int i = 0; i < height; i++) {
                string line = lines[i];
                string[] nums = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < width; j++) {
                    numbers[j, i] = nums[j].ToInt();
                }
            }
            long total = 0;
            for (int i = 0; i < width; i++) {
                long sub = numbers[i, 0];
                char op = operations[i];
                for (int j = 1; j < height; j++) {
                    switch (op) {
                        case '+': sub += numbers[i, j]; break;
                        case '*': sub *= numbers[i, j]; break;
                    }
                }
                total += sub;
            }
            return $"{total}";
        }

        [Description("What is the grand total found by adding together all of the answers to the individual problems?")]
        public override string SolvePart2() {
            long total = 0;
            int lineWidth = lines[0].Length;
            for (int i = 0, k = 0; i < width; i++) {
                char op = operations[i];
                long sub = op == '*' ? 1 : 0;

                for (; k < lineWidth; k++) {
                    long number = 0;
                    for (int j = 0; j < height; j++) {
                        char c = lines[j][k];
                        if (c == ' ') {
                            if (number == 0) { continue; }
                            break;
                        }
                        number = number * 10 + (c - '0');
                    }
                    if (number == 0) { k++; break; }

                    switch (op) {
                        case '+': sub += number; break;
                        case '*': sub *= number; break;
                    }
                }
                total += sub;
            }
            return $"{total}";
        }
    }
}