using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Clock Signal")]
    public class Puzzle25 : ASolver {
        [Description("What is the lowest positive integer that can be used to initialize register a?")]
        public override string SolvePart1() {
            VM program = new VM(Input);
            int initA = 0;
            while (true) {
                program.Reset();
                program.A.Value = initA;
                bool ranCorrectly = true;
                int last = int.MaxValue;
                for (int i = 0; i < 50; i++) {
                    program.Run();
                    if (last == program.Output.Value) {
                        ranCorrectly = false;
                        break;
                    }
                    last = program.Output.Value;
                }
                if (ranCorrectly) {
                    return $"{initA}";
                }
                initA++;
            }
        }
    }
}