using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Donut Maze")]
    public class Puzzle20 : ASolver {
        private Dictionary<Point, Point> map;
        private Point start, end;
        private int width, height;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);

            width = items[0].Length;
            height = items.Count;

            Dictionary<(char, char), Point> teleporters = new Dictionary<(char, char), Point>();
            map = new Dictionary<Point, Point>();
            HashSet<int> used = new HashSet<int>();
            Point pos = new Point();

            for (int i = 0; i < height; i++) {
                pos.Y = i;
                string line = items[i];

                for (int j = 0; j < width; j++) {
                    pos.X = j;
                    char c = line[j];

                    if (char.IsLetterOrDigit(c) && used.Add(i * width + j)) {
                        char right = line[j + 1];
                        char down = items[i + 1][j];

                        if (char.IsLetterOrDigit(right)) {
                            used.Add(i * width + j + 1);
                            if (j > 0 && line[j - 1] == '.') {
                                Point point = pos + Point.WEST;
                                AddTeleport(teleporters, c, right, point);
                            } else {
                                Point point = pos + Point.EAST + Point.EAST;
                                AddTeleport(teleporters, c, right, point);
                            }
                        } else if (char.IsLetterOrDigit(down)) {
                            used.Add((i + 1) * width + j);
                            if (i > 0 && items[i - 1][j] == '.') {
                                Point point = pos + Point.NORTH;
                                AddTeleport(teleporters, c, down, point);
                            } else {
                                Point point = pos + Point.SOUTH + Point.SOUTH;
                                AddTeleport(teleporters, c, down, point);
                            }
                        }
                    } else if (c == '.' && !map.ContainsKey(pos)) {
                        map[pos] = pos;
                    }
                }
            }

            start = teleporters[('A', 'A')];
            end = teleporters[('Z', 'Z')];
        }
        private void AddTeleport(Dictionary<(char, char), Point> teleporters, char first, char second, Point point) {
            if (!teleporters.TryGetValue((first, second), out Point teleport)) {
                teleporters[(first, second)] = point;
                map[point] = point;
            } else {
                map[point] = teleport;
                map[teleport] = point;
            }
        }

        [Description("How many steps does it take to get from AA to ZZ?")]
        public override string SolvePart1() {
            Point[] dirs = new Point[] { Point.NORTH, Point.SOUTH, Point.EAST, Point.WEST };
            HashSet<Point> closed = new HashSet<Point>();
            Queue<(Point, int)> open = new Queue<(Point, int)>();

            closed.Add(start);
            open.Enqueue((start, 0));
            while (open.Count > 0) {
                (Point point, int steps) = open.Dequeue();

                if (point == end) {
                    return $"{steps}";
                }

                Point teleport = map[point];
                if (teleport != point && closed.Add(teleport)) {
                    open.Enqueue((teleport, steps + 1));
                } else {
                    for (int i = 0; i < dirs.Length; i++) {
                        Point dir = point + dirs[i];

                        if (map.ContainsKey(dir) && closed.Add(dir)) {
                            open.Enqueue((dir, steps + 1));
                        }
                    }
                }
            }

            return string.Empty;
        }

        [Description("How many steps does it take to get from AA to ZZ?")]
        public override string SolvePart2() {
            Point[] dirs = new Point[] { Point.NORTH, Point.SOUTH, Point.EAST, Point.WEST };
            HashSet<(Point, int)> closed = new HashSet<(Point, int)>();
            Queue<(Point, int, int)> open = new Queue<(Point, int, int)>();

            closed.Add((start, 0));
            open.Enqueue((start, 0, 0));

            while (open.Count > 0) {
                (Point point, int level, int steps) = open.Dequeue();

                if (point == end && level == 0) {
                    return $"{steps}";
                }

                bool isOuter = point.X == 2 || point.Y == 2 || point.X == width - 3 || point.Y == height - 3;
                Point teleport = map[point];
                int nextLevel = level + (isOuter ? -1 : 1);

                if (teleport != point && (!isOuter || level > 0) && closed.Add((teleport, nextLevel))) {
                    open.Enqueue((teleport, nextLevel, steps + 1));
                } else {
                    for (int i = 0; i < dirs.Length; i++) {
                        Point dir = point + dirs[i];

                        if (map.ContainsKey(dir) && closed.Add((dir, level))) {
                            open.Enqueue((dir, level, steps + 1));
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}