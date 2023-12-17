using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Clumsy Crucible")]
    public class Puzzle17 : ASolver {
        private byte[,] grid;
        private int width, height;
        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;
            grid = new byte[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    grid[y, x] = (byte)(line[x] - '0');
                }
            }
        }

        [Description("Directing the crucible from the lava pool to the machine parts factory, but not moving more than three consecutive blocks in the same direction, what is the least heat loss it can incur?")]
        public override string SolvePart1() {
            return $"{FindLowestHeatLost(0, 0, false)}";
        }

        [Description("Directing the ultra crucible from the lava pool to the machine parts factory, what is the least heat loss it can incur?")]
        public override string SolvePart2() {
            return $"{FindLowestHeatLost(0, 0, true)}";
        }

        private int FindLowestHeatLost(int xs, int ys, bool ultra) {
            Queue<Crucible> open = new();
            HashSet<Crucible> closed = new(height * width * 2);
            open.Enqueue(new Crucible() { X = (short)xs, Y = (short)ys, DX = 1, DY = 0, Straight = 0 });
            open.Enqueue(new Crucible() { X = (short)xs, Y = (short)ys, DX = 0, DY = 1, Straight = 0 });

            int min = int.MaxValue;
            while (open.Count > 0) {
                Crucible crucible = open.Dequeue();
                if (crucible.X == width - 1 && crucible.Y == height - 1) {
                    if (min > crucible.Loss) {
                        min = crucible.Loss;
                    }
                    continue;
                }

                if ((ultra && crucible.Straight < 10) || (!ultra && crucible.Straight < 3)) {
                    Crucible next = crucible.NextStraight;
                    TryAdd(next, open, closed);
                }

                if (!ultra || crucible.Straight >= 4) {
                    Crucible nextRight = crucible.NextRight;
                    TryAdd(nextRight, open, closed);

                    Crucible nextLeft = crucible.NextLeft;
                    TryAdd(nextLeft, open, closed);
                }
            }

            return min;
        }
        private void TryAdd(Crucible next, Queue<Crucible> open, HashSet<Crucible> closed) {
            if (next.IsValid(width, height)) {
                next.Loss += grid[next.Y, next.X];
                if (!closed.TryGetValue(next, out Crucible existing) || existing.Loss > next.Loss) {
                    if (existing != null) {
                        existing.Loss = next.Loss;
                    } else {
                        closed.Add(next);
                    }
                    open.Enqueue(next);
                }
            }
        }

        private class Crucible : IEquatable<Crucible> {
            public short X, Y, DX, DY, Straight;
            public int Loss;
            public bool IsValid(int width, int height) {
                return X >= 0 && Y >= 0 && X < width && Y < height;
            }
            public Crucible NextStraight {
                get {
                    return new Crucible() { X = (short)(X + DX), Y = (short)(Y + DY), DX = DX, DY = DY, Straight = (short)(Straight + 1), Loss = Loss };
                }
            }
            public Crucible NextRight {
                get {
                    return new Crucible() { X = (short)(X - DY), Y = (short)(Y + DX), DX = (short)-DY, DY = DX, Straight = 1, Loss = Loss };
                }
            }
            public Crucible NextLeft {
                get {
                    return new Crucible() { X = (short)(X + DY), Y = (short)(Y - DX), DX = DY, DY = (short)-DX, Straight = 1, Loss = Loss };
                }
            }
            public bool Equals(Crucible other) {
                return X == other.X && Y == other.Y && DX == other.DX && DY == other.DY && Straight == other.Straight;
            }
            public override int GetHashCode() {
                return (X << 16) | (Y << 8) | ((DY + 1) << 6) | ((DX + 1) << 4) | (int)Straight;
            }
            public override string ToString() {
                return $"{X},{Y},{DX},{DY},{Straight},{Loss}";
            }
        }
    }
}