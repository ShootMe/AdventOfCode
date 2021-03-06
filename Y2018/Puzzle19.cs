using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    public class Puzzle19 : ASolver {
        private VM program;
        public Puzzle19(string input) : base(input) { Name = "Go With The Flow"; }

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
        private class VM {
            public List<Instruction> Instructions;
            public Dictionary<string, Register> Registers;
            public string Pointer;

            public VM(string program) {
                List<string> lines = Tools.GetLines(program);
                Instructions = new List<Instruction>(lines.Count);
                Registers = new Dictionary<string, Register>();
                for (int i = 0; i < 7; i++) {
                    Registers.Add($"{i}", new Register() { Name = $"{i}" });
                }

                Pointer = lines[0].Substring(4);

                for (int i = 1; i < lines.Count; i++) {
                    string item = lines[i];

                    int index1 = item.IndexOf(' ');
                    string type = item.Substring(0, index1);

                    int index2 = item.IndexOf(' ', index1 + 1);
                    string value1 = item.Substring(index1 + 1, index2 - index1 - 1);
                    int val1 = Tools.ParseInt(value1);

                    int index3 = item.IndexOf(' ', index2 + 1);
                    string value2 = item.Substring(index2 + 1, index3 - index2 - 1);
                    int val2 = Tools.ParseInt(value2);
                    string value3 = item.Substring(index3 + 1);

                    switch (type) {
                        case "addr":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.addr,
                                One = Registers[value1],
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                        case "addi":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.addi,
                                One = Registers[value1],
                                Two = new Operand() { Value = val2 },
                                Three = Registers[value3]
                            });
                            break;
                        case "mulr":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.mulr,
                                One = Registers[value1],
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                        case "muli":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.muli,
                                One = Registers[value1],
                                Two = new Operand() { Value = val2 },
                                Three = Registers[value3]
                            });
                            break;
                        case "banr":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.banr,
                                One = Registers[value1],
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                        case "bani":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.bani,
                                One = Registers[value1],
                                Two = new Operand() { Value = val2 },
                                Three = Registers[value3]
                            });
                            break;
                        case "borr":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.borr,
                                One = Registers[value1],
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                        case "bori":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.bori,
                                One = Registers[value1],
                                Two = new Operand() { Value = val2 },
                                Three = Registers[value3]
                            });
                            break;
                        case "setr":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.setr,
                                One = Registers[value1],
                                Three = Registers[value3]
                            });
                            break;
                        case "seti":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.seti,
                                One = new Operand() { Value = val1 },
                                Three = Registers[value3]
                            });
                            break;
                        case "gtir":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.gtir,
                                One = new Operand() { Value = val1 },
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                        case "gtri":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.gtri,
                                One = Registers[value1],
                                Two = new Operand() { Value = val2 },
                                Three = Registers[value3]
                            });
                            break;
                        case "gtrr":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.gtrr,
                                One = Registers[value1],
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                        case "eqir":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.eqir,
                                One = new Operand() { Value = val1 },
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                        case "eqri":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.eqri,
                                One = Registers[value1],
                                Two = new Operand() { Value = val2 },
                                Three = Registers[value3]
                            });
                            break;
                        case "eqrr":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.eqrr,
                                One = Registers[value1],
                                Two = Registers[value2],
                                Three = Registers[value3]
                            });
                            break;
                    }
                }
            }
            public void Reset() {
                foreach (Register register in Registers.Values) {
                    register.Reset();
                }
            }
            public void Run() {
                Register pointer = Registers[Pointer];
                while (pointer.Value >= 0 && pointer.Value < Instructions.Count) {
                    Instructions[pointer.Value].Execute();
                    pointer.Value++;
                }
            }
        }
        private enum OpCode {
            addr,
            addi,
            mulr,
            muli,
            banr,
            bani,
            borr,
            bori,
            setr,
            seti,
            gtir,
            gtri,
            gtrr,
            eqir,
            eqri,
            eqrr
        }
        private class Instruction {
            public OpCode Code;
            public Operand One, Two, Three;
            public void Execute() {
                switch (Code) {
                    case OpCode.addr:
                    case OpCode.addi: Three.Value = One.Value + Two.Value; break;
                    case OpCode.mulr:
                    case OpCode.muli: Three.Value = One.Value * Two.Value; break;
                    case OpCode.banr:
                    case OpCode.bani: Three.Value = One.Value & Two.Value; break;
                    case OpCode.borr:
                    case OpCode.bori: Three.Value = One.Value | Two.Value; break;
                    case OpCode.setr:
                    case OpCode.seti: Three.Value = One.Value; break;
                    case OpCode.gtir:
                    case OpCode.gtri:
                    case OpCode.gtrr: Three.Value = One.Value > Two.Value ? 1 : 0; break;
                    case OpCode.eqir:
                    case OpCode.eqri:
                    case OpCode.eqrr: Three.Value = One.Value == Two.Value ? 1 : 0; break;
                }
            }
            public override string ToString() {
                switch (Code) {
                    case OpCode.addr:
                    case OpCode.addi: return $"{Three.Simple()} = {One.Simple()} + {Two.Simple()}";
                    case OpCode.mulr:
                    case OpCode.muli: return $"{Three.Simple()} = {One.Simple()} * {Two.Simple()}";
                    case OpCode.banr:
                    case OpCode.bani: return $"{Three.Simple()} = {One.Simple()} & {Two.Simple()}";
                    case OpCode.borr:
                    case OpCode.bori: return $"{Three.Simple()} = {One.Simple()} | {Two.Simple()}";
                    case OpCode.setr:
                    case OpCode.seti: return $"{Three.Simple()} = {One.Simple()}";
                    case OpCode.gtir:
                    case OpCode.gtri:
                    case OpCode.gtrr: return $"{Three.Simple()} = {One.Simple()} > {Two.Simple()} ? 1 : 0";
                    case OpCode.eqir:
                    case OpCode.eqri:
                    case OpCode.eqrr: return $"{Three.Simple()} = {One.Simple()} == {Two.Simple()} ? 1 : 0";
                }
                return $"{Code} {One.Simple()} {Two.Simple()} {Three.Simple()}";
            }
        }
        private class Register : Operand {
            public string Name;
            public override string Simple() { return $"[{Name}]"; }
            public override string ToString() { return $"[{Name}={Value}]"; }
            public virtual void Reset() { Value = 0; }
        }
        private class Operand {
            public int Value;
            public virtual string Simple() { return $"{Value}"; }
            public override string ToString() { return $"{Value}"; }
        }
    }
}