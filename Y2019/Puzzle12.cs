using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("The N-Body Problem")]
    public class Puzzle12 : ASolver {
        private Moon[] moons;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            moons = new Moon[items.Count];
            for (int i = 0; i < items.Count; i++) {
                moons[i] = new Moon(items[i]);
            }
        }

        [Description("What is the total energy in the system after simulating for 1000 steps?")]
        public override string SolvePart1() {
            Moon[] temp = new Moon[moons.Length];
            Array.Copy(moons, temp, moons.Length);

            for (int i = 0; i < 1000; i++) {
                for (int j = 0; j < temp.Length; j++) {
                    ref Moon moon = ref temp[j];
                    for (int k = j + 1; k < temp.Length; k++) {
                        moon.ApplyGravity(ref temp[k]);
                    }
                }

                for (int j = 0; j < temp.Length; j++) {
                    temp[j].ApplyVelocity();
                }
            }

            int totalEnergy = 0;
            for (int j = 0; j < temp.Length; j++) {
                totalEnergy += temp[j].TotalEnergy();
            }
            return $"{totalEnergy}";
        }

        [Description("How many steps does it take to reach the first state that exactly matches a previous state?")]
        public override string SolvePart2() {
            long stepX = CalculateSteps(1);
            long stepY = CalculateSteps(2);
            long stepZ = CalculateSteps(4);
            long lcmXY = (stepX * stepY) / Extensions.GCD(stepX, stepY);
            return ((lcmXY * stepZ) / Extensions.GCD(lcmXY, stepZ)).ToString();
        }

        private int CalculateSteps(int axis) {
            Moon[] original = new Moon[moons.Length];
            Array.Copy(moons, original, moons.Length);

            int step = 0;
            while (true) {
                for (int j = 0; j < moons.Length; j++) {
                    ref Moon moon = ref moons[j];
                    for (int k = j + 1; k < moons.Length; k++) {
                        moon.ApplyGravity(ref moons[k], axis);
                    }
                }

                for (int j = 0; j < moons.Length; j++) {
                    moons[j].ApplyVelocity();
                }

                step++;

                if (ArrayComparer<Moon>.Comparer.Equals(original, moons)) {
                    return step;
                }
            }
        }

        private struct Moon : IEquatable<Moon> {
            public int X, Y, Z;
            public int VX, VY, VZ;

            public Moon(string position) {
                int index1 = position.IndexOf('=');
                int index2 = position.IndexOf(',', index1);
                X = Tools.ParseInt(position, index1 + 1, index2 - index1 - 1);

                index1 = position.IndexOf('=', index2);
                index2 = position.IndexOf(',', index1);
                Y = Tools.ParseInt(position, index1 + 1, index2 - index1 - 1);

                index1 = position.IndexOf('=', index2);
                index2 = position.IndexOf('>', index1);
                Z = Tools.ParseInt(position, index1 + 1, index2 - index1 - 1);
                VX = 0;
                VY = 0;
                VZ = 0;
            }

            public int TotalEnergy() {
                return (Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z)) * (Math.Abs(VX) + Math.Abs(VY) + Math.Abs(VZ));
            }
            public void ApplyGravity(ref Moon other, int axis = 7) {
                int vx = ((axis & 1) == 0 || other.X == X) ? 0 : other.X > X ? 1 : -1;
                int vy = ((axis & 2) == 0 || other.Y == Y) ? 0 : other.Y > Y ? 1 : -1;
                int vz = ((axis & 4) == 0 || other.Z == Z) ? 0 : other.Z > Z ? 1 : -1;

                VX += vx;
                VY += vy;
                VZ += vz;
                other.VX -= vx;
                other.VY -= vy;
                other.VZ -= vz;
            }
            public void ApplyVelocity() {
                X += VX;
                Y += VY;
                Z += VZ;
            }
            public override string ToString() {
                return $"({X}, {Y}, {Z}) ({VX}, {VY}, {VZ})";
            }

            public bool Equals(Moon other) {
                return X == other.X && Y == other.Y && Z == other.Z && VX == other.VX && VY == other.VY && VZ == other.VZ;
            }
            public override bool Equals(object obj) {
                return obj is Moon moon && Equals(moon);
            }
            public override int GetHashCode() {
                return (X ^ Y ^ Z) | ((VX ^ VY ^ VZ) * 65536);
            }
        }
    }
}