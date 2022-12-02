using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Handheld Halting")]
    public class Puzzle08 : ASolver {
        private List<string> program;

        public override void Setup() {
            program = Input.Lines();
        }

        [Description("What value is in the accumulator?")]
        public override string SolvePart1() {
            return $"{Run().acc}";
        }

        [Description("What is the value of the accumulator after the program terminates?")]
        public override string SolvePart2() {
            for (int i = 0; i < program.Count; i++) {
                string item = program[i];
                int index1 = item.IndexOf(' ');
                string type = item.Substring(0, index1);
                int value = item.Substring(index1 + 1).ToInt();
                if (type == "jmp") {
                    program[i] = $"nop {value}";
                    var result = Run();
                    if (result.pointer < 0 || result.pointer >= program.Count) {
                        return $"{result.acc}";
                    }
                    program[i] = item;
                } else if (type == "nop") {
                    program[i] = $"jmp {value}";
                    var result = Run();
                    if (result.pointer < 0 || result.pointer >= program.Count) {
                        return $"{result.acc}";
                    }
                    program[i] = item;
                }
            }
            return string.Empty;
        }

        private (int acc, int pointer) Run() {
            int pointer = 0;
            int acc = 0;
            HashSet<int> seen = new HashSet<int>();
            while (pointer >= 0 && pointer < program.Count) {
                if (!seen.Add(pointer)) {
                    break;
                }
                string item = program[pointer];
                int index1 = item.IndexOf(' ');
                string type = item.Substring(0, index1);
                int value = item.Substring(index1 + 1).ToInt();
                switch (type) {
                    case "acc": acc += value; pointer++; break;
                    case "jmp": pointer += value; break;
                    case "nop": pointer++; break;
                }
            }
            return (acc, pointer);
        }
    }
}