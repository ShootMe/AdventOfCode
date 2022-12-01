using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Chiton")]
    public class Puzzle15 : ASolver {
        private int[,] riskLevels;
        private int width, height;
        private (int X, int Y) endPosition;
        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            width = items[0].Length;
            height = items.Count;
            riskLevels = new int[width, height];
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                for (int j = 0; j < width; j++) {
                    riskLevels[j, i] = item[j] - '0';
                }
            }

            endPosition = (width - 1, height - 1);
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart1() {
            return $"{FindPath(false)}";
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart2() {
            endPosition = (width * 5 - 1, height * 5 - 1);
            return $"{FindPath(true)}";
        }

        private int FindPath(bool multiLevel) {
            (int, int)[] dirs = { (0, 1), (1, 0), (-1, 0), (0, -1) };
            Heap<Path> queue = new Heap<Path>();
            HashSet<(int, int)> seen = new HashSet<(int, int)>(endPosition.X * endPosition.Y);
            queue.Enqueue(new Path());
            seen.Add((0, 0));

            while (queue.Count > 0) {
                Path current = queue.Dequeue();

                if (current.X == endPosition.X && current.Y == endPosition.Y) {
                    return current.Risk;
                }

                foreach ((int x, int y) in dirs) {
                    int tX = current.X + x; int tY = current.Y + y;
                    if (tX < 0 || tY < 0 || tX > endPosition.X || tY > endPosition.Y) { continue; }

                    int risk = multiLevel ? GetRiskLevel(tX, tY) : riskLevels[tX, tY];

                    if (seen.Add((tX, tY))) {
                        queue.Enqueue(new Path() { X = tX, Y = tY, Risk = current.Risk + risk });
                    }
                }
            }

            return 0;
        }

        private int GetRiskLevel(int x, int y) {
            int risk = riskLevels[x % width, y % height];
            int extra = x / width + y / height;
            risk += extra;
            if (risk > 9) { risk -= 9; }
            return risk;
        }

        private struct Path : IComparable<Path> {
            public int X, Y, Risk;

            public int CompareTo(Path other) {
                int comp = Risk.CompareTo(other.Risk);
                if (comp != 0) { return comp; }

                return (other.X + other.Y).CompareTo(X + Y);
            }
            public override string ToString() {
                return $"({X},{Y})={Risk}";
            }
        }
    }
}