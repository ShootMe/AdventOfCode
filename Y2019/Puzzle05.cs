using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    public class Puzzle05 : ASolver {
        private IntCode program;
        public Puzzle05(string input) : base(input) { Name = "Sunny with a Chance of Asteroids"; }

        public override void Setup() {
            string[] elements = Input.Split(',');
            int[] code = new int[elements.Length];
            for (int i = 0; i < elements.Length; i++) {
                code[i] = Tools.ParseInt(elements[i]);
            }
            program = new IntCode(code);
        }

        [Description("What diagnostic code does the program produce?")]
        public override string SolvePart1() {
            program.Reset();

            program.Run(1);
            while (program.Run(program.Output)) { }
            return $"{program.Output}";
        }

        [Description("What is the diagnostic code for system ID 5?")]
        public override string SolvePart2() {
            program.Reset();

            program.Run(5);
            while (program.Run(program.Output)) { }
            return $"{program.Output}";
        }
    }
}