using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Duet")]
    public class Puzzle18 : ASolver {
        private VM program0, program1;

        public override void Setup() {
            program0 = new VM(Input, 0);
            program1 = new VM(Input, 1);
        }

        [Description("What is the value of the recovered frequency?")]
        public override string SolvePart1() {
            program0.Run();
            return $"{program0.OutPort.Value}";
        }

        [Description("How many times did program 1 send a value?")]
        public override string SolvePart2() {
            program0.Reset();
            program0.InPort = program1.OutPort;
            program1.InPort = program0.OutPort;

            while (program0.Run()) {
                bool sentData = program0.OutPort.HasData;
                program1.Run();
                if (!sentData && !program1.OutPort.HasData) { break; }
            }
            return $"{program1.Sent}";
        }

        private class VM {
            public List<Instruction> Instructions;
            public Dictionary<string, Register> Registers;
            public int Sent, Received;
            public Port InPort, OutPort;
            public int Pointer;
            public bool InputNeeded;

            public VM(string program, int id) {
                List<string> lines = program.Lines();
                Instructions = new List<Instruction>(lines.Count);
                Registers = new Dictionary<string, Register>();

                InPort = new Port();
                OutPort = new Port();
                Registers.Add("p", new Register() { Name = "p", Value = id });

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
                        case "snd":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.snd,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1]
                            });
                            break;
                        case "set":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.set,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : Registers[value2]
                            });
                            break;
                        case "add":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.add,
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
                        case "mod":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.mod,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : Registers[value2]
                            });
                            break;
                        case "rcv":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.rcv,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1]
                            });
                            break;
                        case "jgz":
                            Instructions.Add(new Instruction() {
                                Code = OpCode.jgz,
                                One = val1IsNumeric ? new Operand() { Value = val1 } : Registers[value1],
                                Two = val2IsNumeric ? new Operand() { Value = val2 } : Registers[value2]
                            });
                            break;
                    }
                }
            }
            public void Send(long value) {
                OutPort.Send(value);
                Sent++;
            }
            public long Receive() {
                long value = InPort.Receive();
                Received++;
                return value;
            }
            public void Reset() {
                InputNeeded = false;
                Pointer = 0;
                Sent = 0;
                Received = 0;
                InPort.Values.Clear();
                OutPort.Values.Clear();
                foreach (Register register in Registers.Values) {
                    register.Reset();
                }
            }
            public bool Run() {
                InputNeeded = false;
                while (Pointer >= 0 && Pointer < Instructions.Count) {
                    Instruction instruction = Instructions[Pointer];
                    int inc = instruction.Execute(this);
                    if (inc == int.MaxValue) {
                        InputNeeded = true;
                        return true;
                    }
                    Pointer += inc;
                }
                return false;
            }
        }
        private enum OpCode {
            snd,
            set,
            add,
            mul,
            mod,
            rcv,
            jgz
        }
        private class Instruction {
            public OpCode Code;
            public Operand One, Two;
            public int Execute(VM program) {
                switch (Code) {
                    case OpCode.snd: program.Send(One.Value); break;
                    case OpCode.set: One.Value = Two.Value; break;
                    case OpCode.add: One.Value += Two.Value; break;
                    case OpCode.mul: One.Value *= Two.Value; break;
                    case OpCode.mod: One.Value %= Two.Value; break;
                    case OpCode.rcv:
                        if (!program.InPort.HasData) { return int.MaxValue; }
                        One.Value = program.Receive();
                        break;
                    case OpCode.jgz: if (One.Value > 0) { return (int)Two.Value; } break;
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
        private class Port {
            public long Value;
            public Queue<long> Values = new Queue<long>();
            public bool HasData {
                get { return Values.Count > 0; }
            }
            public long Receive() {
                if (HasData) {
                    Value = Values.Dequeue();
                }
                return Value;
            }
            public void Send(long value) {
                Values.Enqueue(value);
                Value = value;
            }
            public void Reset() { Values.Clear(); }
        }
        private class Operand {
            public long Value;
            public override string ToString() { return $"{Value}"; }
        }
    }
}