using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Chronal Conversion")]
    public class Puzzle21 : ASolver {
        private VM program;

        public override void Setup() {
            program = new VM(Input);
        }

        [Description("What is the value of register 0 that causes the program to halt?")]
        public override string SolvePart1() {
            int r3 = 65536;
            int r5 = 733884;
            while (true) {
                r5 += r3 & 255;
                r5 &= 16777215;
                r5 *= 65899;
                r5 &= 16777215;

                if (256 > r3) {
                    return $"{r5}";
                }

                r3 /= 256;
            }
        }

        [Description("What is the value of register 0 that causes the program to halt?")]
        public override string SolvePart2() {
            HashSet<int> values = new HashSet<int>();
            int r0 = 0;
            int r5 = 0;

        line6:
            int r3 = r5 | 65536;
            r5 = 733884;
            while (true) {
                r5 += r3 & 255;
                r5 &= 16777215;
                r5 *= 65899;
                r5 &= 16777215;

                if (256 > r3) {
                    if (!values.Add(r5)) {
                        return $"{r0}";
                    }
                    r0 = r5;
                    goto line6;
                }

                r3 /= 256;
            }
        }
    }
}