using System;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Laboratories")]
    public class Puzzle07 : ASolver {
        private string[] space;
        private int width, height;
        private (int x, int y) start;

        public override void Setup() {
            space = Input.Split('\n');
            width = space[0].Length;
            height = space.Length;
            for (int y = 0; y < height; y++) {
                string line = space[y];
                for (int x = 0; x < width; x++) {
                    if (line[x] == 'S') {
                        start = (x, y);
                    }
                }
            }
        }

        [Description("How many times will the beam be split?")]
        public override string SolvePart1() {
            int total = 0;
            bool[] current = new bool[width];
            bool[] next = new bool[width];
            current[start.x] = true;

            for (int y = 1; y < height; y++) {
                string line = space[y];
                for (int x = 0; x < width; x++) {
                    if (!current[x]) {
                        continue;
                    } else if (line[x] == '.') {
                        next[x] = true;
                    } else {
                        next[x - 1] = true;
                        next[x + 1] = true;
                        total++;
                    }
                }

                Array.Clear(current);
                Extensions.Swap(ref current, ref next);
            }
            return $"{total}";
        }

        [Description("In total, how many different timelines would a single tachyon particle end up on?")]
        public override string SolvePart2() {
            long[] current = new long[width];
            long[] next = new long[width];
            current[start.x] = 1;

            for (int y = 1; y < height; y++) {
                string line = space[y];
                for (int x = 0; x < width; x++) {
                    if (current[x] == 0) {
                        continue;
                    } else if (line[x] == '.') {
                        next[x] += current[x];
                    } else {
                        next[x - 1] += current[x];
                        next[x + 1] += current[x];
                    }
                }

                Array.Clear(current);
                Extensions.Swap(ref current, ref next);
            }

            long total = 0;
            for (int x = 0; x < width; x++) {
                total += current[x];
            }
            return $"{total}";
        }
    }
}