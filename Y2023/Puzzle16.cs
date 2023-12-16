using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("The Floor Will Be Lava")]
    public class Puzzle16 : ASolver {
        private char[,] grid;
        private int width, height;
        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;
            grid = new char[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    grid[y, x] = line[x];
                }
            }
        }

        [Description("With the beam starting in the top-left heading right, how many tiles end up being energized?")]
        public override string SolvePart1() {
            return $"{EnergizedTiles(-1, 0, 1, 0)}";
        }

        [Description("How many tiles are energized in that configuration?")]
        public override string SolvePart2() {
            int max = 0;
            for (int y = 0; y < height; y++) {
                int tiles = EnergizedTiles(-1, y, 1, 0);
                if (tiles > max) { max = tiles; }
                tiles = EnergizedTiles(width, y, -1, 0);
                if (tiles > max) { max = tiles; }
            }
            for (int x = 0; x < width; x++) {
                int tiles = EnergizedTiles(x, -1, 0, 1);
                if (tiles > max) { max = tiles; }
                tiles = EnergizedTiles(x, height, 0, -1);
                if (tiles > max) { max = tiles; }
            }
            return $"{max}";
        }

        private int EnergizedTiles(int xs, int ys, int dxs, int dys) {
            Queue<(int, int, int, int)> beams = new();
            HashSet<(int, int, int, int)> seen = new(height * width * 2);
            HashSet<(int, int)> energized = new(height * width);
            beams.Enqueue((xs, ys, dxs, dys));

            while (beams.Count > 0) {
                (int x, int y, int dx, int dy) = beams.Dequeue();
                energized.Add((x, y));

                int nx = x + dx;
                int ny = y + dy;
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) { continue; }

                char position = grid[ny, nx];
                if (position == '.' || (position == '-' && dx != 0) || (position == '|' && dy != 0)) {
                    if (seen.Add((nx, ny, dx, dy))) {
                        beams.Enqueue((nx, ny, dx, dy));
                    }
                } else if (position == '\\' && seen.Add((nx, ny, dx, dy))) {
                    beams.Enqueue((nx, ny, dy, dx));
                } else if (position == '/' && seen.Add((nx, ny, dx, dy))) {
                    beams.Enqueue((nx, ny, -dy, -dx));
                } else if (position == '-' && seen.Add((nx, ny, dx, dy))) {
                    beams.Enqueue((nx, ny, -1, 0));
                    beams.Enqueue((nx, ny, 1, 0));
                } else if (position == '|' && seen.Add((nx, ny, dx, dy))) {
                    beams.Enqueue((nx, ny, 0, -1));
                    beams.Enqueue((nx, ny, 0, 1));
                }
            }

            return energized.Count - 1;
        }
    }
}