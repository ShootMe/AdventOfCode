using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Coprocessor Conflagration")]
    public class Puzzle23 : ASolver {
        private VM program;

        public override void Setup() {
            program = new VM(Input);
        }

        [Description("How many times is the mul instruction invoked?")]
        public override string SolvePart1() {
            program.Run();
            return $"{program.Multiplications}";
        }

        [Description("What value would be left in register h?")]
        public override string SolvePart2() {
            return $"{CompiledVersion(1)}";
        }

        private int CompiledVersion(int a) {
            int b = 99;
            int c = b;

            if (a != 0) {
                b = b * 100 + 100000;
                c = b + 17000;
            }

            int h = 0;
            do {
                if ((b & 1) == 1) {
                    int max = (int)Math.Sqrt(b);
                    for (int i = 3; i <= max; i += 2) {
                        if ((b % i) == 0) {
                            h++;
                            break;
                        }
                    }
                } else {
                    h++;
                }

                if (b == c) {
                    return h;
                }

                b += 17;
            } while (true);
        }

        private class VM {
            public List<Instruction> Instructions;
            public Dictionary<string, Register> Registers;
            public int Pointer;
            public int Multiplications;

            public VM(string program) {
                List<string> lines = program.Lines();
                Instructions = new List<Instruction>(lines.Count);
                Registers = new Dictionary<string, Register>();

                for (int i = 0; i < lines.Count; i++) {
                    string item = lines[i];

                    int index1 = item.IndexOf(' ');

                    string type = item.Substring(0, index1);

                    int index2 = item.IndexOf(' ', index1 + 1);
                    if (index2 < 0) { index2 = item.Length; }

                    string value1 = item.Substring(index1 + 1, index2 - index1 - 1);
                    int val1;
                    bool val1IsNumeric = int.TryParse(value1, out val1);

                    string value2 = string.Empty;
                    if (index2 < item.Length) {
                        value2 = item.Substring(index2 + 1);
                    }
                    int val2;
                    bool val2IsNumeric = int.TryParse(value2, out val2);

                    if (!val1IsNumeric && !Registers.ContainsKey(value1)) {
                        Registers.Add(value1, new Register() { Name = value1 });
                    }
                    if (!string.IsNullOrEmpty(value2) && !val2IsNumeric && !Registers.ContainsKey(value2)) {
                        Registers.Add(value2, new Register() { Name = value2 });
                    }

                    switch (type) {
                        case "set":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.set,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : Registers[value2]
                            });
                            break;
                        case "sub":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.sub,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : Registers[value2]
                            });
                            break;
                        case "mul":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.mul,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : Registers[value2]
                            });
                            break;
                        case "jnz":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.jnz,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : Registers[value2]
                            });
                            break;
                    }
                }
            }
            public void Reset() {
                Pointer = 0;
                Multiplications = 0;
                foreach (Register register in Registers.Values) {
                    register.Reset();
                }
            }
            public bool Run() {
                while (Pointer >= 0 && Pointer < Instructions.Count) {
                    Instruction instruction = Instructions[Pointer];
                    if (instruction.Code == OpCode.mul) {
                        Multiplications++;
                    }
                    int inc = instruction.Execute();
                    Pointer += inc;
                }
                return false;
            }
        }
        private enum OpCode {
            set,
            sub,
            mul,
            jnz
        }
        private class Instruction {
            public OpCode Code;
            public Operand One, Two;
            public int Execute() {
                switch (Code) {
                    case OpCode.set: One.Value = Two.Value; break;
                    case OpCode.sub: One.Value -= Two.Value; break;
                    case OpCode.mul: One.Value *= Two.Value; break;
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
            public override string ToString() { return $"{Name} {Value}"; }
            public virtual void Reset() { Value = 0; }
        }
        private class Operand {
            public int Value;
            public override string ToString() { return $"{Value}"; }
        }
    }
}