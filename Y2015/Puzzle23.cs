using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Opening the Turing Lock")]
    public class Puzzle23 : ASolver {
        private VM program;

        public override void Setup() {
            program = new VM(Input);
        }

        [Description("What is the value in register b when the program finishes?")]
        public override string SolvePart1() {
            program.Run();
            return $"{program.B.Value}";
        }

        [Description("What is the value in register b when the program finishes?")]
        public override string SolvePart2() {
            program.ClearRegisters();
            program.A.Value = 1;
            program.Run();
            return $"{program.B.Value}";
        }

        private class VM {
            public List<Instruction> Instructions;
            public Register A, B;
            public int Pointer;

            public VM(string program) {
                List<string> lines = program.Lines();
                Instructions = new List<Instruction>(lines.Count);
                A = new Register() { Name = "a", Value = 0 };
                B = new Register() { Name = "b", Value = 0 };

                Dictionary<string, Register> registers = new Dictionary<string, Register>();
                registers.Add("a", A);
                registers.Add("b", B);

                for (int i = 0; i < lines.Count; i++) {
                    string item = lines[i];

                    int index1 = item.IndexOf(' ');

                    string type = item.Substring(0, index1);

                    int index2 = item.IndexOf(' ', index1 + 1);
                    if (index2 < 0) { index2 = item.Length + 1; }

                    string value1 = item.Substring(index1 + 1, index2 - index1 - 2);
                    string value2 = string.Empty;
                    if (index2 < item.Length) {
                        value2 = item.Substring(index2 + 2);
                    }
                    int val1;
                    bool val1IsNumeric = int.TryParse(value1, out val1);
                    int val2;
                    bool val2IsNumeric = int.TryParse(value2, out val2);

                    switch (type) {
                        case "hlf":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.hlf,
                                One = registers[value1]
                            });
                            break;
                        case "tpl":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.tpl,
                                One = registers[value1]
                            });
                            break;
                        case "inc":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.inc,
                                One = registers[value1]
                            });
                            break;
                        case "jmp":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.jmp,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : registers[value1]
                            });
                            break;
                        case "jie":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.jie,
                                One = registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : registers[value2]
                            });
                            break;
                        case "jio":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.jio,
                                One = registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : registers[value2]
                            });
                            break;
                    }
                }
            }
            public void ClearRegisters() {
                A.Value = 0;
                B.Value = 0;
            }
            public void Run() {
                Pointer = 0;
                while (Pointer >= 0 && Pointer < Instructions.Count) {
                    Instruction instruction = Instructions[Pointer];
                    Pointer += instruction.Execute();
                }
            }
        }
        private enum OpCode {
            hlf,
            tpl,
            inc,
            jmp,
            jie,
            jio
        }
        private class Instruction {
            public OpCode Code;
            public Operand One, Two;
            public int Execute() {
                switch (Code) {
                    case OpCode.hlf: One.Value >>= 1; break;
                    case OpCode.tpl: One.Value *= 3; break;
                    case OpCode.inc: One.Value++; break;
                    case OpCode.jmp: return One.Value;
                    case OpCode.jie: if ((One.Value & 1) == 0) { return Two.Value; } break;
                    case OpCode.jio: if (One.Value == 1) { return Two.Value; } break;
                }
                return 1;
            }
            public override string ToString() {
                return $"{Code} {One} {Two}";
            }
        }
        private class Register : Operand {
            public string Name;
            public override string ToString() {
                return $"{Name} {Value}";
            }
        }
        private class Operand {
            public int Value;
            public override string ToString() { return $"{Value}"; }
        }
    }
}