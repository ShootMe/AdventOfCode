using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Race Condition")]
    public class Puzzle20 : ASolver {
        private List<(int, int)> path = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            int height = lines.Length;
            int width = lines[0].Length;
            char[,] map = new char[height, width];

            int endX = 0, endY = 0, startX = 0, startY = 0;
            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    map[y, x] = c;
                    if (c == 'S') {
                        startX = x; startY = y;
                    } else if (c == 'E') {
                        endX = x; endY = y;
                    }
                }
            }

            void CreatePath() {
                (int x, int y) current = default;

                bool AddNext(int x, int y) {
                    if (map[y, x] == '#') { return false; }
                    current = (x, y);
                    path.Add(current);
                    map[y, x] = '#';
                    return true;
                }

                AddNext(startX, startY);

                while (AddNext(current.x + 1, current.y) ||
                    AddNext(current.x, current.y + 1) ||
                    AddNext(current.x - 1, current.y) ||
                    AddNext(current.x, current.y - 1)) {
                    if (current == (endX, endY)) {
                        return;
                    }
                }
            }

            CreatePath();
        }

        [Description("How many cheats would save you at least 100 picoseconds?")]
        public override string SolvePart1() {
            return $"{CountWays(2, path.Count > 100 ? 100 : 2)}";
        }

        [Description("How many cheats would save you at least 100 picoseconds?")]
        public override string SolvePart2() {
            return $"{CountWays(20, path.Count > 100 ? 100 : 50)}";
        }

        private int CountWays(int cheatMax, int saved) {
            int total = 0;
            for (int p1 = 0; p1 < path.Count - saved; p1++) {
                (int x1, int y1) = path[p1];

                for (int p2 = p1 + saved; p2 < path.Count;) {
                    (int x2, int y2) = path[p2];
                    int distance = Math.Abs(x2 - x1) + Math.Abs(y2 - y1);

                    if (distance > cheatMax) {
                        p2 += distance - cheatMax;
                        continue;
                    }

                    if ((p2 - p1 - distance) >= saved) { total++; }
                    p2++;
                }
            }
            return total;
        }
    }
}