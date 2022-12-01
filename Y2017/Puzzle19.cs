using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2017 {
    [Description("A Series of Tubes")]
    public class Puzzle19 : ASolver {
        private char[] network;
        private int width, height, start;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            width = items[0].Length;
            height = items.Count;
            network = new char[width * height];

            int index = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                for (int j = 0; j < item.Length; j++) {
                    network[index++] = item[j];
                }
            }

            for (int i = 0; i < width; i++) {
                if (network[i] == '|') {
                    start = i;
                    break;
                }
            }
        }

        [Description("What letters will the packet see if it follows the path?")]
        public override string SolvePart1() {
            return $"{FindPath(new Point() { X = start % width, Y = start / width }).letters}";
        }

        [Description("How many steps does the packet need to go?")]
        public override string SolvePart2() {
            return $"{FindPath(new Point() { X = start % width, Y = start / width }).moves}";
        }

        private (string letters, int moves) FindPath(Point packet) {
            int index = packet.Y * width + packet.X;
            int dir = 2;
            int[] moveAmounts = new int[] { -width, 1, width, -1 };
            int[] moveAmountsX = new int[] { 0, 1, 0, -1 };
            int[] moveAmountsY = new int[] { -1, 0, 1, 0 };
            StringBuilder sb = new StringBuilder();
            int originalDir = dir;
            int moves = 1;

            while (true) {
                int nextY = packet.Y + moveAmountsY[dir];
                int nextX = packet.X + moveAmountsX[dir];
                int nextIndex = index + moveAmounts[dir];
                char next = nextY >= 0 && nextX >= 0 && nextY < height && nextX < width ? network[nextIndex] : ' ';

                if (next != ' ') {
                    if (char.IsLetter(next)) {
                        sb.Append(next);
                    }
                    moves++;
                    originalDir = dir;
                    packet.X = nextX;
                    packet.Y = nextY;
                    index = nextIndex;
                } else if (dir == originalDir) {
                    dir--;
                    if (dir < 0) {
                        dir = 3;
                    }
                } else if (dir + 1 == originalDir || (originalDir == 0 && dir == 3)) {
                    dir = originalDir + 1;
                    if (dir > 3) {
                        dir = 0;
                    }
                } else {
                    return (sb.ToString(), moves);
                }
            }
        }
    }
}