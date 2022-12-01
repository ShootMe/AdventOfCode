using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Dumbo Octopus")]
    public class Puzzle11 : ASolver {
        private byte[] octopuses;
        private int width, height;
        public override void Setup() {
            List<string> items = Tools.GetLines(Input);

            width = items[0].Length;
            height = items.Count;
            octopuses = new byte[width * height];
            for (int i = 0, p = 0; i < items.Count; i++) {
                string item = items[i];
                for (int j = 0; j < item.Length; j++) {
                    octopuses[p++] = (byte)(item[j] - '0');
                }
            }
        }

        [Description("How many total flashes are there after 100 steps?")]
        public override string SolvePart1() {
            int flashes = 0;
            for (int i = 0; i < 100; i++) {
                flashes += Step();
            }
            return $"{flashes}";
        }

        [Description("What is the first step during which all octopuses flash?")]
        public override string SolvePart2() {
            Setup();
            int stepCount = 0;
            while (true) {
                stepCount++;
                if (Step() == octopuses.Length) {
                    return $"{stepCount}";
                }
            }
        }

        private int Step() {
            Span<int> points = stackalloc int[octopuses.Length];
            int index = 0;
            for (int pos = 0; pos < octopuses.Length; pos++) {
                ref byte value = ref octopuses[pos];
                value &= 0xf;
                if (++value > 9) {
                    points[index++] = pos;
                    value = 16;
                }
            }

            int flashCount = 0;
            while (flashCount < index) {
                int pos = points[flashCount++];
                int x = pos % width;

                pos -= width;
                if (pos >= 0) {
                    if (x > 0 && CheckPosition(pos - 1)) { points[index++] = pos - 1; }
                    if (CheckPosition(pos)) { points[index++] = pos; }
                    if (x + 1 < width && CheckPosition(pos + 1)) { points[index++] = pos + 1; }
                }
                pos += width;
                if (x > 0 && CheckPosition(pos - 1)) { points[index++] = pos - 1; }
                if (x + 1 < width && CheckPosition(pos + 1)) { points[index++] = pos + 1; }
                pos += width;
                if (pos < octopuses.Length) {
                    if (x > 0 && CheckPosition(pos - 1)) { points[index++] = pos - 1; }
                    if (CheckPosition(pos)) { points[index++] = pos; }
                    if (x + 1 < width && CheckPosition(pos + 1)) { points[index++] = pos + 1; }
                }
            }

            return flashCount;
        }
        private bool CheckPosition(int pos) {
            ref byte value = ref octopuses[pos];
            if (value != 16 && ++value > 9) {
                value = 16;
                return true;
            }
            return false;
        }
    }
}