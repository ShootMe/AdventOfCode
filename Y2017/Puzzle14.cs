using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2017 {
    [Description("Disk Defragmentation")]
    public class Puzzle14 : ASolver {
        private int[] grid;
        private int squareCount;

        public override void Setup() {
            squareCount = 0;
            grid = new int[128 * 128];
            for (int i = 0; i < 128; i++) {
                WrappingList<byte> hash = KnotHash.Calculate($"{Input}-{i}");
                int index = i * 128;
                foreach (byte value in hash) {
                    squareCount += BitOperations.PopCount((uint)value);
                    byte mask = 128;
                    while (mask != 0) {
                        grid[index++] = (mask & value) != 0 ? -1 : 0;
                        mask >>= 1;
                    }
                }
            }
        }

        [Description("How many squares are used?")]
        public override string SolvePart1() {
            return $"{squareCount}";
        }

        [Description("How many regions are present given your key string?")]
        public override string SolvePart2() {
            int x = 0;
            int y = 0;
            int group = 1;
            for (int i = 0; i < grid.Length; i++) {
                if (grid[i] < 0) {
                    grid[i] = group;
                    Expand(group, x, y, 1, 0);
                    Expand(group, x, y, 0, 1);
                    Expand(group, x, y, -1, 0);
                    Expand(group++, x, y, 0, -1);
                }

                x++;
                if (x == 128) {
                    x = 0;
                    y++;
                }
            }

            return $"{group - 1}";
        }

        private void Expand(int group, int x, int y, int xMove, int yMove) {
            int newX = x + xMove;
            int newY = y + yMove;
            int index = newY * 128 + newX;
            if (newX >= 0 && newY >= 0 && newX < 128 && newY < 128 && grid[index] < 0) {
                grid[index] = group;
                Expand(group, newX, newY, 1, 0);
                Expand(group, newX, newY, 0, 1);
                Expand(group, newX, newY, -1, 0);
                Expand(group, newX, newY, 0, -1);
            }
        }
    }
}