using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("1202 Program Alarm")]
    public class Puzzle02 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Tools.GetLongs(Input, ','));
        }

        [Description("What value is left at position 0 after the program halts?")]
        public override string SolvePart1() {
            return $"{RunProgram(12, 2)}";
        }

        [Description("What is 100 * noun + verb?")]
        public override string SolvePart2() {
            for (int i = 0; i < 100; i++) {
                for (int j = 0; j < 100; j++) {
                    int result = RunProgram(i, j);
                    if (result == 19690720) {
                        return $"{100 * i + j}";
                    }
                }
            }

            return string.Empty;
        }

        private int RunProgram(int noun, int verb) {
            program.Reset();
            program[1] = noun;
            program[2] = verb;
            program.Run();

            return (int)program[0];
        }
    }
}