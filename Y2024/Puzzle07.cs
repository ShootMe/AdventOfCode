using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Bridge Repair")]
    public class Puzzle07 : ASolver {
        private List<Formula> formulas = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                formulas.Add(new Formula(line));
            }
        }

        [Description("What is their total calibration result?")]
        public override string SolvePart1() {
            long total = 0;
            for (int i = 0; i < formulas.Count; i++) {
                Formula formula = formulas[i];
                total += formula.HasValidOperators() ? formula.Value : 0L;
            }
            return $"{total}";
        }

        [Description("What is their total calibration result?")]
        public override string SolvePart2() {
            long total = 0;
            for (int i = 0; i < formulas.Count; i++) {
                Formula formula = formulas[i];
                total += formula.HasValidOperators(true) ? formula.Value : 0L;
            }
            return $"{total}";
        }

        private class Formula {
            public long Value;
            public List<int> Operands = new();
            public Formula(string formula) {
                int index = formula.IndexOf(':');
                Value = formula[0..index].ToLong();
                string[] operands = formula[(index + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < operands.Length; i++) {
                    Operands.Add(operands[i].ToInt());
                }
            }
            public bool HasValidOperators(bool use3 = false) {
                return TryNext(1, Operands[0], use3) == Value;
            }
            private long TryNext(int index, long value, bool use3) {
                if (index < Operands.Count) {
                    long operand = Operands[index];
                    if (TryNext(index + 1, value + operand, use3) == Value || TryNext(index + 1, value * operand, use3) == Value) {
                        return Value;
                    }
                    if (use3) {
                        if (TryNext(index + 1, value * GetBase10(operand) + operand, use3) == Value) {
                            return Value;
                        }
                    }
                } else {
                    return value;
                }
                return -value;
            }
            private static long GetBase10(long num) {
                int i = 1;
                while (i <= num) {
                    i *= 10;
                }
                return i;
            }
        }
    }
}