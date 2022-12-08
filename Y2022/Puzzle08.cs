using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Treetop Tree House")]
    public class Puzzle08 : ASolver {
        private int[] forest;
        private int height;
        private int width;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            height = lines.Length;
            width = lines[0].Length;
            forest = new int[height * width];
            for (int i = 0, k = 0; i < lines.Length; i++) {
                string line = lines[i];
                for (int j = 0; j < line.Length; j++) {
                    forest[k++] = line[j] - '0';
                }
            }
        }

        [Description("Consider your map; how many trees are visible from outside the grid?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (IsVisible(i, j)) {
                        total++;
                    }
                }
            }
            return $"{total}";
        }
        
        [Description("Consider each tree on your map. What is the highest scenic score possible for any tree?")]
        public override string SolvePart2() {
            int max = 0;
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    int count = VisibleCount(i, j);
                    if (count > max) {
                        max = count;
                    }
                }
            }
            return $"{max}";
        }

        private bool IsVisible(int x, int y) {
            if (x == 0 || x == width - 1 || y == 0 || y == height - 1) { return true; }

            int start = y * height + x;
            int tree = forest[start];
            bool visible = true;
            for (int i = start + height; i < forest.Length; i += height) {
                if (tree <= forest[i]) {
                    visible = false;
                    break;
                }
            }
            if (visible) { return true; }

            visible = true;
            for (int i = start - height; i >= 0; i -= height) {
                if (tree <= forest[i]) {
                    visible = false;
                    break;
                }
            }
            if (visible) { return true; }

            visible = true;
            for (int i = start + 1, xt = x + 1; i < forest.Length && xt < width; i++, xt++) {
                if (tree <= forest[i]) {
                    visible = false;
                    break;
                }
            }
            if (visible) { return true; }

            visible = true;
            for (int i = start - 1, xt = x - 1; i >= 0 && xt >= 0; i--, xt--) {
                if (tree <= forest[i]) {
                    visible = false;
                    break;
                }
            }
            return visible;
        }

        private int VisibleCount(int x, int y) {
            if (x == 0 || x == width - 1 || y == 0 || y == height - 1) { return 0; }

            int start = y * height + x;
            int tree = forest[start];
            int count = 0;
            for (int i = start + height; i < forest.Length; i += height) {
                count++;
                if (tree <= forest[i]) {
                    break;
                }
            }

            int total = count;
            count = 0;
            for (int i = start - height; i >= 0; i -= height) {
                count++;
                if (tree <= forest[i]) {
                    break;
                }
            }
            total *= count;
            count = 0;
            for (int i = start + 1, xt = x + 1; i < forest.Length && xt < width; i++, xt++) {
                count++;
                if (tree <= forest[i]) {
                    break;
                }
            }
            total *= count;
            count = 0;
            for (int i = start - 1, xt = x - 1; i >= 0 && xt >= 0; i--, xt--) {
                count++;
                if (tree <= forest[i]) {
                    break;
                }
            }
            return total * count;
        }
    }
}