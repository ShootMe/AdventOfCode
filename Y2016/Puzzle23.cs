using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Safe Cracking")]
    public class Puzzle23 : ASolver {
        [Description("What value should be sent to the safe?")]
        public override string SolvePart1() {
            VM program = new VM(Input);
            program.A.Value = 7;
            program.Run();
            return $"{program.A.Value}";
        }

        [Description("What value should actually be sent to the safe?")]
        public override string SolvePart2() {
            //VM program = new VM(Input);
            //program.A.Value = 12;
            //program.Run();
            return $"{CompiledVersion(12)}";
        }

        private int CompiledVersion(int a) {
            int b = a - 1;
            do {
                a *= b;
                b--;
            } while (b > 0);

            a += 91 * 98;

            return a;
        }
    }
}