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
            (int x, int y)[] rope = new (int x, int y)[knots];

            foreach (string line in Input.Split('\n')) {
                int amount = line[2..].ToInt();
                int xd = line[0] switch { 'R' => 1, 'L' => -1, _ => 0 };
                int yd = line[0] switch { 'U' => 1, 'D' => -1, _ => 0 };

                while (amount-- > 0) {
                    rope[0] = (rope[0].x + xd, rope[0].y + yd);

                    for (int i = 1; i < rope.Length; i++) {
                        int diffX = rope[i - 1].x - rope[i].x;
                        int diffY = rope[i - 1].y - rope[i].y;
                        if (Math.Abs(diffX) > 1 || Math.Abs(diffY) > 1) {
                            rope[i] = (rope[i].x + Math.Sign(diffX), rope[i].y + Math.Sign(diffY));
                        }
                    }

                    positions.Add(rope[^1]);
                }
            }
            return positions.Count;
        }
    }
}