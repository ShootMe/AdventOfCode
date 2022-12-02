using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Monitoring Station")]
    public class Puzzle10 : ASolver {
        private bool[] grid;
        private int width;
        private HashSet<Point> asteroids;
        private Point station;

        public override void Setup() {
            List<string> items = Input.Lines();
            grid = new bool[Input.Length];
            width = items[0].Length;
            for (int i = 0; i < items.Count; i++) {
                string row = items[i];
                for (int j = 0; j < row.Length; j++) {
                    if (row[j] == '#') {
                        grid[i * row.Length + j] = true;
                    }
                }
            }

            int x = 0;
            int y = 0;
            int maxCount = 0;
            for (int i = 0; i < grid.Length; i++) {
                if (grid[i]) {
                    HashSet<Point> slopes = AsteroidSlopes(x, y);
                    if (slopes.Count > maxCount) {
                        maxCount = slopes.Count;
                        asteroids = slopes;
                        station = new Point() { X = x, Y = y };
                    }
                }

                x++;
                if (x == width) {
                    y++;
                    x = 0;
                }
            }
        }

        [Description("How many other asteroids can be detected from that location?")]
        public override string SolvePart1() {
            return $"{asteroids.Count}";
        }

        [Description("What is the coordinate of the 200th asteroid destroyed?")]
        public override string SolvePart2() {
            int destroyed = 0;
            Point nextToDestroy = default;
            while (destroyed < 200) {
                nextToDestroy = default;
                foreach (Point slope in asteroids) {
                    if (slope < nextToDestroy) {
                        nextToDestroy = slope;
                    }
                }

                asteroids.Remove(nextToDestroy);
                destroyed++;
            }

            Point point = station;
            do {
                point += nextToDestroy;
            } while (!grid[point.X + point.Y * width]);

            return $"{point.X * 100 + point.Y}";
        }

        private HashSet<Point> AsteroidSlopes(int x1, int y1) {
            int x2 = 0;
            int y2 = 0;
            HashSet<Point> slopes = new HashSet<Point>();
            for (int i = 0; i < grid.Length; i++) {
                if ((x2 != x1 || y2 != y1) && grid[i]) {
                    int sX = x2 - x1;
                    int sY = y2 - y1;
                    int gcd = Extensions.GCD(sX, sY);
                    sX /= gcd;
                    sY /= gcd;

                    Point slope = new Point() { X = sX, Y = sY };
                    if (!slopes.Contains(slope)) {
                        slopes.Add(slope);
                    }
                }

                x2++;
                if (x2 == width) {
                    y2++;
                    x2 = 0;
                }
            }

            return slopes;
        }
    }
}