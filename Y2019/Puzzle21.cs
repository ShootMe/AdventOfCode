using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    public class Puzzle21 : ASolver {
        private IntCode program;
        public Puzzle21(string input) : base(input) { Name = "Springdroid Adventure"; }

        public override void Setup() {
            string[] elements = Input.Split(',');
            long[] code = new long[elements.Length];
            for (int i = 0; i < elements.Length; i++) {
                code[i] = Tools.ParseLong(elements[i]);
            }
            program = new IntCode(code);
        }

        [Description("What amount of hull damage does it report?")]
        public override string SolvePart1() {
            string instructions =
@"NOT A J
NOT C T
AND D T
OR T J
WALK
".Replace("\r", string.Empty);
            for (int i = 0; i < instructions.Length; i++) {
                while (!program.InputRequired && program.Run()) { }
                program.Run(instructions[i]);
            }
            while (program.Run()) {
                //Console.Write((char)program.Output);
            }
            return $"{program.Output}";
        }

        [Description("What amount of hull damage does the springdroid now report?")]
        public override string SolvePart2() {
            program.Reset();
            string instructions =
@"NOT A J
NOT B T 
AND D T 
OR T J
NOT C T
AND D T
AND H T
OR T J
RUN
".Replace("\r", string.Empty);
            for (int i = 0; i < instructions.Length; i++) {
                while (!program.InputRequired && program.Run()) { }
                program.Run(instructions[i]);
            }
            while (program.Run()) {
                //Console.Write((char)program.Output);
            }
            return $"{program.Output}";
        }
    }
}