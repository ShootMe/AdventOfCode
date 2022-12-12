using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Hill Climbing Algorithm")]
    public class Puzzle12 : ASolver {
        private int[,] map;
        private int width;
        private int height;
        private (int x, int y) start;
        private (int x, int y) end;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            height = lines.Length;
            width = lines[0].Length;
            map = new int[height, width];

            for (int y = 0; y < lines.Length; y++) {
                string line = lines[y];
                for (int x = 0; x < line.Length; x++) {
                    char c = line[x];
                    if (c == 'S') {
                        start = (x, y);
                        map[y, x] = 0;
                    } else if (c == 'E') {
                        end = (x, y);
                        map[y, x] = 26;
                    } else {
                        map[y, x] = line[x] - 'a';
                    }
                }
            }
        }


        [Description("What is the fewest steps required to get to the location that should get the best signal?")]
        public override string SolvePart1() {
            return $"{StepsNeededAt(start.x, start.y)}";
        }

        [Description("What is the fewest steps required to move from any square with elevation a to the end?")]
        public override string SolvePart2() {
            int min = int.MaxValue;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (map[y, x] != 0) { continue; }

                    int moves = StepsNeededAt(x, y);
                    if (moves < min) {
                        min = moves;
                    }
                }
            }
            return $"{min}";
        }

        private int StepsNeededAt(int startX, int startY) {
            (int, int)[] dirs = { (0, 1), (1, 0), (-1, 0), (0, -1) };
            Heap<Path> queue = new Heap<Path>();
            HashSet<(int, int)> seen = new HashSet<(int, int)>(width * height);
            queue.Enqueue(new Path() { X = startX, Y = startY, Value = 0 });
            seen.Add((startX, startY));

            while (queue.Count > 0) {
                Path current = queue.Dequeue();

                if (current.X == end.x && current.Y == end.y) {
                    return current.Steps;
                }

                foreach ((int x, int y) in dirs) {
                    int tX = current.X + x; int tY = current.Y + y;
                    if (tX < 0 || tY < 0 || tX >= width || tY >= height) { continue; }

                    int value = map[tY, tX];
                    int diff = value - current.Value;
                    if (diff <= 1 && seen.Add((tX, tY))) {
                        queue.Enqueue(new Path() { X = tX, Y = tY, Value = value, Steps = current.Steps + 1 });
                    }
                }
            }

            return int.MaxValue;
        }

        private struct Path : IComparable<Path> {
            public int X, Y, Value, Steps;

            public int CompareTo(Path other) {
                int comp = Steps.CompareTo(other.Steps);
                if (comp != 0) { return comp; }

                return (other.X + other.Y).CompareTo(X + Y);
            }
            public override string ToString() {
                return $"({X},{Y})={Value}";
            }
        }
    }
}