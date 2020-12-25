using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    public class Puzzle20 : ASolver {
        private Particle[] particles;
        public Puzzle20(string input) : base(input) { Name = "Particle Swarm"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            particles = new Particle[items.Count];
            for (int i = 0; i < items.Count; i++) {
                particles[i] = new Particle(items[i]);
            }
        }

        [Description("Which particle will stay closest to position <0,0,0> in the long term?")]
        public override string SolvePart1() {
            int minAccel = int.MaxValue;
            int minVel = int.MaxValue;
            int minIndex = 0;
            for (int i = 0; i < particles.Length; i++) {
                Particle particle = particles[i];
                int accel = Math.Abs(particle.AX) + Math.Abs(particle.AY) + Math.Abs(particle.AZ);
                if (accel <= minAccel) {
                    int velocity = Math.Abs(particle.VX) + Math.Abs(particle.VY) + Math.Abs(particle.VZ);
                    if (accel < minAccel || velocity < minVel) {
                        minVel = velocity;
                        minAccel = accel;
                        minIndex = i;
                    }
                }
            }
            return $"{minIndex}";
        }

        [Description("How many particles are left after all collisions are resolved?")]
        public override string SolvePart2() {
            bool[] destroyed = new bool[particles.Length];
            int lastClosest = int.MaxValue;
            bool shouldStep;

            while (true) {
                int closestDistance = int.MaxValue;
                shouldStep = false;

                for (int i = 0; i < particles.Length; i++) {
                    if (destroyed[i]) { continue; }

                    if (!particles[i].Step()) {
                        shouldStep = true;
                    }
                }

                for (int i = 0; i < particles.Length; i++) {
                    if (destroyed[i]) { continue; }

                    Particle one = particles[i];

                    for (int j = i + 1; j < particles.Length; j++) {
                        if (destroyed[j]) { continue; }

                        Particle two = particles[j];
                        int distance = one.Distance(two);
                        if (distance == 0) {
                            destroyed[i] = true;
                            destroyed[j] = true;
                        }

                        if (distance < closestDistance) {
                            closestDistance = distance;
                        }
                    }
                }

                if (!shouldStep && closestDistance != 0 && closestDistance >= lastClosest) {
                    break;
                }
                lastClosest = closestDistance;
            }

            int count = 0;
            for (int i = 0; i < destroyed.Length; i++) {
                if (!destroyed[i]) {
                    count++;
                }
            }
            return $"{count}";
        }

        private struct Particle {
            public int X, Y, Z;
            public int VX, VY, VZ;
            public int AX, AY, AZ;

            public Particle(string value) {
                int index1 = value.IndexOf(',');
                X = Tools.ParseInt(value, 3, index1 - 3);

                int index2 = value.IndexOf(',', index1 + 1);
                Y = Tools.ParseInt(value, index1 + 1, index2 - index1 - 1);

                index1 = value.IndexOf('>', index2);
                Z = Tools.ParseInt(value, index2 + 1, index1 - index2 - 1);

                index2 = value.IndexOf(',', index1 + 3);
                VX = Tools.ParseInt(value, index1 + 6, index2 - index1 - 6);

                index1 = value.IndexOf(',', index2 + 1);
                VY = Tools.ParseInt(value, index2 + 1, index1 - index2 - 1);

                index2 = value.IndexOf('>', index1);
                VZ = Tools.ParseInt(value, index1 + 1, index2 - index1 - 1);

                index1 = value.IndexOf(',', index2 + 3);
                AX = Tools.ParseInt(value, index2 + 6, index1 - index2 - 6);

                index2 = value.IndexOf(',', index1 + 1);
                AY = Tools.ParseInt(value, index1 + 1, index2 - index1 - 1);

                index1 = value.IndexOf('>', index2);
                AZ = Tools.ParseInt(value, index2 + 1, index1 - index2 - 1);
            }
            public bool Step() {
                VX += AX;
                X += VX;

                VY += AY;
                Y += VY;

                VZ += AZ;
                Z += VZ;

                bool xSame = (X >= 0 && AX >= 0 && VX >= 0) || (X <= 0 && AX <= 0 && VX <= 0);
                bool ySame = (Y >= 0 && AY >= 0 && VY >= 0) || (Y <= 0 && AY <= 0 && VY <= 0);
                bool zSame = (Z >= 0 && AZ >= 0 && VZ >= 0) || (Z <= 0 && AZ <= 0 && VZ <= 0);
                return xSame && ySame && zSame;
            }
            public int Distance(Particle particle) {
                return Math.Abs(X - particle.X) + Math.Abs(Y - particle.Y) + Math.Abs(Z - particle.Z);
            }
            public override string ToString() {
                return $"({X},{Y},{Z})({VX},{VY},{VZ})({AX},{AY},{AZ})";
            }
        }
    }
}