using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Lanternfish")]
    public class Puzzle06 : ASolver {
        private int[] fishCounts;

        public override void Setup() {
            int[] fishes = Input.ToInts(',');
            fishCounts = new int[9];
            for (int i = 0; i < fishes.Length; i++) {
                fishCounts[fishes[i]]++;
            }
        }

        [Description("How many lanternfish would there be after 80 days?")]
        public override string SolvePart1() {
            return $"{CountFishAfter(80)}";
        }

        [Description("How many lanternfish would there be after 256 days?")]
        public override string SolvePart2() {
            return $"{CountFishAfter(256)}";
        }

        public long CountFishAfter(int days) {
            Span<long> population = stackalloc long[9];
            for (int i = 0; i < 9; i++) { population[i] = fishCounts[i]; }

            int indexA = 7;
            int indexB = 0;
            for (int i = 0; i < days; i++) {
                population[indexA++] += population[indexB++];
                if (indexA >= 9) { indexA = 0; }
                if (indexB >= 9) { indexB = 0; }
            }

            long totalPopulation = 0;
            for (int i = 0; i < 9; i++) {
                totalPopulation += population[i];
            }
            return totalPopulation;
        }
    }
}