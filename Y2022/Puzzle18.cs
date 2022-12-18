using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Boiling Boulders")]
    public class Puzzle18 : ASolver {
        private HashSet<Cube> cubeSet = new();
        private List<Cube> cubes = new();
        private int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
        private int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                string[] splits = line.Split(',');
                Cube cube = new Cube() { X = splits[0].ToInt(), Y = splits[1].ToInt(), Z = splits[2].ToInt() };
                cubes.Add(cube);
                cubeSet.Add(cube);
            }

            for (int i = 0; i < cubes.Count; i++) {
                Cube cube = cubes[i];
                if (cube.X < minX) { minX = cube.X; }
                if (cube.X > maxX) { maxX = cube.X; }
                if (cube.Y < minY) { minY = cube.Y; }
                if (cube.Y > maxY) { maxY = cube.Y; }
                if (cube.Z < minZ) { minZ = cube.Z; }
                if (cube.Z > maxZ) { maxZ = cube.Z; }
            }
            minX--; minY--; minZ--;
            maxX++; maxY++; maxZ++;
        }

        [Description("What is the surface area of your scanned lava droplet?")]
        public override string SolvePart1() {
            return $"{SurfaceArea(cubes)}";
        }

        [Description("What is the exterior surface area of your scanned lava droplet?")]
        public override string SolvePart2() {
            List<Cube> outerCubes = GenerateOuterCubes();

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            int depth = maxZ - minZ + 1;
            return $"{SurfaceArea(outerCubes) - 2 * (width * height + width * depth + height * depth)}";
        }

        private List<Cube> GenerateOuterCubes() {
            Queue<Cube> open = new();
            List<Cube> outerCubes = new();

            Cube current = new Cube() { X = minX, Y = minY, Z = minZ };
            open.Enqueue(current);

            while (open.Count > 0) {
                current = open.Dequeue();
                outerCubes.Add(current);

                TryAdd(open, new Cube() { X = current.X + 1, Y = current.Y, Z = current.Z });
                TryAdd(open, new Cube() { X = current.X - 1, Y = current.Y, Z = current.Z });
                TryAdd(open, new Cube() { X = current.X, Y = current.Y + 1, Z = current.Z });
                TryAdd(open, new Cube() { X = current.X, Y = current.Y - 1, Z = current.Z });
                TryAdd(open, new Cube() { X = current.X, Y = current.Y, Z = current.Z + 1 });
                TryAdd(open, new Cube() { X = current.X, Y = current.Y, Z = current.Z - 1 });
            }
            return outerCubes;
        }
        private void TryAdd(Queue<Cube> open, Cube cube) {
            if (cube.X >= minX && cube.X <= maxX && cube.Y >= minY && cube.Y <= maxY && cube.Z >= minZ && cube.Z <= maxZ && cubeSet.Add(cube)) {
                open.Enqueue(cube);
            }
        }
        private static int SurfaceArea(List<Cube> cubes) {
            int total = cubes.Count * 6;
            for (int i = 0; i < cubes.Count; i++) {
                Cube one = cubes[i];
                for (int j = i + 1; j < cubes.Count; j++) {
                    Cube two = cubes[j];
                    if ((one.X == two.X && one.Y == two.Y && Math.Abs(one.Z - two.Z) == 1) ||
                       (one.X == two.X && one.Z == two.Z && Math.Abs(one.Y - two.Y) == 1) ||
                       (one.Y == two.Y && one.Z == two.Z && Math.Abs(one.X - two.X) == 1)) {
                        total -= 2;
                    }
                }
            }
            return total;
        }

        private struct Cube : IEquatable<Cube> {
            public int X, Y, Z;

            public bool Equals(Cube other) {
                return X == other.X && Y == other.Y && Z == other.Z;
            }
            public override int GetHashCode() {
                return X + Y + Z;
            }
            public override string ToString() {
                return $"[{X},{Y},{Z}]";
            }
        }
    }
}