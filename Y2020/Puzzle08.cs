using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    public class Puzzle08 : ASolver {
        private List<string> program;
        public Puzzle08(string input) : base(input) { Name = "Handheld Halting"; }

        public override void Setup() {
            program = Tools.GetLines(Input);
        }

        [Description("What value is in the accumulator?")]
        public override string SolvePart1() {
            return $"{Run().Item1}";
        }

        [Description("What is the value of the accumulator after the program terminates?")]
        public override string SolvePart2() {
            for (int i = 0; i < program.Count; i++) {
                string item = program[i];
                int index1 = item.IndexOf(' ');
                string type = item.Substring(0, index1);
                int value = Tools.ParseInt(item.Substring(index1 + 1));
                if (type == "jmp") {
                    program[i] = $"nop {value}";
                    Tuple<int, int> result = Run();
                    if (result.Item2 < 0 || result.Item2 >= program.Count) {
                        return $"{result.Item1}";
                    }
                    program[i] = item;
                } else if (type == "nop") {
                    program[i] = $"jmp {value}";
                    Tuple<int, int> result = Run();
                    if (result.Item2 < 0 || result.Item2 >= program.Count) {
                        return $"{result.Item1}";
                    }
                    program[i] = item;
                }
            }
            return string.Empty;
        }

        private Tuple<int, int> Run() {
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
                int value = Tools.ParseInt(item.Substring(index1 + 1));
                switch (type) {
                    case "acc": acc += value; pointer++; break;
                    case "jmp": pointer += value; break;
                    case "nop": pointer++; break;
                }
            }
            return new Tuple<int, int>(acc, pointer);
        }
    }
}