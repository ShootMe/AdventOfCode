using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Leonardo's Monorail")]
    public class Puzzle12 : ASolver {
        private VM program;

        public override void Setup() {
            program = new VM(Input);
        }

        [Description("What value is left in register a?")]
        public override string SolvePart1() {
            program.Reset();
            program.Run();
            return $"{program.A.Value}";
        }

        [Description("What value is now left in register a?")]
        public override string SolvePart2() {
            program.Reset();
            program.C.Value = 1;
            program.Run();
            return $"{program.A.Value}";
        }
    }
    public class VM {
        public List<Instruction> Instructions;
        public Register A, B, C, D, Output;
        public int Pointer;

        public VM(string program) {
            List<string> lines = Tools.GetLines(program);
            Instructions = new List<Instruction>(lines.Count);
            A = new Register() { Name = "a", Value = 0 };
            B = new Register() { Name = "b", Value = 0 };
            C = new Register() { Name = "c", Value = 0 };
            D = new Register() { Name = "d", Value = 0 };
            Output = new Register() { Name = "out", Value = 0 };

            Dictionary<string, Register> registers = new Dictionary<string, Register>();
            registers.Add("a", A);
            registers.Add("b", B);
            registers.Add("c", C);
            registers.Add("d", D);
            registers.Add("out", Output);

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
                int val2;
                bool val2IsNumeric = int.TryParse(value2, out val2);

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
                            Two = val2IsNumeric ? new Operand() { Value = val2 } : registers[value2],
                        });
                        break;
                    case "tgl":
                        Instructions.Add(new Instruction() {
                            Code = OpCode.tgl,
                            One = val1IsNumeric ? new Operand() { Value = val1 } : registers[value1]
                        });
                        break;
                    case "out":
                        Instructions.Add(new Instruction() {
                            Code = OpCode.trn,
                            One = val1IsNumeric ? new Operand() { Value = val1 } : registers[value1],
                            Two = registers["out"]
                        });
                        break;
                }
            }
        }
        public void Reset() {
            A.Value = 0;
            B.Value = 0;
            C.Value = 0;
            D.Value = 0;
            Output.Value = 0;
            Pointer = 0;
        }
        public void Run() {
            while (Pointer >= 0 && Pointer < Instructions.Count) {
                Instruction instruction = Instructions[Pointer];
                int ptrInc = instruction.Execute(this);
                if (ptrInc == int.MaxValue) {
                    Pointer++;
                    break;
                }
                Pointer += ptrInc;
            }
        }
        public void ToggleInstruction(int relative) {
            if (Pointer + relative >= 0 && Pointer + relative < Instructions.Count) {
                Instruction instruction = Instructions[Pointer + relative];
                switch (instruction.Code) {
                    case OpCode.inc: instruction.Code = OpCode.dec; break;
                    case OpCode.dec:
                    case OpCode.tgl: instruction.Code = OpCode.inc; break;
                    case OpCode.jnz: instruction.Code = OpCode.cpy; break;
                    case OpCode.cpy: instruction.Code = OpCode.jnz; break;
                }
            }
        }
    }
    public enum OpCode {
        cpy,
        inc,
        dec,
        jnz,
        tgl,
        trn
    }
    public class Instruction {
        public OpCode Code;
        public Operand One, Two;
        public int Execute(VM program) {
            switch (Code) {
                case OpCode.cpy: if (Two is Register) { Two.Value = One.Value; } break;
                case OpCode.inc: One.Value++; break;
                case OpCode.dec: One.Value--; break;
                case OpCode.jnz: if (One.Value != 0) { return Two.Value; } break;
                case OpCode.tgl: program.ToggleInstruction(One.Value); break;
                case OpCode.trn: Two.Value = One.Value; return int.MaxValue;
            }
            return 1;
        }
        public override string ToString() {
            return $"{Code} {One} {Two}";
        }
    }
    public class Register : Operand {
        public string Name;
        public override string ToString() {
            return $"{Name} {Value}";
        }
    }
    public class Operand {
        public int Value;
        public override string ToString() { return $"{Value}"; }
    }
}