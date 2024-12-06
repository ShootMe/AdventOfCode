using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Guard Gallivant")]
    public class Puzzle06 : ASolver {
        private char[,] grid;
        private int width, height, startX, startY;
        private Dictionary<(int, int), (int, int)> seenArea = new();
        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;
            grid = new char[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    grid[y, x] = c;
                    if (c == '^') {
                        startX = x;
                        startY = y;
                    }
                }
            }
        }

        [Description("How many distinct positions will the guard visit before leaving the mapped area?")]
        public override string SolvePart1() {
            seenArea.Add((startX, startY), (0, -1));
            PatrolArea(startX, startY, 0, -1, delegate (int x, int y, int dirX, int dirY) {
                seenArea.TryAdd((x, y), (dirX, dirY));
            });
            return $"{seenArea.Count}";
        }
        private bool PatrolArea(int sX, int sY, int dirX, int dirY, Action<int, int, int, int> newLocation) {
            int x = sX, y = sY;
            HashSet<(int, int, int, int)> seenDir = new();

            while (true) {
                int newX = x + dirX;
                int newY = y + dirY;
                if (newX < 0 || newX >= width || newY < 0 || newY >= height) {
                    return false;
                } else if (grid[newY, newX] == '#') {
                    if (!seenDir.Add((x, y, dirX, dirY))) {
                        return true;
                    }
                    int tempDir = dirX;
                    dirX = -dirY;
                    dirY = tempDir;
                    continue;
                }

                x = newX; y = newY;
                newLocation?.Invoke(x, y, dirX, dirY);
            }
        }

        [Description("How many different positions could you choose for this obstruction?")]
        public override string SolvePart2() {
            int total = 0;
            seenArea.Remove((startX, startY));
            foreach (KeyValuePair<(int x, int y), (int dirX, int dirY)> pair in seenArea) {
                grid[pair.Key.y, pair.Key.x] = '#';
                int x = pair.Key.x - pair.Value.dirX;
                int y = pair.Key.y - pair.Value.dirY;
                bool isLoop = PatrolArea(x, y, -pair.Value.dirY, pair.Value.dirX, null);
                if (isLoop) {
                    total++;
                }
                grid[pair.Key.y, pair.Key.x] = '.';
            }

            return $"{total}";
        }
    }
}