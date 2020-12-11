using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    public class Puzzle12 : ASolver {
        private VM program;
        public Puzzle12(string input) : base(input) { Name = "Leonardo&apos;s Monorail"; }

        public override void Setup() {
            program = new VM(Input);
        }

        [Description("What value is left in register a?")]
        public override string SolvePart1() {
            program.ClearRegisters();
            program.Run();
            return $"{program.A.Value}";
        }

        [Description("What is the answer?")]
        public override string SolvePart2() {
            program.ClearRegisters();
            program.C.Value = 1;
            program.Run();
            return $"{program.A.Value}";
        }

        private class VM {
            public List<Instruction> Instructions;
            public Register A, B, C, D;
            public int Pointer;

            public VM(string program) {
                List<string> lines = Tools.GetLines(program);
                Instructions = new List<Instruction>(lines.Count);
                A = new Register() { Name = "a", Value = 0 };
                B = new Register() { Name = "b", Value = 0 };
                C = new Register() { Name = "c", Value = 0 };
                D = new Register() { Name = "d", Value = 0 };

                Dictionary<string, Register> registers = new Dictionary<string, Register>();
                registers.Add("a", A);
                registers.Add("b", B);
                registers.Add("c", C);
                registers.Add("d", D);

                for (int i = 0; i < lines.Count; i++) {
                    string item = lines[i];

                    int index1 = item.IndexOf(' ');

                    string type = item.Substring(0, index1);

                    int index2 = item.IndexOf(' ', index1 + 1);
                    if (index2 < 0) { index2 = item.Length; }

                    string value1 = item.Substring(index1 + 1, index2 - index1 - 1);
                    string value2 = string.Empty;
                    if (index2 < item.Length) {
                        value2 = item.Substring(index2 + 1);
                    }
                    int val1;
                    bool val1IsNumeric = int.TryParse(value1, out val1);

                    switch (type) {
                        case "cpy":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.cpy,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : registers[value1],
                                Two = registers[value2]
                            });
                            break;
                        case "inc":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.inc,
                                One = registers[value1]
                            });
                            break;
                        case "dec":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.dec,
                                One = registers[value1]
                            });
                            break;
                        case "jnz":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.jnz,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : registers[value1],
                                Two = new Operand() { Value = int.Parse(value2) }
                            });
                            break;
                    }
                }
            }
            public void ClearRegisters() {
                A.Value = 0;
                B.Value = 0;
                C.Value = 0;
                D.Value = 0;
            }
            public void Run() {
                Pointer = 0;
                while(Pointer>=0 && Pointer < Instructions.Count) {
                    Instruction instruction = Instructions[Pointer];
                    Pointer += instruction.Execute();
                }
            }
        }
        private enum OpCode {
            cpy,
            inc,
            dec,
            jnz
        }
        private class Instruction {
            public OpCode Code;
            public Operand One, Two;
            public int Execute() {
                switch (Code) {
                    case OpCode.cpy: Two.Value = One.Value; break;
                    case OpCode.inc: One.Value++; break;
                    case OpCode.dec: One.Value--; break;
                    case OpCode.jnz: if (One.Value != 0) { return Two.Value; } break;
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
            public virtual int Fetch() { return Value; }
        }
    }
}