using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Sunny with a Chance of Asteroids")]
    public class Puzzle05 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Input.ToLongs(','));
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