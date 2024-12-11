using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Plutonian Pebbles")]
    public class Puzzle11 : ASolver {
        private List<long> stones = new();
        public override void Setup() {
            string[] values = Input.Split(' ');
            foreach (string value in values) {
                stones.Add(value.ToLong());
            }
        }

        [Description("How many stones will you have after blinking 25 times?")]
        public override string SolvePart1() {
            return $"{FindTotalStones(25)}";
        }

        [Description("How many stones would you have after blinking a total of 75 times?")]
        public override string SolvePart2() {
            return $"{FindTotalStones(75)}";
        }

        private long FindTotalStones(int blinks) {
            Dictionary<long, long> counts = new();
            for (int j = 0; j < stones.Count; j++) {
                UpdateCounts(counts, stones[j], 1);
            }

            Dictionary<long, long> newCounts = new();
            for (int i = 0; i < blinks; i++) {
                newCounts.Clear();
                foreach (var stone in counts) {
                    UpdateStone(stone, newCounts);
                }
                Dictionary<long, long> temp = counts;
                counts = newCounts;
                newCounts = temp;
            }

            long total = 0;
            foreach (long value in counts.Values) {
                total += value;
            }

            return total;
        }
        private static void UpdateCounts(Dictionary<long, long> counts, long newStone, long amount) {
            if (!counts.TryAdd(newStone, amount)) {
                counts[newStone] += amount;
            }
        }
        private void UpdateStone(KeyValuePair<long, long> stone, Dictionary<long, long> counts) {
            if (stone.Key == 0) {
                UpdateCounts(counts, 1, stone.Value);
            } else if (stone.Key == 1) {
                UpdateCounts(counts, 2024, stone.Value);
            } else {
                string stoneString = stone.Key.ToString();
                if ((stoneString.Length & 1) == 0) {
                    long next = stoneString.Substring(0, stoneString.Length / 2).ToLong();
                    UpdateCounts(counts, next, stone.Value);
                    next = stoneString.Substring(stoneString.Length / 2).ToLong();
                    UpdateCounts(counts, next, stone.Value);
                } else {
                    UpdateCounts(counts, stone.Key * 2024, stone.Value);
                }
            }
        }
    }
}