using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2019 {
    [Description("Set and Forget")]
    public class Puzzle17 : ASolver {
        private IntCode program;
        private int width, height;
        private char[] display;

        public override void Setup() {
            program = new IntCode(Tools.GetLongs(Input, ','));

            width = 0;
            int count = 0;
            while (program.Run()) {
                count++;
                if (program.Output == 10 && width == 0) {
                    width = count - 1;
                }
            }

            height = count / (width + 1);
            display = new char[width * height];
            program.Reset();
            int index = 0;
            while (program.Run()) {
                if (program.Output != 10) {
                    display[index++] = (char)program.Output;
                }
            }
        }

        [Description("What is the sum of the alignment parameters for the scaffold intersections?")]
        public override string SolvePart1() {
            int x = 1;
            int y = 1;
            int maxLength = display.Length - width;
            int sum = 0;
            for (int i = width + 1; i < maxLength; i++) {
                char current = display[i];
                char up = display[i - width];
                char down = display[i + width];
                char left = display[i - 1];
                char right = display[i + 1];

                if (current == '#' && current == up && up == down && down == left && left == right) {
                    sum += x * y;
                }

                x++;
                if (x == width - 1) {
                    i += 2;
                    x = 1;
                    y++;
                }
            }
            return $"{sum}";
        }

        [Description("After visiting every part, how much dust does the vacuum robot report it has collected?")]
        public override string SolvePart2() {
            Point robot = FindRobot();
            string path = FindPath(robot);
            List<string> movements = new List<string>();
            FindMovements(movements, path);

            program.Reset();
            program[0] = 2;

            string main = path.Replace(movements[0], "A,").Replace(movements[1], "B,").Replace(movements[2], "C,");
            main = $"{main.Substring(0, main.Length - 1)}\n";
            for (int i = 0; i < main.Length; i++) {
                program.WriteInput(main[i]);
            }

            string moveA = $"{movements[0].Substring(0, movements[0].Length - 1)}\n";
            for (int i = 0; i < moveA.Length; i++) {
                program.WriteInput(moveA[i]);
            }

            string moveB = $"{movements[1].Substring(0, movements[1].Length - 1)}\n";
            for (int i = 0; i < moveB.Length; i++) {
                program.WriteInput(moveB[i]);
            }

            string moveC = $"{movements[2].Substring(0, movements[2].Length - 1)}\nn\n";
            for (int i = 0; i < moveC.Length; i++) {
                program.WriteInput(moveC[i]);
            }

            while (program.Run()) { }

            return $"{program.Output}";
        }

        private Point FindRobot() {
            int x = 0;
            int y = 0;
            for (int i = 0; i < display.Length; i++) {
                char c = display[i];
                switch (c) {
                    case '^':
                    case '<':
                    case 'v':
                    case '>':
                        return new Point() { X = x, Y = y };
                }
                x++;
                if (x == width) {
                    x = 0;
                    y++;
                }
            }
            return Point.ZERO;
        }
        private string FindPath(Point robot) {
            int index = robot.Y * width + robot.X;
            char facing = display[index];
            int dir = facing == '^' ? 0 : facing == '>' ? 1 : facing == 'v' ? 2 : 3;
            int[] moveAmounts = new int[] { -width, 1, width, -1 };
            int[] moveAmountsX = new int[] { 0, 1, 0, -1 };
            int[] moveAmountsY = new int[] { -1, 0, 1, 0 };
            StringBuilder sb = new StringBuilder();
            int moveAmount = 0;
            int originalDir = dir;

            while (true) {
                int nextY = robot.Y + moveAmountsY[dir];
                int nextX = robot.X + moveAmountsX[dir];
                int nextIndex = index + moveAmounts[dir];
                char next = nextY >= 0 && nextX >= 0 && nextY < height && nextX < width ? display[nextIndex] : '\0';

                if (next == '#') {
                    if (dir + 1 == originalDir || (originalDir == 0 && dir == 3)) {
                        if (moveAmount > 0) {
                            sb.Append($"{moveAmount},");
                        }
                        sb.Append("L,");
                        moveAmount = 0;
                    } else if (dir - 1 == originalDir || (originalDir == 3 && dir == 0)) {
                        if (moveAmount > 0) {
                            sb.Append($"{moveAmount},");
                        }
                        sb.Append("R,");
                        moveAmount = 0;
                    }
                    originalDir = dir;
                    moveAmount++;
                    robot.X = nextX;
                    robot.Y = nextY;
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
                    sb.Append($"{moveAmount},");
                    return sb.ToString();
                }
            }
        }
        private bool FindMovements(List<string> movements, string path) {
            if (movements.Count <= 3) {
                if (string.IsNullOrWhiteSpace(path)) {
                    return true;
                } else if (movements.Count == 3) {
                    return false;
                }
            }

            int starting = 0;
            while (starting < path.Length) {
                char current = path[starting];
                if (current == 'L' || current == 'R' || char.IsDigit(current)) {
                    break;
                }
                starting++;
            }

            int commas = 0;
            for (int j = starting + 1; j < path.Length; j++) {
                char next = path[j];
                if (next == ' ') { break; }

                if (next == ',') {
                    commas++;
                    if (commas >= 2) {
                        if (j - starting >= 20) { break; }

                        movements.Add(path.Substring(starting, j - starting + 1));

                        if (FindMovements(movements, path.Replace(movements[^1], " "))) {
                            return true;
                        }

                        movements.RemoveAt(movements.Count - 1);
                    }
                }
            }

            return false;
        }
    }
}