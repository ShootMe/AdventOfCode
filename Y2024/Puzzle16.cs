using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Reindeer Maze")]
    //[Run("Example")]
    //[Run("Given")]
    public class Puzzle16 : ASolver {
        private char[,] map;
        private int width, height, startX, startY, endX, endY;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            height = lines.Length;
            width = lines[0].Length;
            map = new char[height, width];

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
        }

        [Description("What is the lowest score a Reindeer could possibly get?")]
        public override string SolvePart1() {
            Dictionary<(int, int), int> seen = new();
            Queue<(int, int, int, int)> open = new();
            open.Enqueue((startX, startY, 0, 0));
            seen.Add((startX, startY), 0);

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
            while (open.Count > 0) {
                (int x, int y, int dir, int score) = open.Dequeue();
                if (map[y, x] == 'E') {
                    if (score < minScore) {
                        minScore = score;
                    }
                    continue;
                }

                AddNext(x, y, dir, score + 1);
                AddNext(x, y, dir + 1, score + 1001);
                AddNext(x, y, dir - 1, score + 1001);
            }
            return $"{minScore}";
        }

        [Description("How many tiles are part of at least one of the best paths through the maze?")]
        public override string SolvePart2() {
            Dictionary<(int, int, int), int> seen = new();
            Queue<Path> open = new();
            open.Enqueue(new Path(startX, startY, 0, 0, null));
            seen.Add((startX, startY, 0), 0);

            void AddNext(Path current, int dir, int score) {
                int x = current.X, y = current.Y;
                if (dir < 0) { dir += 4; } else if (dir > 3) { dir -= 4; }
                switch (dir) {
                    case 0: x++; break;
                    case 1: y++; break;
                    case 2: x--; break;
                    case 3: y--; break;
                }
                if (map[y, x] == '#') { return; }

                score += current.Score;
                int oldScore = 0;
                bool exists = seen.TryGetValue((x, y, dir), out oldScore);
                if (!exists || oldScore >= score) {
                    seen[(x, y, dir)] = score;
                    open.Enqueue(new Path(x, y, dir, score, current));
                }
            }

            HashSet<(int, int)> bestPath = new();
            int minScore = int.MaxValue;
            while (open.Count > 0) {
                Path current = open.Dequeue();
                if (map[current.Y, current.X] == 'E') {
                    if (current.Score < minScore) {
                        minScore = current.Score;
                        bestPath.Clear();
                    }
                    if (current.Score == minScore) {
                        Path temp = current;
                        int count = 0;
                        while (temp != null) {
                            bestPath.Add((temp.X, temp.Y));
                            temp = temp.Last;
                            count++;
                        }
                    }
                    continue;
                }

                AddNext(current, current.Dir, 1);
                AddNext(current, current.Dir + 1, 1001);
                AddNext(current, current.Dir - 1, 1001);
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