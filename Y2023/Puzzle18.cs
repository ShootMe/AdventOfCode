using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
namespace AdventOfCode.Y2023 {
    [Description("Lavaduct Lagoon")]
    public class Puzzle18 : ASolver {
        private List<Direction> directions = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                directions.Add(new Direction(line));
            }
        }

        [Description("How many cubic meters of lava could it hold?")]
        public override string SolvePart1() {
            int[] DirX = { 1, 0, -1, 0 };
            int[] DirY = { 0, 1, 0, -1 };
            int x = 0, y = 0;
            int sum = 0;
            int perimeter = 0;
            for (int i = 0; i < directions.Count; i++) {
                Direction direction = directions[i];
                int nx = x + DirX[direction.Dir] * direction.Length;
                int ny = y + DirY[direction.Dir] * direction.Length;
                sum += x * ny - y * nx;
                perimeter += direction.Length;
                x = nx; y = ny;
            }

            return $"{(Math.Abs(sum) - perimeter) / 2 + 1 + perimeter}";
        }

        [Description("How many cubic meters of lava could the lagoon hold?")]
        public override string SolvePart2() {
            int[] DirX = { 1, 0, -1, 0 };
            int[] DirY = { 0, 1, 0, -1 };
            long x = 0, y = 0;
            long sum = 0;
            long perimeter = 0;
            for (int i = 0; i < directions.Count; i++) {
                Direction direction = directions[i];
                long nx = x + DirX[direction.DirTrue] * direction.LengthTrue;
                long ny = y + DirY[direction.DirTrue] * direction.LengthTrue;
                sum += x * ny - y * nx;
                perimeter += direction.LengthTrue;
                x = nx; y = ny;
            }

            return $"{(Math.Abs(sum) - perimeter) / 2 + 1 + perimeter}";
        }

        private class Direction {
            public int Dir, Length;
            public int DirTrue, LengthTrue;
            public Direction(string data) {
                Dir = data[0] switch { 'R' => 0, 'D' => 1, 'L' => 2, _ => 3 };
                int index = data.IndexOf(' ', 2);
                Length = data.Substring(2, index - 2).ToInt();
                DirTrue = data[data.Length - 2] - '0';
                LengthTrue = int.Parse(data.Substring(index + 3, data.Length - 5 - index), NumberStyles.AllowHexSpecifier);
            }
            public override string ToString() {
                return $"{Dir},{Length},{DirTrue},{LengthTrue}";
            }
        }
    }
}