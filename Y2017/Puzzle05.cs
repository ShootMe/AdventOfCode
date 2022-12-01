using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("A Maze of Twisty Trampolines, All Alike")]
    public class Puzzle05 : ASolver {
        private int[] codes;

        public override void Setup() {
            codes = Tools.GetInts(Input);
        }

        [Description("How many steps does it take to reach the exit?")]
        public override string SolvePart1() {
            int[] program = new int[codes.Length];
            Array.Copy(codes, program, codes.Length);

            int index = 0;
            int steps = 0;
            while (index >= 0 && index < program.Length) {
                steps++;
                ref int code = ref program[index];
                index += code;
                code++;
            }
            return $"{steps}";
        }

        [Description("How many steps does it now take to reach the exit?")]
        public override string SolvePart2() {
            int[] program = new int[codes.Length];
            Array.Copy(codes, program, codes.Length);

            int index = 0;
            int steps = 0;
            while (index >= 0 && index < program.Length) {
                steps++;
                ref int code = ref program[index];
                index += code;
                if (code >= 3) {
                    code--;
                } else {
                    code++;
                }
            }
            return $"{steps}";
        }
    }
}