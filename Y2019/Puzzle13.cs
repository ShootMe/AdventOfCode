using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Care Package")]
    public class Puzzle13 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Input.ToLongs(','));
        }

        [Description("How many block tiles are on the screen when the game exits?")]
        public override string SolvePart1() {
            Dictionary<Point, int> screen = RunGame();
            int count = 0;
            foreach (int value in screen.Values) {
                if (value == 2) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("What is your score after the last block is broken?")]
        public override string SolvePart2() {
            return $"{PlayGame()}";
        }

        private Dictionary<Point, int> RunGame() {
            Dictionary<Point, int> screen = new Dictionary<Point, int>();
            program.Reset();

            while (program.Run()) {
                int out1 = (int)program.Output;
                program.Run();
                int out2 = (int)program.Output;
                program.Run();
                int out3 = (int)program.Output;

                Point point = new Point() { X = out1, Y = out2 };
                if (screen.ContainsKey(point)) {
                    screen[point] = out3;
                } else {
                    screen.Add(point, out3);
                }
            }

            return screen;
        }
        private int PlayGame() {
            int width = 44;
            int height = 23;
            int[] screen = new int[width * height];

            program.Reset();
            program[0] = 2;

            int ballX = 0;
            int paddleX = 0;
            int score = 0;
            while (program.Run()) {
                if (program.InputRequired) {
                    program.Run(ballX < paddleX ? -1 : ballX == paddleX ? 0 : 1);
                }

                int out1 = (int)program.Output;
                program.Run();
                int out2 = (int)program.Output;
                program.Run();
                int out3 = (int)program.Output;

                if (out1 < 0) {
                    score = out3;
                } else {
                    screen[out2 * width + out1] = out3;
                    if (out3 == 4) {
                        ballX = out1;
                    } else if (out3 == 3) {
                        paddleX = out1;
                    }
                }
            }

            return score;
        }
    }
}