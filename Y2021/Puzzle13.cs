using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Transparent Origami")]
    public class Puzzle13 : ASolver {
        private HashSet<(int, int)> dots;
        private List<int> folds;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            dots = new HashSet<(int, int)>();
            folds = new List<int>();

            int i = 0;
            while (i < items.Count) {
                string item = items[i++];
                if (string.IsNullOrEmpty(item)) { break; }

                string[] values = item.Split(',');
                dots.Add((Tools.ParseInt(values[0]), Tools.ParseInt(values[1])));
            }

            while (i < items.Count) {
                string item = items[i++];
                folds.Add((item[11] == 'x' ? 1 : -1) * Tools.ParseInt(item, 13));
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

            bool[,] grid = new bool[maxX - minX, maxY - minY];
            foreach ((int x, int y) in dots) {
                grid[x - minX, y - minY] = true;
            }

            // ██  ███   ██  ███  ████ ████  ██  █  █  ███   ██ █  █ █     ██  ███   ██  ███   ███ ████ █  █ █   ██   █████
            //█  █ █  █ █  █ █  █ █    █    █  █ █  █   █     █ █ █  █    █  █ █  █ █  █ █  █ █      █  █  █  █ █ █   █   █
            //████ ███  █    █  █ ███  ███  █    ████   █     █ ██   █    █  █ █  █ █  █ █  █ █      █  █  █   █   █ █   █
            //█  █ █  █ █    █  █ █    █    █ ██ █  █   █     █ █ █  █    █  █ ███  █  █ ███   ██    █  █  █   █    █   █
            //█  █ █  █ █  █ █  █ █    █    █  █ █  █   █  █  █ █ █  █    █  █ █     ██  █ █     █   █  █  █  █ █   █  █
            //█  █ ███   ██  ███  ████ █     ██  █  █  ███  ██  █  █ ████  ██  █       █ █  █ ███    █   ██  █   █  █  ████
            Dictionary<int, char> charMap = new Dictionary<int, char>() { { 210, 'A' }, { 169, 'B' }, { 112, 'C' }, { 175, 'D' }, { 137, 'E' }, { 53, 'F' }, { 160, 'G' }, { 207, 'H' }, { 154, 'I' }, { 165, 'J' }, { 127, 'K' }, { 105, 'L' }, { 168, 'O' }, { 91, 'P' }, { 159, 'R' }, { 125, 'S' }, { 95, 'T' }, { 55, 'T' }, { 171, 'U' }, { 73, 'Y' }, { 146, 'Z' } };
            char[] characters = new char[amountOfChars];
            for (int i = 0; i < amountOfChars; i++) {
                int charSum = 0;
                int startX = i * (charWidth + 1) + minX;
                int endX = (i + 1) * (charWidth + 1) + minX - 1;
                for (int j = minY; j < maxY; j++) {
                    int rowSum = 0;
                    for (int k = startX; k < endX; k++) {
                        if (grid[k - minX, j - minY]) {
                            rowSum += 1 << (k - startX);
                        }
                    }
                    charSum += rowSum * (j - minY + 1);
                }

                if (charMap.TryGetValue(charSum, out char found)) {
                    characters[i] = found;
                } else {
                    characters[i] = '?';
                }
            }

            //System.Console.WriteLine();
            //for (int j = minY; j < maxY; j++) {
            //    for (int i = minX; i < maxX; i++) {
            //        System.Console.Write(grid[i, j] ? '█' : ' ');
            //    }
            //    System.Console.WriteLine();
            //}

            return new string(characters);
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