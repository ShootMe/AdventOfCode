using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Smoke Basin")]
    public class Puzzle09 : ASolver {
        private int[] map;
        private int width, height;
        private List<int> lowPoints;

        public override void Setup() {
            List<string> items = Input.Lines();
            width = items[0].Length;
            height = items.Count;
            map = new int[width * height];
            lowPoints = new List<int>();
            int pos = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                for (int j = 0; j < width; j++) {
                    map[pos++] = item[j] - '0';
                }
            }
        }

        [Description("What is the sum of the risk levels of all low points on your heightmap?")]
        public override string SolvePart1() {
            int totalRisk = 0;
            for (int i = 0; i < map.Length; i++) {
                int riskLevel = RiskLevelAt(i);
                if (riskLevel > 0) { lowPoints.Add(i); }

                totalRisk += riskLevel;
            }
            return $"{totalRisk}";
        }

        [Description("What do you get if you multiply together the sizes of the three largest basins?")]
        public override string SolvePart2() {
            Basin[] basins = FillBasins();
            Array.Sort(basins, (Basin left, Basin right) => {
                return right.Area.CompareTo(left.Area);
            });

            return $"{basins[0].Area * basins[1].Area * basins[2].Area}";
        }
        private int RiskLevelAt(int pos) {
            int x = pos % width;
            int y = pos / width;
            int value = map[pos];
            return (x - 1 < 0 || map[pos - 1] > value) &&
                (x + 1 >= width || map[pos + 1] > value) &&
                (y - 1 < 0 || map[pos - width] > value) &&
                (y + 1 >= height || map[pos + width] > value) ? value + 1 : 0;
        }
        private Basin[] FillBasins() {
            Basin[] basins = new Basin[lowPoints.Count];
            for (int i = 0; i < lowPoints.Count; i++) {
                Basin current = new Basin() { Position = lowPoints[i] };
                basins[i] = current;
                FillBasins(current.Position, current);
            }
            return basins;
        }
        private void FillBasins(int pos, Basin basin) {
            if (map[pos] == 9) { return; }

            map[pos] = 9;
            basin.Area++;

            int x = pos % width;
            if (x + 1 < width) { FillBasins(pos + 1, basin); }
            if (x - 1 >= 0) { FillBasins(pos - 1, basin); }
            int y = pos / width;
            if (y - 1 >= 0) { FillBasins(pos - width, basin); }
            if (y + 1 < height) { FillBasins(pos + width, basin); }
        }
        private class Basin {
            public int Position;
            public int Area;
            public override string ToString() {
                return $"{Position}={Area}";
            }
        }
    }
}