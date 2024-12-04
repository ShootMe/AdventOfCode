using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Ceres Search")]
    public class Puzzle04 : ASolver {
        private string[] xmas;
        private int height, width;

        public override void Setup() {
            xmas = Input.Split('\n');
            height = xmas.Length;
            width = xmas[0].Length;
        }

        [Description("How many times does XMAS appear?")]
        public override string SolvePart1() {
            int total = 0;
            for (int y = 0; y < height; y++) {
                string line = xmas[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    if (c == 'X') {
                        total += CountXMAS(x, y);
                    }
                }
            }
            return $"{total}";
        }
        private int CountXMAS(int x, int y) {
            return IsXMAS(x, y, -1, -1) + IsXMAS(x, y, 0, -1) + IsXMAS(x, y, 1, -1) +
                IsXMAS(x, y, -1, 0) + IsXMAS(x, y, 1, 0) +
                IsXMAS(x, y, -1, 1) + IsXMAS(x, y, 0, 1) + IsXMAS(x, y, 1, 1);
        }
        private int IsXMAS(int x, int y, int xd, int yd) {
            if (y + 3 * yd < 0 || y + 3 * yd >= height) { return 0; }
            if (x + 3 * xd < 0 || x + 3 * xd >= width) { return 0; }

            return xmas[y + yd][x + xd] == 'M' &&
                xmas[y + 2 * yd][x + 2 * xd] == 'A' &&
                xmas[y + 3 * yd][x + 3 * xd] == 'S' ? 1 : 0;
        }

        [Description("How many times does an X-MAS appear?")]
        public override string SolvePart2() {
            int total = 0;
            for (int y = 0; y < height; y++) {
                string line = xmas[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    if (c == 'A') {
                        total += CountX_MAS(x, y);
                    }
                }
            }
            return $"{total}";
        }
        private int CountX_MAS(int x, int y) {
            if (y - 1 < 0 || y + 1 >= height) { return 0; }
            if (x - 1 < 0 || x + 1 >= width) { return 0; }

            return ((xmas[y - 1][x - 1] == 'M' && xmas[y + 1][x + 1] == 'S') || (xmas[y - 1][x - 1] == 'S' && xmas[y + 1][x + 1] == 'M')) &&
                ((xmas[y - 1][x + 1] == 'M' && xmas[y + 1][x - 1] == 'S') || (xmas[y - 1][x + 1] == 'S' && xmas[y + 1][x - 1] == 'M')) ? 1 : 0;
        }
    }
}