using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Hex Ed")]
    public class Puzzle11 : ASolver {
        [Description("What is the fewest number of steps required to reach him?")]
        public override string SolvePart1() {
            string[] directions = Input.Split(',');
            int x = 0;
            int y = 0;

            for (int i = 0; i < directions.Length; i++) {
                string dir = directions[i];
                switch (dir) {
                    case "n": y++; break;
                    case "ne": x++; break;
                    case "se": x++; y--; break;
                    case "s": y--; break;
                    case "sw": x--; break;
                    case "nw": x--; y++; break;
                }
            }

            return $"{GetDistance(x, y)}";
        }

        [Description("How many steps away is the furthest he ever got from his starting position?")]
        public override string SolvePart2() {
            string[] directions = Input.Split(',');
            int x = 0;
            int y = 0;
            int maxDist = 0;

            for (int i = 0; i < directions.Length; i++) {
                string dir = directions[i];
                switch (dir) {
                    case "n": y++; break;
                    case "ne": x++; break;
                    case "se": x++; y--; break;
                    case "s": y--; break;
                    case "sw": x--; break;
                    case "nw": x--; y++; break;
                }

                int dist = GetDistance(x, y);
                if (dist > maxDist) {
                    maxDist = dist;
                }
            }

            return $"{maxDist}";
        }

        private int GetDistance(int x, int y) {
            if ((x >= 0 && y >= 0) || (x <= 0 && y <= 0)) {
                return Math.Abs(x + y);
            }
            return Math.Max(Math.Abs(x), Math.Abs(y));
        }
    }
}