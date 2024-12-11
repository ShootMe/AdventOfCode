using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Plutonian Pebbles")]
    public class Puzzle11 : ASolver {
        private List<long> stones = new();
        private Dictionary<(long, int), long> lookup = new();
        public override void Setup() {
            string[] values = Input.Split(' ');
            foreach (string value in values) {
                stones.Add(value.ToLong());
            }
        }

        [Description("How many stones will you have after blinking 25 times?")]
        public override string SolvePart1() {
            long total = 0;
            for (int j = 0; j < stones.Count; j++) {
                long stone = stones[j];
                total += NextStone(stone, 25);
            }
            return $"{total}";
        }
        
        [Description("How many stones would you have after blinking a total of 75 times?")]
        public override string SolvePart2() {
            long total = 0;
            for (int j = 0; j < stones.Count; j++) {
                long stone = stones[j];
                total += NextStone(stone, 75);
            }
            return $"{total}";
        }

        private long NextStone(long stone, int level) {
            if (level == 0) { return 1; }

            long value;
            if (lookup.TryGetValue((stone, level), out value)) {
                return value;
            }

            if (stone == 0) {
                value = NextStone(1, level - 1);
            } else if (stone == 1) {
                value = NextStone(2024, level - 1);
            } else {
                string stoneString = stone.ToString();
                if ((stoneString.Length & 1) == 0) {
                    value = NextStone(stoneString.Substring(0, stoneString.Length / 2).ToLong(), level - 1) +
                       NextStone(stoneString.Substring(stoneString.Length / 2).ToLong(), level - 1);
                } else {
                    value = NextStone(stone * 2024, level - 1);
                }
            }
            lookup.Add((stone, level), value);

            return value;
        }
    }
}