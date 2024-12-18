using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("RAM Run")]
    public class Puzzle18 : ASolver {
        private int[] bytes;
        private char[,] grid;
        public override void Setup() {
            bytes = Input.Replace('\n', ',').ToInts(',');
            grid = new char[73, 73];
            for (int i = 0; i < 73; i++) {
                grid[0, i] = '#';
                grid[72, i] = '#';
                grid[i, 0] = '#';
                grid[i, 72] = '#';
            }
        }

        [Description("What is the minimum number of steps needed to reach the exit?")]
        public override string SolvePart1() {
            for (int i = 0; i < 2048; i += 2) {
                grid[bytes[i + 1] + 1, bytes[i] + 1] = '#';
            }
            return $"{Solve()}";
        }

        [Description("What are the coordinates of that will prevent the exit from being reachable from your starting position?")]
        public override string SolvePart2() {
            int index = Search(2048, bytes.Length - 2);
            return $"{bytes[index]},{bytes[index + 1]}";
        }
        private int Search(int start, int end) {
            if (start == end) { return start; }
            int mid = (start + end) / 2;
            mid += mid & 1;
            for (int i = start; i < mid; i += 2) {
                grid[bytes[i + 1] + 1, bytes[i] + 1] = '#';
            }
            if (Solve() == 0) {
                for (int i = start; i < mid; i += 2) {
                    grid[bytes[i + 1] + 1, bytes[i] + 1] = '\0';
                }
                return Search(start, mid - 2);
            }
            return Search(mid, end);
        }
        private int Solve() {
            HashSet<(int, int)> seen = new();
            Queue<(int, int, int)> open = new();
            void AddNext(int x, int y, int steps) {
                char m = grid[y, x];
                if (m == '#' || !seen.Add((x, y))) { return; }
                open.Enqueue((x, y, steps));
            }

            open.Enqueue((1, 1, 0));
            while (open.Count > 0) {
                (int x, int y, int steps) = open.Dequeue();
                if (x == 71 && y == 71) {
                    return steps;
                }

                AddNext(x + 1, y, steps + 1);
                AddNext(x - 1, y, steps + 1);
                AddNext(x, y + 1, steps + 1);
                AddNext(x, y - 1, steps + 1);
            }
            return 0;
        }
    }
}