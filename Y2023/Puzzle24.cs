using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Never Tell Me The Odds")]
    public class Puzzle24 : ASolver {
        private List<Snowball> snowballs = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                snowballs.Add(new Snowball(lines[i], i));
            }
        }

        [Description("How many of these intersections occur within the test area?")]
        public override string SolvePart1() {
            int total = 0;
            long min = snowballs.Count > 10 ? 200000000000000 : 7;
            long max = snowballs.Count > 10 ? 400000000000000 : 27;
            for (int i = 0; i < snowballs.Count; i++) {
                Snowball snowball1 = snowballs[i];
                for (int j = i + 1; j < snowballs.Count; j++) {
                    var sx = snowball1.IntersectsXY(snowballs[j]);
                    if (!sx.Intersects || sx.Position.X < min || sx.Position.X > max || sx.Position.Y < min || sx.Position.Y > max || sx.Time1 < 0 || sx.Time2 < 0) { continue; }

                    total++;
                }
            }
            return $"{total}";
        }

        [Description("What do you get if you add up the X, Y, and Z coordinates of that initial position?")]
        public override string SolvePart2() {
            const int vRange = 350;

            Snowball s1 = snowballs[0]; s1.Velocity.x -= vRange;
            Snowball s2 = snowballs[1]; s2.Velocity.x -= vRange;
            Snowball s3 = snowballs[2]; s3.Velocity.x -= vRange;
            Snowball s4 = snowballs[3]; s4.Velocity.x -= vRange;
            long y1 = s1.Velocity.y; long y2 = s2.Velocity.y; long y3 = s3.Velocity.y; long y4 = s4.Velocity.y;

            for (int x = -vRange; x <= vRange; x++, s1.Velocity.x++, s2.Velocity.x++, s3.Velocity.x++, s4.Velocity.x++) {
                s1.Velocity.y = y1 - vRange; s2.Velocity.y = y2 - vRange; s3.Velocity.y = y3 - vRange; s4.Velocity.y = y4 - vRange;

                for (int y = -vRange; y <= vRange; y++, s1.Velocity.y++, s2.Velocity.y++, s3.Velocity.y++, s4.Velocity.y++) {
                    var i1 = s1.IntersectsXY(s2);
                    if (!i1.Intersects) { continue; }
                    var i2 = s2.IntersectsXY(s3);
                    if (i1.Position != i2.Position) { continue; }
                    var i3 = s3.IntersectsXY(s4);
                    if (i1.Position != i3.Position) { continue; }

                    for (int z = -vRange; z <= vRange; z++) {
                        long z1 = s1.ZAt(i1.Time1, z);
                        long z2 = s2.ZAt(i2.Time1, z);
                        if (z1 != z2) { continue; }
                        long z3 = s3.ZAt(i3.Time1, z);
                        if (z1 != z3) { continue; }

                        return $"{i1.Position.X + i1.Position.Y + z1}";
                    }
                }
            }
            return string.Empty;
        }
        private class Snowball : IComparable<Snowball>, IEquatable<Snowball> {
            public int ID;
            public (long x, long y, long z) Position, Velocity;

            public Snowball(string line, int id) {
                ID = id;
                string[] splits = line.SplitOn(", ", ", ", " @ ", ", ", ", ");
                Position = (splits[0].ToLong(), splits[1].ToLong(), splits[2].ToLong());
                Velocity = (splits[3].ToLong(), splits[4].ToLong(), splits[5].ToLong());
            }
            public Snowball(Snowball copy) {
                ID = copy.ID;
                Position = copy.Position;
                Velocity = copy.Velocity;
            }
            public (bool Intersects, (long X, long Y) Position, long Time1, long Time2) IntersectsXY(Snowball s2) {
                double d = (double)Velocity.y * s2.Velocity.x - Velocity.x * s2.Velocity.y;
                if (d == 0 || Velocity.x == 0 || s2.Velocity.x == 0) { return (false, (0, 0), 0, 0); }

                double m1 = (double)Velocity.y / Velocity.x;
                double m2 = (double)s2.Velocity.y / s2.Velocity.x;
                double x = ((Position.y - m1 * Position.x) - (s2.Position.y - m2 * s2.Position.x)) / (m2 - m1);
                double y = (Position.y - m1 * Position.x) + m1 * x;

                return (true, ((long)x, (long)y), (long)((x - Position.x) / Velocity.x), (long)((x - s2.Position.x) / s2.Velocity.x));
            }
            public long ZAt(double time, int offset) {
                return (long)(Position.z + (Velocity.z + offset) * time);
            }
            public int CompareTo(Snowball other) {
                int comp = Position.x.CompareTo(other.Position.x);
                if (comp != 0) { return comp; }
                comp = Position.y.CompareTo(other.Position.y);
                if (comp != 0) { return comp; }
                return Position.z.CompareTo(other.Position.z);
            }
            public override string ToString() { return $"{Position} - {Velocity}"; }
            public bool Equals(Snowball other) { return ID == other.ID; }
            public override int GetHashCode() { return ID; }
        }
    }
}