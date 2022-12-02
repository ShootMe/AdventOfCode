using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Lobby Layout")]
    public class Puzzle24 : ASolver {
        private Dictionary<Point, bool> tiles, state;

        public override void Setup() {
            List<string> items = Input.Lines();
            tiles = new Dictionary<Point, bool>(15000);
            for (int i = 0; i < items.Count; i++) {
                Point point = GetPosition(items[i]);
                bool flipped;
                if (!tiles.TryGetValue(point, out flipped)) {
                    tiles[point] = false;
                }
                tiles[point] = !flipped;
            }
        }

        [Description("How many tiles are left with the black side up?")]
        public override string SolvePart1() {
            int count = 0;
            foreach (bool flipped in tiles.Values) {
                if (flipped) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("How many tiles will be black after 100 days?")]
        public override string SolvePart2() {
            state = new Dictionary<Point, bool>(tiles.Count);
            for (int i = 0; i < 100; i++) {
                Step();
            }

            int count = 0;
            foreach (bool flipped in tiles.Values) {
                if (flipped) {
                    count++;
                }
            }
            return $"{count}";
        }

        private void Step() {
            foreach (KeyValuePair<Point, bool> pair in tiles) {
                Point point = pair.Key;

                int count = GetFlipCount(state, point, true);

                if (pair.Value && (count == 0 || count > 2)) {
                    state[point] = false;
                } else if (!pair.Value && count == 2) {
                    state[point] = true;
                } else {
                    state[point] = pair.Value;
                }
            }

            Dictionary<Point, bool> temp = tiles;
            tiles = state;
            state = temp;
        }
        private int GetFlipCount(Dictionary<Point, bool> state, Point point, bool recursive = false) {
            int count = GetFlipped(state, point, -1, -1, recursive);
            count += GetFlipped(state, point, -1, 0, recursive);
            count += GetFlipped(state, point, 0, -1, recursive);
            count += GetFlipped(state, point, 0, 1, recursive);
            count += GetFlipped(state, point, 1, 0, recursive);
            count += GetFlipped(state, point, 1, 1, recursive);
            return count;
        }
        private int GetFlipped(Dictionary<Point, bool> state, Point point, int xMove, int yMove, bool recursive) {
            Point next = new Point() { X = point.X + xMove, Y = point.Y + yMove };
            bool flipped;
            if (tiles.TryGetValue(next, out flipped)) {
                return flipped ? 1 : 0;
            }

            if (recursive && !state.ContainsKey(next)) {
                int count = GetFlipCount(state, next);
                if (count == 2) {
                    state[next] = true;
                }
            }

            return 0;
        }
        private Point GetPosition(string directions) {
            int x = 0;
            int y = 0;

            for (int i = 0; i < directions.Length; i++) {
                char dir = directions[i];
                char next = i + 1 < directions.Length ? directions[i + 1] : '\0';
                if (dir == 'n') {
                    if (next == 'w') {
                        x--;
                    }
                    y--;
                    i++;
                } else if (dir == 's') {
                    if (next == 'e') {
                        x++;
                    }
                    y++;
                    i++;
                } else if (dir == 'e') {
                    x++;
                } else if (dir == 'w') {
                    x--;
                }
            }
            return new Point() { X = x, Y = y };
        }
    }
}