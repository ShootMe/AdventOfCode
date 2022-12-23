using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Unstable Diffusion")]
    public class Puzzle23 : ASolver {
        private HashSet<(int x, int y)> elves = new();
        
        public override void Setup() {
            string[] lines = Input.Split('\n');

            for (int y = 0; y < lines.Length; y++) {
                string line = lines[y];
                for (int x = 0; x < line.Length; x++) {
                    if (line[x] == '#') {
                        elves.Add((x, y));
                    }
                }
            }
        }

        [Description("How many empty ground tiles does that rectangle contain?")]
        public override string SolvePart1() {
            Simulate(0, 10);
            var bounds = GetBounds();
            return $"{(bounds.maxX - bounds.minX + 1) * (bounds.maxY - bounds.minY + 1) - elves.Count}";
        }

        [Description("Figure out where the Elves need to go. What is the number of the first round where no Elf moves?")]
        public override string SolvePart2() {
            return $"{Simulate(10, int.MaxValue)}";
        }

        private int Simulate(int start, int max) {
            HashSet<(int x, int y)> newMap = new();
            Dictionary<(int x, int y), (int x, int y)> used = new();

            while (start < max) {
                newMap.Clear(); used.Clear();
                int moved = 0;
                foreach ((int x, int y) in elves) {
                    (bool hasElfN, int dirXN, int dirYN) = HasElf(start & 3, x, y);
                    (bool hasElfS, int dirXS, int dirYS) = HasElf((start + 1) & 3, x, y);
                    (bool hasElfW, int dirXW, int dirYW) = HasElf((start + 2) & 3, x, y);
                    (bool hasElfE, int dirXE, int dirYE) = HasElf((start + 3) & 3, x, y);

                    if (hasElfN || hasElfS || hasElfW || hasElfE) {
                        int dirX; int dirY;
                        if (!hasElfN) {
                            dirX = dirXN; dirY = dirYN;
                        } else if (!hasElfS) {
                            dirX = dirXS; dirY = dirYS;
                        } else if (!hasElfW) {
                            dirX = dirXW; dirY = dirYW;
                        } else if (!hasElfE) {
                            dirX = dirXE; dirY = dirYE;
                        } else {
                            newMap.Add((x, y));
                            continue;
                        }

                        if (!used.TryGetValue((x + dirX, y + dirY), out var orig)) {
                            used[(x + dirX, y + dirY)] = (x, y);
                            newMap.Add((x + dirX, y + dirY));
                            moved++;
                        } else {
                            used[(x + dirX, y + dirY)] = (int.MaxValue, int.MaxValue);
                            if (orig.x != int.MaxValue) {
                                newMap.Remove((x + dirX, y + dirY));
                                newMap.Add(orig);
                                moved--;
                            }
                            newMap.Add((x, y));
                        }
                    } else {
                        newMap.Add((x, y));
                    }
                }

                var temp = elves;
                elves = newMap;
                newMap = temp;
                start++;
                if (moved == 0) { return start; }
            }

            return start;
        }
        private (int minX, int minY, int maxX, int maxY) GetBounds() {
            int minX = int.MaxValue; int minY = int.MaxValue;
            int maxX = int.MinValue; int maxY = int.MinValue;
            foreach (var pos in elves) {
                if (pos.x < minX) { minX = pos.x; }
                if (pos.x > maxX) { maxX = pos.x; }
                if (pos.y < minY) { minY = pos.y; }
                if (pos.y > maxY) { maxY = pos.y; }
            }

            //System.Console.WriteLine();
            //for (int y = minY; y <= maxY; y++) {
            //    for (int x = minX; x <= maxX; x++) {
            //        if (elves.Contains((x, y))) {
            //            System.Console.Write('#');
            //        } else {
            //            System.Console.Write('.');
            //        }
            //    }
            //    System.Console.WriteLine();
            //}
            return (minX, minY, maxX, maxY);
        }
        private (bool, int, int) HasElf(int dir, int x, int y) {
            return dir switch {
                0 => (elves.Contains((x - 1, y - 1)) || elves.Contains((x, y - 1)) || elves.Contains((x + 1, y - 1)), 0, -1),
                1 => (elves.Contains((x - 1, y + 1)) || elves.Contains((x, y + 1)) || elves.Contains((x + 1, y + 1)), 0, 1),
                2 => (elves.Contains((x - 1, y - 1)) || elves.Contains((x - 1, y)) || elves.Contains((x - 1, y + 1)), -1, 0),
                _ => (elves.Contains((x + 1, y - 1)) || elves.Contains((x + 1, y)) || elves.Contains((x + 1, y + 1)), 1, 0)
            };
        }
    }
}