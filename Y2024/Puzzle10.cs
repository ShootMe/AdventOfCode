using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Hoof It")]
    public class Puzzle10 : ASolver {
        private byte[,] map;
        private int width, height;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            height = lines.Length;
            width = lines[0].Length;
            map = new byte[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    map[y, x] = (byte)(c - 0x30);
                }
            }
        }

        [Description("What is the sum of the scores of all trailheads on your topographic map?")]
        public override string SolvePart1() {
            int total = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    byte value = map[y, x];
                    if (value == 0) {
                        total += FindTrailHeadScore(x, y);
                    }
                }
            }
            return $"{total}";
        }
        private int FindTrailHeadScore(int x, int y, bool allowAll = false) {
            HashSet<(int, int)> seen = new();
            Queue<(int, int)> open = new();
            open.Enqueue((x, y));
            void AddNext(int x, int y, int next) {
                if (x < 0 || x >= width || y < 0 || y >= height || map[y, x] != next || (!allowAll && !seen.Add((x, y)))) { return; }
                open.Enqueue((x, y));
            }

            int total = 0;
            while (open.Count > 0) {
                (int cx, int cy) = open.Dequeue();
                int value = map[cy, cx] + 1;
                if (value == 10) {
                    total++;
                    continue;
                }

                AddNext(cx - 1, cy, value);
                AddNext(cx + 1, cy, value);
                AddNext(cx, cy - 1, value);
                AddNext(cx, cy + 1, value);
            }
            return total;
        }

        [Description("What is the sum of the ratings of all trailheads?")]
        public override string SolvePart2() {
            int total = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    byte value = map[y, x];
                    if (value == 0) {
                        total += FindTrailHeadScore(x, y, true);
                    }
                }
            }
            return $"{total}";
        }
    }
}