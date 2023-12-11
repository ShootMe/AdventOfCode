using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Cosmic Expansion")]
    public class Puzzle11 : ASolver {
        private List<Galaxy> galaxies = new();
        private int width, height;

        public override void Setup() {
            string[] space = Input.Split('\n');
            height = space.Length;
            width = space[0].Length;
            HashSet<int> rowsFilled = new();
            HashSet<int> colsFilled = new();
            for (int j = 0; j < height; j++) {
                string line = space[j];
                for (int i = 0; i < width; i++) {
                    char c = line[i];
                    if (c == '#') {
                        rowsFilled.Add(j);
                        colsFilled.Add(i);
                        galaxies.Add(new Galaxy() { ID = galaxies.Count + 1, X = i, Y = j });
                    }
                }
            }
            for (int j = height - 1; j >= 0; j--) {
                if (rowsFilled.Contains(j)) { continue; }
                for (int i = 0; i < galaxies.Count; i++) {
                    Galaxy galaxy = galaxies[i];
                    if (galaxy.Y > j) {
                        galaxy.EY++;
                    }
                }
            }
            for (int j = width - 1; j >= 0; j--) {
                if (colsFilled.Contains(j)) { continue; }
                for (int i = 0; i < galaxies.Count; i++) {
                    Galaxy galaxy = galaxies[i];
                    if (galaxy.X > j) {
                        galaxy.EX++;
                    }
                }
            }
        }

        [Description("What is the sum of these lengths?")]
        public override string SolvePart1() {
            long total = 0;
            for (int i = 0; i < galaxies.Count; i++) {
                Galaxy galaxy1 = galaxies[i];
                for (int j = i + 1; j < galaxies.Count; j++) {
                    Galaxy galaxy2 = galaxies[j];
                    total += Math.Abs(galaxy1.GetX() - galaxy2.GetX()) + Math.Abs(galaxy1.GetY() - galaxy2.GetY());
                }
            }
            return $"{total}";
        }

        [Description("What is the sum of these lengths?")]
        public override string SolvePart2() {
            long total = 0;
            for (int i = 0; i < galaxies.Count; i++) {
                Galaxy galaxy1 = galaxies[i];
                for (int j = i + 1; j < galaxies.Count; j++) {
                    Galaxy galaxy2 = galaxies[j];
                    total += Math.Abs(galaxy1.GetX(1000000) - galaxy2.GetX(1000000)) + Math.Abs(galaxy1.GetY(1000000) - galaxy2.GetY(1000000));
                }
            }
            return $"{total}";
        }

        private class Galaxy {
            public int ID, X, Y, EX, EY;
            public long GetX(int expansions = 2) { return (long)X + (long)EX * (expansions - 1); }
            public long GetY(int expansions = 2) { return (long)Y + (long)EY * (expansions - 1); }
        }
    }
}