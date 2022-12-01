using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Settlers of The North Pole")]
    public class Puzzle18 : ASolver {
        private byte[] lumberArea, state;
        private int width, height;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);

            width = items[0].Length;
            height = items.Count;
            lumberArea = new byte[width * height];
            int index = 0;

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                for (int j = 0; j < item.Length; j++) {
                    lumberArea[index++] = item[j] == '#' ? (byte)16 : item[j] == '|' ? (byte)1 : (byte)0;
                }
            }

            state = new byte[lumberArea.Length];
            Array.Copy(lumberArea, state, lumberArea.Length);
        }

        [Description("What will the total value of the lumber collection area be after 10 minutes?")]
        public override string SolvePart1() {
            byte[] original = new byte[lumberArea.Length];
            Array.Copy(lumberArea, original, lumberArea.Length);

            for (int i = 0; i < 10; i++) {
                Step();
            }

            int trees = 0;
            int lumberyards = 0;
            for (int i = 0; i < lumberArea.Length; i++) {
                switch (lumberArea[i]) {
                    case 1: trees++; break;
                    case 16: lumberyards++; break;
                }
            }

            Array.Copy(original, lumberArea, lumberArea.Length);
            return $"{trees * lumberyards}";
        }

        [Description("What will the total value of the lumber collection area be after 1000000000 minutes?")]
        public override string SolvePart2() {
            Dictionary<int, int> seenValues = new Dictionary<int, int>();
            int lastSeen = 0;
            int lastValue = 0;
            int minutes = 1000000000;
            for (int i = 0; i < minutes; i++) {
                Step();

                int trees = 0;
                int lumberyards = 0;
                for (int j = 0; j < lumberArea.Length; j++) {
                    switch (lumberArea[j]) {
                        case 1: trees++; break;
                        case 16: lumberyards++; break;
                    }
                }

                int newValue = trees * lumberyards;
                int lastIndex;
                if (seenValues.TryGetValue(newValue, out lastIndex)) {
                    if (lastSeen == i - lastIndex) {
                        minutes = ((minutes - i) % lastSeen) + i;
                    }
                    lastSeen = i - lastIndex;
                }
                seenValues[newValue] = i;
                lastValue = newValue;
            }

            return $"{lastValue}";
        }

        private void Step() {
            int x = 0;
            int y = 0;
            for (int i = 0; i < lumberArea.Length; i++) {
                byte tile = lumberArea[i];

                byte value = GetTile(x - 1, y - 1);
                value += GetTile(x - 1, y);
                value += GetTile(x - 1, y + 1);
                value += GetTile(x, y - 1);
                value += GetTile(x, y + 1);
                value += GetTile(x + 1, y - 1);
                value += GetTile(x + 1, y);
                value += GetTile(x + 1, y + 1);

                int trees = value & 0xf;
                int lumberyards = value >> 4;

                if (tile == 0 && trees >= 3) {
                    state[i] = 1;
                } else if (tile == 1 && lumberyards >= 3) {
                    state[i] = 16;
                } else if (tile == 16 && (lumberyards == 0 || trees == 0)) {
                    state[i] = 0;
                } else {
                    state[i] = tile;
                }

                x++;
                if (x == width) {
                    x = 0;
                    y++;
                }
            }

            byte[] temp = state;
            state = lumberArea;
            lumberArea = temp;
        }
        private byte GetTile(int x, int y) {
            if (x < 0 || y < 0 || x >= width || y >= height) { return 0; }
            return lumberArea[y * width + x];
        }
    }
}