using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Transparent Origami")]
    public class Puzzle13 : ASolver {
        private HashSet<(int, int)> dots;
        private List<int> folds;

        public override void Setup() {
            List<string> items = Input.Lines();
            dots = new HashSet<(int, int)>();
            folds = new List<int>();

            int i = 0;
            while (i < items.Count) {
                string item = items[i++];
                if (string.IsNullOrEmpty(item)) { break; }

                string[] values = item.Split(',');
                dots.Add((values[0].ToInt(), values[1].ToInt()));
            }

            while (i < items.Count) {
                string item = items[i++];
                folds.Add((item[11] == 'x' ? 1 : -1) * item.Substring(13).ToInt());
            }
        }

        [Description("How many dots are visible after completing just the first fold instruction on your transparent paper?")]
        public override string SolvePart1() {
            Fold(folds[0]);
            return $"{dots.Count}";
        }

        [Description("What code do you use to activate the infrared thermal imaging camera system?")]
        public override string SolvePart2() {
            for (int i = 1; i < folds.Count; i++) {
                Fold(folds[i]);
            }

            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            foreach ((int x, int y) in dots) {
                if (x < minX) { minX = x; }
                if (x > maxX) { maxX = x; }
                if (y < minY) { minY = y; }
                if (y > maxY) { maxY = y; }
            }
            maxX++;
            maxY++;

            int charWidth = maxY - minY - 2;
            int amountOfChars = (maxX - minX + 1) / (charWidth + 1);

            bool[,] grid = new bool[maxY - minY, maxX - minX];
            foreach ((int x, int y) in dots) {
                grid[y - minY, x - minX] = true;
            }

            return Extensions.FindStringInGrid(grid);
        }

        private void Fold(int fold) {
            List<(int, int)> removeDots = new List<(int, int)>();
            List<(int, int)> addDots = new List<(int, int)>();
            foreach ((int x, int y) in dots) {
                if (fold > 0) {
                    if (x <= fold) { continue; }

                    addDots.Add((fold + fold - x, y));
                    removeDots.Add((x, y));
                } else {
                    if (y <= -fold) { continue; }

                    addDots.Add((x, -fold + -fold - y));
                    removeDots.Add((x, y));
                }
            }

            for (int i = 0; i < removeDots.Count; i++) {
                dots.Remove(removeDots[i]);
            }
            for (int i = 0; i < addDots.Count; i++) {
                dots.Add(addDots[i]);
            }
        }
    }
}