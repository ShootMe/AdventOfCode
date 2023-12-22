using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Step Counter")]
    public class Puzzle21 : ASolver {
        private bool[,] grid;
        private int[,] gridMoves;
        private int width, height;
        private (int x, int y) start;
        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;

            grid = new bool[height, width];
            gridMoves = new int[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    grid[y, x] = line[x] == '#';
                    if (line[x] == 'S') { start = (x, y); }
                }
            }

            FillGrid(start.x, start.y);
        }

        [Description("How many garden plots could the Elf reach in exactly 64 steps?")]
        public override string SolvePart1() {
            return $"{CountsFor(0, 64).Even}";
        }

        [Description("How many garden plots could the Elf reach in exactly 26501365 steps?")]
        public override string SolvePart2() {
            long extraGrids = (26501365 - start.x) / width;
            (int Even, int Odd) counts = CountsFor(0, width * 2);
            long total = (extraGrids + 1) * (extraGrids + 1) * counts.Odd + extraGrids * extraGrids * counts.Even;
            (int Even, int Odd) corners = CountsFor(width / 2 + 1, width * 2);
            total += extraGrids * corners.Even - (extraGrids + 1) * corners.Odd;
            return $"{total}";
        }
        private (int Even, int Odd) CountsFor(int minLength, int maxLength) {
            int countO = 0;
            int countE = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (grid[y, x]) { continue; }

                    int length = gridMoves[y, x];
                    if (length >= minLength && maxLength >= length) {
                        if (((maxLength - length) & 1) == 0) {
                            countE++;
                        } else {
                            countO++;
                        }
                    }
                }
            }
            return (countE, countO);
        }
        private void FillGrid(int xs, int ys) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    gridMoves[y, x] = -1;
                }
            }

            Queue<(int, int, int)> open = new(height * width);

            void TryAdd(int nx, int ny, int length) {
                if (nx < 0 || ny < 0 || nx >= width || ny >= height || grid[ny, nx] || gridMoves[ny, nx] >= 0) { return; }

                gridMoves[ny, nx] = length + 1;
                open.Enqueue((nx, ny, length + 1));
            }

            open.Enqueue((xs, ys, 0));
            gridMoves[ys, xs] = 0;

            while (open.Count > 0) {
                (int x, int y, int length) = open.Dequeue();

                TryAdd(x - 1, y, length);
                TryAdd(x + 1, y, length);
                TryAdd(x, y - 1, length);
                TryAdd(x, y + 1, length);
            }
        }
    }
}