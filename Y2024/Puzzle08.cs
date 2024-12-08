using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Resonant Collinearity")]
    public class Puzzle08 : ASolver {
        private int width, height;
        private Dictionary<char, List<(int, int)>> nodes = new();
        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    if (c != '.') {
                        List<(int, int)> list;
                        if (!nodes.TryGetValue(c, out list)) {
                            list = new();
                            nodes.Add(c, list);
                        }
                        list.Add((x, y));
                    }
                }
            }
        }

        [Description("How many unique locations within the bounds of the map contain an antinode?")]
        public override string SolvePart1() {
            return $"{CountAntiNodes(false)}";
        }

        [Description("How many unique locations within the bounds of the map contain an antinode?")]
        public override string SolvePart2() {
            return $"{CountAntiNodes(true)}";
        }
        private int CountAntiNodes(bool addResonant) {
            HashSet<(int, int)> antiNodes = new();
            void MarkUnique(int x, int diffX, int y, int diffY) {
                while (x >= 0 && x < width && y >= 0 && y < height) {
                    antiNodes.Add((x, y));
                    x -= diffX; y -= diffY;
                }
            }
            foreach (KeyValuePair<char, List<(int, int)>> pair in nodes) {
                List<(int, int)> node = pair.Value;
                for (int i = 0; i < node.Count; i++) {
                    (int x1, int y1) = node[i];
                    for (int j = i + 1; j < node.Count; j++) {
                        (int x2, int y2) = node[j];
                        if (addResonant) {
                            MarkUnique(x1, x2 - x1, y1, y2 - y1);
                            MarkUnique(x2, x1 - x2, y2, y1 - y2);
                        } else {
                            MarkUnique(x1 - (x2 - x1), width, y1 - (y2 - y1), height);
                            MarkUnique(x2 - (x1 - x2), width, y2 - (y1 - y2), height);
                        }
                    }
                }
            }
            return antiNodes.Count;
        }
    }
}