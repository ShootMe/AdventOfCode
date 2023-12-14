using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Parabolic Reflector Dish")]
    public class Puzzle14 : ASolver {
        private char[,] grid;
        private int width, height, load;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;
            grid = new char[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    grid[y, x] = line[x];
                    if (line[x] == 'O') {
                        load += height - y;
                    }
                }
            }
        }

        [Description("What is the total load on the north support beams?")]
        public override string SolvePart1() {
            TiltNorth();
            return $"{load}";
        }

        [Description("Run the spin cycle for 1000000000 cycles. What is the total load on the north support beams?")]
        public override string SolvePart2() {
            TiltWest();
            TiltSouth();
            TiltEast();
            List<int> loads = new();
            loads.Add(load);
            for (int i = 1; i < 1000000000; i++) {
                TiltNorth();
                TiltWest();
                TiltSouth();
                TiltEast();
                loads.Add(load);

                (int position, int length) = FindSequence(loads);
                if (position >= 0) {
                    int leftOver = (1000000000 - position) % length;
                    return $"{loads[position + leftOver - 1]}";
                }
            }
            return string.Empty;
        }
        private (int index, int length) FindSequence(List<int> loads) {
            int lastIndex = loads.Count - 1;
            int minIndex = loads.Count / 2 - 1;
            for (int i = lastIndex - 5; i >= minIndex; i--) {
                int index = lastIndex;
                int newIndex = i;
                while (index > i && loads[index--] == loads[newIndex--]) { }
                if (index == i) { return (i, lastIndex - index); }
            }

            return (-1, 0);
        }
        private void TiltNorth() {
            int[] minIndexes = new int[width];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    char c = grid[y, x];
                    if (c != 'O') {
                        if (c == '#') { minIndexes[x] = y + 1; }
                        continue;
                    }

                    int ny = minIndexes[x];
                    if (y > ny) {
                        grid[ny, x] = 'O';
                        grid[y, x] = '.';
                        load += y - ny;
                        minIndexes[x] = ny + 1;
                    } else {
                        minIndexes[x] = y + 1;
                    }
                }
            }
        }
        private void TiltWest() {
            int[] minIndexes = new int[height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    char c = grid[y, x];
                    if (c != 'O') {
                        if (c == '#') { minIndexes[y] = x + 1; }
                        continue;
                    }

                    int nx = minIndexes[y];
                    if (x > nx) {
                        grid[y, nx] = 'O';
                        grid[y, x] = '.';
                        minIndexes[y] = nx + 1;
                    } else {
                        minIndexes[y] = x + 1;
                    }
                }
            }
        }
        private void TiltSouth() {
            int[] minIndexes = new int[width];
            Array.Fill(minIndexes, height - 1);
            for (int y = height - 1; y >= 0; y--) {
                for (int x = 0; x < width; x++) {
                    char c = grid[y, x];
                    if (c != 'O') {
                        if (c == '#') { minIndexes[x] = y - 1; }
                        continue;
                    }

                    int ny = minIndexes[x];
                    if (y < ny) {
                        grid[ny, x] = 'O';
                        grid[y, x] = '.';
                        load += y - ny;
                        minIndexes[x] = ny - 1;
                    } else {
                        minIndexes[x] = y - 1;
                    }
                }
            }
        }
        private void TiltEast() {
            int[] minIndexes = new int[height];
            Array.Fill(minIndexes, width - 1);
            for (int x = width - 1; x >= 0; x--) {
                for (int y = 0; y < height; y++) {
                    char c = grid[y, x];
                    if (c != 'O') {
                        if (c == '#') { minIndexes[y] = x - 1; }
                        continue;
                    }

                    int nx = minIndexes[y];
                    if (x < nx) {
                        grid[y, nx] = 'O';
                        grid[y, x] = '.';
                        minIndexes[y] = nx - 1;
                    } else {
                        minIndexes[y] = x - 1;
                    }
                }
            }
        }
    }
}