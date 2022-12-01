using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Go With The Flow")]
    public class Puzzle19 : ASolver {
        private VM program;

        public override void Setup() {
            program = new VM(Input);
        }

        [Description("What value is left in register 0 when the background process halts?")]
        public override string SolvePart1() {
            program.Run();
            return $"{program.Registers["0"].Value}";
        }

        [Description("What value is left in register 0 when this new background process halts?")]
        public override string SolvePart2() {
            //Takes a while to run even with optimizations
            //Running c# version of it instead

            //program.Reset();
            //program.Registers["0"].Value = 1;

            ////Modify a few instructions to exit out of inner loop earlier
            //program.Instructions[3] = new Instruction() {//mulr 5 2 1 -> mulr 5 2 6
            //    Code = OpCode.mulr,
            //    One = program.Registers["5"],
            //    Two = program.Registers["2"],
            //    Three = program.Registers["6"]
            //};
            //program.Instructions[4] = new Instruction() {//eqrr 1 4 1 -> eqrr 6 4 1
            //    Code = OpCode.eqrr,
            //    One = program.Registers["6"],
            //    Two = program.Registers["4"],
            //    Three = program.Registers["1"]
            //};
            //program.Instructions[9] = new Instruction() {//gtrr 2 4 1 -> gtrr 6 4 1
            //    Code = OpCode.gtrr,
            //    One = program.Registers["6"],
            //    Two = program.Registers["4"],
            //    Three = program.Registers["1"]
            //};

            //program.Run();
            //return $"{program.Registers["0"].Value}";

            return $"{CompiledVersion(1)}";
        }

        private int CompiledVersion(int zero) {
            int four = (2 * 2 * 19 * 11) + (6 * 22 + 10);

            if (zero != 0) {
                four += ((27 * 28) + 29) * 30 * 14 * 32;
                zero = 0;
            }

            int five = 1;
            do {
                int two = 1;
                int one;
                do {
                    one = five * two;
                    if (one == four) {
                        zero += five;
                    }

                    two++;
                } while (one < four);

                five++;
            } while (five <= four);

            return zero;
        }
    }
}