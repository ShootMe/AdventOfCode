using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Treetop Tree House")]
    public class Puzzle08 : ASolver {
        private byte[,] forest;
        private int height;
        private int width;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            height = lines.Length;
            width = lines[0].Length;
            forest = new byte[height, width];
            for (int y = 0; y < lines.Length; y++) {
                string line = lines[y];
                for (int x = 0; x < line.Length; x++) {
                    forest[y, x] = (byte)(line[x] - '0');
                }
            }
        }

        [Description("Consider your map; how many trees are visible from outside the grid?")]
        public override string SolvePart1() {
            int total = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (IsVisible(x, y)) {
                        total++;
                    }
                }
            }
            return $"{total}";
        }

        [Description("Consider each tree on your map. What is the highest scenic score possible for any tree?")]
        public override string SolvePart2() {
            int max = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int count = VisibleCount(x, y);
                    if (count > max) {
                        max = count;
                    }
                }
            }
            return $"{max}";
        }

        private bool IsVisible(int x, int y) {
            if (x == 0 || x == width - 1 || y == 0 || y == height - 1) { return true; }

            int tree = forest[y, x];
            return CheckDirection(tree, x, y + 1, 0, 1).Visible ||
                CheckDirection(tree, x, y - 1, 0, -1).Visible ||
                CheckDirection(tree, x + 1, y, 1, 0).Visible ||
                CheckDirection(tree, x - 1, y, -1, 0).Visible;
        }
        private int VisibleCount(int x, int y) {
            if (x == 0 || x == width - 1 || y == 0 || y == height - 1) { return 0; }

            int tree = forest[y, x];
            return CheckDirection(tree, x, y + 1, 0, 1).Count *
                CheckDirection(tree, x, y - 1, 0, -1).Count *
                CheckDirection(tree, x + 1, y, 1, 0).Count *
                CheckDirection(tree, x - 1, y, -1, 0).Count;
        }
        private (bool Visible, int Count) CheckDirection(int tree, int x, int y, int xd, int yd) {
            int count = 0;
            while (x >= 0 && x < width && y >= 0 && y < height) {
                count++;
                if (tree <= forest[y, x]) {
                    return (false, count);
                }
                x += xd;
                y += yd;
            }
            return (true, count);
        }
    }
}