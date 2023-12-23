using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Sand Slabs")]
    public class Puzzle22 : ASolver {
        private List<Brick> bricks = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                bricks.Add(new Brick(lines[i], i));
            }

            bricks.Sort();
            Brick[,] zMax = new Brick[10, 10];
            for (int i = 0; i < bricks.Count; i++) {
                MoveBrickDown(zMax, bricks[i]);
            }
            bricks.Sort();
        }
        private void MoveBrickDown(Brick[,] zMax, Brick brick) {
            int maxZ = 0;
            for (int y = brick.End1.y; y <= brick.End2.y; y++) {
                for (int x = brick.End1.x; x <= brick.End2.x; x++) {
                    Brick max = zMax[y, x];
                    if (max != null && max.End2.z > maxZ) { maxZ = max.End2.z; }
                }
            }

            for (int y = brick.End1.y; y <= brick.End2.y; y++) {
                for (int x = brick.End1.x; x <= brick.End2.x; x++) {
                    Brick max = zMax[y, x];
                    zMax[y, x] = brick;
                    if (max != null && max.End2.z == maxZ) {
                        brick.Below.Add(max);
                        max.Above.Add(brick);
                    }
                }
            }

            int diff = brick.End1.z - maxZ - 1;
            brick.End1.z -= diff;
            brick.End2.z -= diff;
        }

        [Description("How many bricks could be safely chosen as the one to get disintegrated?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < bricks.Count; i++) {
                if (CanRemoveBrick(bricks[i])) { total++; }
            }
            return $"{total}";
        }

        [Description("What is the sum of the number of other bricks that would fall?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < bricks.Count; i++) {
                total += BricksToFallWhenRemoved(bricks[i]);
            }
            return $"{total}";
        }

        private bool CanRemoveBrick(Brick brick) {
            if (brick.Above.Count == 0) { return true; }

            foreach (Brick above in brick.Above) {
                if (above.Below.Count == 1) { return false; }
            }
            return true;
        }
        private int BricksToFallWhenRemoved(Brick brick) {
            if (brick.Above.Count == 0) { return 0; }

            Queue<Brick> bricksLeft = new();
            bricksLeft.Enqueue(brick);
            int total = 0;
            bool[] removed = new bool[bricks.Count];
            removed[brick.ID] = true;

            while (bricksLeft.Count > 0) {
                Brick current = bricksLeft.Dequeue();
                foreach (Brick above in current.Above) {
                    if (removed[above.ID]) { continue; }

                    bool willFall = true;
                    foreach (Brick below in above.Below) {
                        if (!removed[below.ID]) {
                            willFall = false;
                            break;
                        }
                    }

                    if (willFall) {
                        removed[above.ID] = true;
                        bricksLeft.Enqueue(above);
                        total++;
                    }
                }
            }

            return total;
        }
        private class Brick : IComparable<Brick>, IEquatable<Brick> {
            public int ID;
            public (int x, int y, int z) End1, End2;
            public HashSet<Brick> Below = new();
            public HashSet<Brick> Above = new();

            public Brick(string line, int id) {
                ID = id;
                string[] splits = line.SplitOn(",", ",", "~", ",", ",");
                End1 = (splits[0].ToInt(), splits[1].ToInt(), splits[2].ToInt());
                End2 = (splits[3].ToInt(), splits[4].ToInt(), splits[5].ToInt());
            }
            public int CompareTo(Brick other) {
                int comp = End1.z.CompareTo(other.End1.z);
                if (comp != 0) { return comp; }
                comp = End1.x.CompareTo(other.End1.x);
                if (comp != 0) { return comp; }
                return End1.y.CompareTo(other.End1.y);
            }
            public override string ToString() { return $"{End1} - {End2}"; }
            public bool Equals(Brick other) { return ID == other.ID; }
            public override int GetHashCode() { return ID; }
        }
    }
}