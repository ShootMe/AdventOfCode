using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Reactor Reboot")]
    public class Puzzle22 : ASolver {
        private List<Cube> cubes;

        public override void Setup() {
            List<string> items = Input.Lines();
            cubes = new List<Cube>();
            for (int i = 0; i < items.Count; i++) {
                string line = items[i];
                cubes.Add(new Cube(line));
            }
        }

        [Description("How many cubes are on?")]
        public override string SolvePart1() {
            List<Cube> calculated = new List<Cube>();
            long totalVolume = 0;
            for (int i = cubes.Count - 1; i >= 0; i--) {
                Cube cube = cubes[i];
                if (!cube.IsValid(-50, 50)) { continue; }

                if (cube.Value) {
                    totalVolume += cube.Volume() - Overlap(calculated, cube, 0);
                }
                calculated.Add(cube);
            }
            return $"{totalVolume}";
        }

        [Description("How many cubes are on for the whole input?")]
        public override string SolvePart2() {
            List<Cube> calculated = new List<Cube>();
            long totalVolume = 0;
            for (int i = cubes.Count - 1; i >= 0; i--) {
                Cube cube = cubes[i];

                if (cube.Value) {
                    totalVolume += cube.Volume() - Overlap(calculated, cube, 0);
                }
                calculated.Add(cube);
            }
            return $"{totalVolume}";
        }
        private long Overlap(List<Cube> calculated, Cube cube, int index) {
            long volume = 0;
            for (int i = index; i < calculated.Count; i++) {
                Cube other = calculated[i];
                if (!cube.Overlaps(other)) { continue; }

                Cube overlap = cube.Intersection(other);
                volume += overlap.Volume() - Overlap(calculated, overlap, i + 1);
            }
            return volume;
        }
        private class Cube {
            public int X1, X2;
            public int Y1, Y2;
            public int Z1, Z2;
            public bool Value;

            public Cube(int x1, int x2, int y1, int y2, int z1, int z2, bool value) {
                X1 = x1; X2 = x2;
                Y1 = y1; Y2 = y2;
                Z1 = z1; Z2 = z2;
                Value = value;
            }
            public Cube(string line) {
                string[] splits = line.SplitOn(" x=", "..", ",y=", "..", ",z=", "..");
                Value = splits[0] == "on";
                X1 = splits[1].ToInt();
                X2 = splits[2].ToInt();

                Y1 = splits[3].ToInt();
                Y2 = splits[4].ToInt();

                Z1 = splits[5].ToInt();
                Z2 = splits[6].ToInt();
            }
            public bool IsValid(int min, int max) {
                return X1 <= max && X2 >= min
                    && Y1 <= max && Y2 >= min
                    && Z1 <= max && Z2 >= min;
            }
            public long Volume() {
                return (long)(X2 - X1 + 1) * (Y2 - Y1 + 1) * (Z2 - Z1 + 1);
            }
            public bool Overlaps(Cube cube) {
                return X1 <= cube.X2 && X2 >= cube.X1
                    && Y1 <= cube.Y2 && Y2 >= cube.Y1
                    && Z1 <= cube.Z2 && Z2 >= cube.Z1;
            }
            public Cube Intersection(Cube cube) {
                return new Cube(Math.Max(X1, cube.X1), Math.Min(X2, cube.X2),
                Math.Max(Y1, cube.Y1), Math.Min(Y2, cube.Y2),
                Math.Max(Z1, cube.Z1), Math.Min(Z2, cube.Z2), true);
            }
            public override string ToString() {
                return $"({X1}..{X2})({Y1}..{Y2})({Z1}..{Z2}) On={Value}";
            }
        }
    }
}