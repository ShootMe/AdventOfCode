using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Rope Bridge")]
    public class Puzzle09 : ASolver {
        [Description("How many positions does the tail of the rope visit at least once?")]
        public override string SolvePart1() {
            return $"{CountPositions(2)}";
        }

        [Description("How many positions does the tail of the rope visit at least once?")]
        public override string SolvePart2() {
            return $"{CountPositions(10)}";
        }

        private int CountPositions(int knots) {
            HashSet<(int x, int y)> positions = new();
            List<(int x, int y)> rope = new();
            for (int i = 0; i < knots; i++) {
                rope.Add((0, 0));
            }

            foreach (string line in Input.Split('\n')) {
                int amount = line[2..].ToInt();
                int xd = 0;
                int yd = 0;
                switch (line[0]) {
                    case 'R': xd = 1; break;
                    case 'L': xd = -1; break;
                    case 'U': yd = 1; break;
                    case 'D': yd = -1; break;
                }

                while (amount-- > 0) {
                    rope[0] = (rope[0].x + xd, rope[0].y + yd);

                    for (int i = 1; i < rope.Count; i++) {
                        if (Math.Abs(rope[i].x - rope[i - 1].x) > 1 || Math.Abs(rope[i].y - rope[i - 1].y) > 1) {
                            rope[i] = (rope[i].x + Math.Sign(rope[i - 1].x - rope[i].x), rope[i].y + Math.Sign(rope[i - 1].y - rope[i].y));
                        }
                    }

                    positions.Add(rope[^1]);
                }
            }
            return positions.Count;
        }
    }
}