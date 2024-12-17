using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Reindeer Maze")]
    public class Puzzle16 : ASolver {
        private char[,] map;
        private int width, height, endX, endY;
        private Queue<(int, int, int, int)> open = new();
        private Dictionary<(int, int), int> seen = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            height = lines.Length;
            width = lines[0].Length;
            map = new char[height, width];

            int startX = 0, startY = 0;
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

            open.Enqueue((startX, startY, 0, 0));
            seen.Add((startX, startY), 0);
        }

        [Description("What is the lowest score a Reindeer could possibly get?")]
        public override string SolvePart1() {
            void AddNext(int x, int y, int dir, int score) {
                if (dir < 0) { dir += 4; } else if (dir > 3) { dir -= 4; }
                switch (dir) {
                    case 0: x++; break;
                    case 1: y++; break;
                    case 2: x--; break;
                    case 3: y--; break;
                }
                if (map[y, x] == '#') { return; }

                int oldScore = 0;
                bool exists = seen.TryGetValue((x, y), out oldScore);
                if (!exists || oldScore > score) {
                    seen[(x, y)] = score;
                    open.Enqueue((x, y, dir, score));
                }
            }

            int minScore = int.MaxValue;
            int minDir = int.MaxValue;
            while (open.Count > 0) {
                (int x, int y, int dir, int score) = open.Dequeue();

                if (map[y, x] == 'E') {
                    if (score < minScore) {
                        minScore = score;
                        minDir = dir;
                    }
                    continue;
                }

                AddNext(x, y, dir, score + 1);
                AddNext(x, y, dir + 1, score + 1001);
                AddNext(x, y, dir - 1, score + 1001);
            }

            open.Enqueue((endX, endY, minDir, minScore));
            return $"{minScore}";
        }

        [Description("How many tiles are part of at least one of the best paths through the maze?")]
        public override string SolvePart2() {
            HashSet<(int, int)> bestPath = new();
            void CheckPrevious(int x, int y, int dir, int score) {
                if (dir < 0) { dir += 4; } else if (dir > 3) { dir -= 4; }
                switch (dir) {
                    case 0: x--; break;
                    case 1: y--; break;
                    case 2: x++; break;
                    case 3: y++; break;
                }

                if (map[y, x] == '#' || !seen.TryGetValue((x, y), out int prevScore) || prevScore > score) { return; }

                open.Enqueue((x, y, dir, score));
            }

            while (open.Count > 0) {
                (int x, int y, int dir, int score) = open.Dequeue();
                if (!bestPath.Add((x, y))) { continue; }

                CheckPrevious(x, y, dir, score - 1);
                CheckPrevious(x, y, dir + 1, score - 1001);
                CheckPrevious(x, y, dir - 1, score - 1001);
            }
            return $"{bestPath.Count}";
        }
        private class Path {
            public int X, Y, Dir, Score;
            public Path Last;
            public Path(int x, int y, int dir, int score, Path last) {
                X = x; Y = y; Dir = dir; Score = score; Last = last;
            }
        }
    }
}