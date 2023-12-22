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
                bricks.Add(new Brick(lines[i]));
            }
            bricks.Sort();
        }

        [Description("How many bricks could be safely chosen as the one to get disintegrated?")]
        public override string SolvePart1() {
            for (int i = 0; i < bricks.Count; i++) {
                Brick brick = bricks[i];
                while (MoveBrickDown(i, brick)) { }
                brick.SaveState();
            }

            int total = 0;
            for (int i = 0; i < bricks.Count; i++) {
                if (BricksToFall(bricks[i]) == 0) { total++; }
            }
            return $"{total}";
        }

        [Description("What is the sum of the number of other bricks that would fall?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < bricks.Count; i++) {
                total += BricksToFall(bricks[i]);
            }
            return $"{total}";
        }
        private int BricksToFall(Brick brick) {
            brick.Disabled = true;

            int count = 0;
            for (int i = 0; i < bricks.Count; i++) {
                Brick test = bricks[i];
                if (test.Disabled) { continue; }
                if (MoveBrickDown(i, test)) { count++; }
            }

            for (int i = 0; i < bricks.Count; i++) {
                bricks[i].RestoreState();
            }
            brick.Disabled = false;
            return count;
        }
        private bool MoveBrickDown(int index, Brick brick) {
            if (brick.End1.z <= 1 || brick.End2.z <= 1) { return false; }

            brick.End1.z--;
            brick.End2.z--;
            for (int i = index - 1; i >= 0; i--) {
                Brick other = bricks[i];
                if (other.Intersects(brick)) {
                    brick.End1.z++;
                    brick.End2.z++;
                    return false;
                }
            }
            return true;
        }
        private class Brick : IComparable<Brick> {
            public (int x, int y, int z) End1, End2;
            private (int x, int y, int z) End1Save, End2Save;
            public bool Disabled;
            public Brick(string line) {
                string[] splits = line.SplitOn(",", ",", "~", ",", ",");
                End1 = (splits[0].ToInt(), splits[1].ToInt(), splits[2].ToInt());
                End2 = (splits[3].ToInt(), splits[4].ToInt(), splits[5].ToInt());
                End1Save = End1;
                End2Save = End2;
            }
            public void SaveState() {
                End1Save = End1;
                End2Save = End2;
            }
            public void RestoreState() {
                End1 = End1Save;
                End2 = End2Save;
            }
            public bool Intersects(Brick other) {
                return !Disabled && !other.Disabled &&
                    (End2.z >= other.End1.z && End1.z <= other.End2.z) &&
                    (End2.x >= other.End1.x && End1.x <= other.End2.x) &&
                    (End2.y >= other.End1.y && End1.y <= other.End2.y);
            }
            public int CompareTo(Brick other) {
                int comp = End1.z.CompareTo(other.End1.z);
                if (comp != 0) { return comp; }
                comp = End1.x.CompareTo(other.End1.x);
                if (comp != 0) { return comp; }
                return End1.y.CompareTo(other.End1.y);
            }
            public override string ToString() {
                return $"{End1} - {End2}";
            }
        }
    }
}