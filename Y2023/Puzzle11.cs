using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Cosmic Expansion")]
    public class Puzzle11 : ASolver {
        private Galaxy[] galaxyCountsX, galaxyCountsY;
        private int galaxiesX, galaxiesY;

        public override void Setup() {
            string[] space = Input.Split('\n');
            int height = space.Length;
            int width = space[0].Length;
            galaxyCountsX = new Galaxy[width];
            galaxyCountsY = new Galaxy[height];

            for (int y = 0; y < height; y++) {
                string line = space[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    if (c != '#') { continue; }

                    galaxyCountsX[x].Count++;
                    galaxyCountsY[y].Count++;
                }
            }

            int expansions = 0;
            for (int y = 0; y < height; y++) {
                if (galaxyCountsY[y].Count > 0) {
                    galaxiesY += galaxyCountsY[y].Count;
                    galaxyCountsY[y].Expansions = expansions;
                    continue;
                }
                expansions++;
            }

            expansions = 0;
            for (int x = 0; x < width; x++) {
                if (galaxyCountsX[x].Count > 0) {
                    galaxiesX += galaxyCountsX[x].Count;
                    galaxyCountsX[x].Expansions = expansions;
                    continue;
                }
                expansions++;
            }
        }

        [Description("What is the sum of these lengths?")]
        public override string SolvePart1() {
            return $"{GetSumOfLength(2)}";
        }

        [Description("What is the sum of these lengths?")]
        public override string SolvePart2() {
            return $"{GetSumOfLength(1000000)}";
        }

        private long GetSumOfLength(int expansions) {
            long total = 0;
            for (int i = galaxyCountsX.Length - 1, j = galaxiesX - 1; i >= 0; i--) {
                Galaxy galaxy = galaxyCountsX[i];
                if (galaxy.Count == 0) { continue; }

                int modifier = galaxy.Count * (j - galaxy.Count + 1);
                total += galaxy.GetValue(i, expansions) * modifier;
                j -= 2 * galaxy.Count;
            }

            for (int i = galaxyCountsY.Length - 1, j = galaxiesY - 1; i >= 0; i--) {
                Galaxy galaxy = galaxyCountsY[i];
                if (galaxy.Count == 0) { continue; }
                int modifier = galaxy.Count * (j - galaxy.Count + 1);
                total += galaxy.GetValue(i, expansions) * modifier;
                j -= 2 * galaxy.Count;
            }

            return total;
        }

        private struct Galaxy {
            public int Count, Expansions;
            public long GetValue(int index, int expansions) { return (long)index + (long)Expansions * (expansions - 1); }
            public override string ToString() { return $"{Count},{Expansions}"; }
        }
    }
}