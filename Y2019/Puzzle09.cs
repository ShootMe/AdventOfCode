using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    public class Puzzle09 : ASolver {
        private IntCode program;
        public Puzzle09(string input) : base(input) { Name = "Sensor Boost"; }

        public override void Setup() {
            string[] elements = Input.Split(',');
            long[] code = new long[elements.Length];
            for (int i = 0; i < elements.Length; i++) {
                code[i] = Tools.ParseLong(elements[i]);
            }
            program = new IntCode(code);
        }

        [Description("What is the answer?")]
        public override string SolvePart1() {
            program.Reset();
            program.Run(1);
            return $"{program.Output}";
        }

        [Description("What is the answer?")]
        public override string SolvePart2() {
            program.Reset();
            program.Run(2);
            return $"{program.Output}";
        }
    }
}