using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
namespace AdventOfCode.Synacor {
    public class Emulator {
        private const int MAX_NUM = 32767;
        private const int REGISTER_0 = 32768;
        private readonly uint[] memory;
        private readonly Stack<uint> stack;
        private uint instruction;

        public static void RunChallenge() {
            byte[] data = File.ReadAllBytes("Synacor/challenge.bin");
            uint[] program = new uint[data.Length >> 1];
            for (int i = 0; i < program.Length; i++) {
                program[i] = BitConverter.ToUInt16(data, i * 2);
            }

            //Console.WriteLine(Ackermann.FindValue());
            //Console.WriteLine(Vault.FindPath());

            Emulator emu = new Emulator(program);
            emu.Run();
        }
        public Emulator(uint[] binary) {
            memory = new uint[32768 + 8];
            Array.Copy(binary, memory, binary.Length);
            stack = new Stack<uint>();
            Reset();
        }
        public uint this[int index] {
            get { return memory[index]; }
            set { memory[index] = value; }
        }
        public void Reset() {
            stack.Clear();
            instruction = 0;
        }
        public void Run() {
            uint backupInstruction = 0;
            uint[] backup = new uint[memory.Length];
            uint[] backupStack = null;

            memory[5451] = (uint)OpCode.JumpTrue; //change jf to jt to change default tele operation
            memory[5485] = 6; // change set [0] 4 to set [0] 6
            memory[5487] = REGISTER_0 + 7; //change set [1] 1 to set [7] 25734
            memory[5488] = 25734;
            memory[5489] = (uint)OpCode.NoOperation; //change call 6027 to noop
            memory[5490] = (uint)OpCode.NoOperation;

            byte[] solution = Encoding.ASCII.GetBytes(
@"take tablet
use tablet
doorway
north
north
bridge
continue
down
east
take empty lantern
west
west
passage
ladder
west
south
north
take can
use can
use lantern
west
ladder
darkness
continue
west
west
west
west
north
take red coin
north
east
take concave coin
down
take corroded coin
up
west
west
take blue coin
up
take shiny coin
down
east
use blue coin
use red coin
use shiny coin
use concave coin
use corroded coin
north
take teleporter
use teleporter
north
north
north
north
north
north
north
east
take journal
west
north
north
take orb
north
east
east
north
west
south
east
east
west
north
north
east
vault
take mirror
use mirror
");
            int solutionIndex = 0;

        StartWhile:
            do {
                uint code = memory[instruction++];
                uint operand1, operand2, operand3;

                switch ((OpCode)code) {
                    case OpCode.Halt:
                        goto ExitWhile;
                    case OpCode.Set:
                        operand1 = memory[instruction++];
                        SetValue(operand1, GetValue(memory[instruction++]));
                        break;
                    case OpCode.Push:
                        stack.Push(GetValue(memory[instruction++]));
                        break;
                    case OpCode.Pop:
                        if (stack.Count == 0) { throw new Exception("Failed to pop stack"); }

                        SetValue(memory[instruction++], stack.Pop());
                        break;
                    case OpCode.Equals:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        operand3 = GetValue(memory[instruction++]);
                        SetValue(operand1, operand2 == operand3 ? 1u : 0);
                        break;
                    case OpCode.GreaterThan:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        operand3 = GetValue(memory[instruction++]);
                        SetValue(operand1, operand2 > operand3 ? 1u : 0);
                        break;
                    case OpCode.Jump:
                        instruction = GetValue(memory[instruction]);
                        break;
                    case OpCode.JumpTrue:
                        operand1 = GetValue(memory[instruction++]);
                        operand2 = GetValue(memory[instruction++]);
                        if (operand1 != 0) {
                            instruction = operand2;
                        }
                        break;
                    case OpCode.JumpFalse:
                        operand1 = GetValue(memory[instruction++]);
                        operand2 = GetValue(memory[instruction++]);
                        if (operand1 == 0) {
                            instruction = operand2;
                        }
                        break;
                    case OpCode.Add:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        operand3 = GetValue(memory[instruction++]);
                        SetValue(operand1, (operand2 + operand3) & MAX_NUM);
                        break;
                    case OpCode.Multiply:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        operand3 = GetValue(memory[instruction++]);
                        SetValue(operand1, (operand2 * operand3) & MAX_NUM);
                        break;
                    case OpCode.Mod:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        operand3 = GetValue(memory[instruction++]);
                        SetValue(operand1, operand2 % operand3);
                        break;
                    case OpCode.And:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        operand3 = GetValue(memory[instruction++]);
                        SetValue(operand1, operand2 & operand3);
                        break;
                    case OpCode.Or:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        operand3 = GetValue(memory[instruction++]);
                        SetValue(operand1, operand2 | operand3);
                        break;
                    case OpCode.Not:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        SetValue(operand1, (~operand2) & MAX_NUM);
                        break;
                    case OpCode.Read:
                        operand1 = memory[instruction++];
                        operand2 = GetValue(memory[instruction++]);
                        SetValue(operand1, memory[operand2]);
                        break;
                    case OpCode.Write:
                        operand1 = GetValue(memory[instruction++]);
                        operand2 = GetValue(memory[instruction++]);
                        memory[operand1] = operand2;
                        break;
                    case OpCode.Call:
                        stack.Push(instruction + 1);
                        instruction = GetValue(memory[instruction]);
                        break;
                    case OpCode.Return:
                        if (stack.Count == 0) { goto ExitWhile; }

                        instruction = stack.Pop();
                        break;
                    case OpCode.Output:
                        operand1 = GetValue(memory[instruction++]);
                        Console.Write((char)operand1);
                        break;
                    case OpCode.Input:
                        operand1 = memory[instruction++];
                        if (operand1 > MAX_NUM) {
                            if (solutionIndex < solution.Length) {
                                byte c = solution[solutionIndex++];
                                if (c != 13) {
                                    memory[operand1] = c;
                                } else {
                                    instruction -= 2;
                                }
                            } else {
                                ConsoleKeyInfo key = Console.ReadKey();
                                if (key.KeyChar == '\r') {
                                    memory[operand1] = '\n';
                                } else if (key.Key == ConsoleKey.Home) {
                                    Console.WriteLine("Saved Backup");
                                    instruction -= 2;
                                    backupInstruction = instruction;
                                    Array.Copy(memory, backup, memory.Length);
                                    backupStack = stack.ToArray();
                                } else if (key.Key == ConsoleKey.End) {
                                    Console.WriteLine("Loaded Backup");
                                    instruction = backupInstruction;
                                    Array.Copy(backup, memory, memory.Length);
                                    if (backupStack != null) {
                                        stack.Clear();
                                        for (int i = backupStack.Length - 1; i >= 0; i--) {
                                            stack.Push(backupStack[i]);
                                        }
                                    }
                                } else {
                                    memory[operand1] = key.KeyChar;
                                }
                            }
                        }
                        break;
                }
            } while (true);

        ExitWhile:
            Console.WriteLine("Continue from backup?");
            string result = Console.ReadLine();
            if (char.ToLower(result[0]) == 'y') {
                instruction = backupInstruction;
                Array.Copy(backup, memory, memory.Length);
                if (backupStack != null) {
                    stack.Clear();
                    for (int i = backupStack.Length - 1; i >= 0; i--) {
                        stack.Push(backupStack[i]);
                    }
                }
                goto StartWhile;
            }
        }
        public string Dissassemble() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= MAX_NUM; i++) {
                uint code = memory[i];

                switch ((OpCode)code) {
                    case OpCode.Halt:
                        sb.AppendLine($"{{{i,5}}}: halt");
                        break;
                    case OpCode.Set:
                        sb.AppendLine($"{{{i,5}}}: set {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])}");
                        i += 2;
                        break;
                    case OpCode.Push:
                        sb.AppendLine($"{{{i,5}}}: push {DisplayOperand(memory[i + 1])}");
                        i++;
                        break;
                    case OpCode.Pop:
                        sb.AppendLine($"{{{i,5}}}: pop {DisplayOperand(memory[i + 1])}");
                        i++;
                        break;
                    case OpCode.Equals:
                        sb.AppendLine($"{{{i,5}}}: eq {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])} == {DisplayOperand(memory[i + 3])}? 1 : 0");
                        i += 3;
                        break;
                    case OpCode.GreaterThan:
                        sb.AppendLine($"{{{i,5}}}: gt {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])} > {DisplayOperand(memory[i + 3])}? 1 : 0");
                        i += 3;
                        break;
                    case OpCode.Jump:
                        sb.AppendLine($"{{{i,5}}}: jmp {DisplayOperand(memory[i + 1])}");
                        i++;
                        break;
                    case OpCode.JumpTrue:
                        sb.AppendLine($"{{{i,5}}}: jt {DisplayOperand(memory[i + 1])} != 0 : {DisplayOperand(memory[i + 2])}");
                        i += 2;
                        break;
                    case OpCode.JumpFalse:
                        sb.AppendLine($"{{{i,5}}}: jf {DisplayOperand(memory[i + 1])} == 0 : {DisplayOperand(memory[i + 2])}");
                        i += 2;
                        break;
                    case OpCode.Add:
                        sb.AppendLine($"{{{i,5}}}: add {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])} + {DisplayOperand(memory[i + 3])}");
                        i += 3;
                        break;
                    case OpCode.Multiply:
                        sb.AppendLine($"{{{i,5}}}: mul {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])} * {DisplayOperand(memory[i + 3])}");
                        i += 3;
                        break;
                    case OpCode.Mod:
                        sb.AppendLine($"{{{i,5}}}: mod {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])} % {DisplayOperand(memory[i + 3])}");
                        i += 3;
                        break;
                    case OpCode.And:
                        sb.AppendLine($"{{{i,5}}}: and {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])} & {DisplayOperand(memory[i + 3])}");
                        i += 3;
                        break;
                    case OpCode.Or:
                        sb.AppendLine($"{{{i,5}}}: or {DisplayOperand(memory[i + 1])} = {DisplayOperand(memory[i + 2])} | {DisplayOperand(memory[i + 3])}");
                        i += 3;
                        break;
                    case OpCode.Not:
                        sb.AppendLine($"{{{i,5}}}: not {DisplayOperand(memory[i + 1])} = ~{DisplayOperand(memory[i + 2])}");
                        i += 2;
                        break;
                    case OpCode.Read:
                        sb.AppendLine($"{{{i,5}}}: rmem {DisplayOperand(memory[i + 1])} = memory{{{DisplayOperand(memory[i + 2])}}}");
                        i += 2;
                        break;
                    case OpCode.Write:
                        sb.AppendLine($"{{{i,5}}}: wmem memory{{{DisplayOperand(memory[i + 1])}}} = {DisplayOperand(memory[i + 2])}");
                        i += 2;
                        break;
                    case OpCode.Call:
                        sb.AppendLine($"{{{i,5}}}: call {DisplayOperand(memory[i + 1])}");
                        i++;
                        break;
                    case OpCode.Return:
                        sb.AppendLine($"{{{i,5}}}: ret");
                        break;
                    case OpCode.Output:
                        uint value = memory[i + 1];
                        string output = value < 256 ? $"'{(char)value}'" : DisplayOperand(value);
                        sb.AppendLine($"{{{i,5}}}: out {output}");
                        i++;
                        break;
                    case OpCode.Input:
                        sb.AppendLine($"{{{i,5}}}: in {DisplayOperand(memory[i + 1])}");
                        i++;
                        break;
                    case OpCode.NoOperation:
                        sb.AppendLine($"{{{i,5}}}: noop");
                        break;
                    default:
                        if (code > 31 && code < 128) {
                            sb.AppendLine($"{{{i,5}}}: {(char)code}");
                        } else {
                            sb.AppendLine($"{{{i,5}}}: {code:X}");
                        }
                        break;
                }
            }
            return sb.ToString();
        }
        private string DisplayOperand(uint operand) {
            return operand > MAX_NUM ? $"[{operand - REGISTER_0}]" : $"{operand}";
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint GetValue(uint value) {
            return value > MAX_NUM ? memory[value] : value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetValue(uint address, uint value) {
            if (address > MAX_NUM) {
                memory[address] = value;
            }
        }
        private class Vault {
            private static int[] operators = new int[] { 1, 0, 2, 0, 0, 1, 0, 1, 3, 0, 2, 0, 0, 2, 0, 1 };
            private static int[] values = new int[] { 0, 8, 0, 1, 4, 0, 11, 0, 0, 4, 0, 18, 22, 0, 9, 0 };
            public static string FindPath() {
                HashSet<State> closed = new HashSet<State>();
                Heap<State> open = new Heap<State>();
                State current = new State() { Position = 12, Value = 22 };
                open.Enqueue(current);
                closed.Add(current);

                while (open.Count > 0) {
                    current = open.Dequeue();
                    if (current.Position == 3) {
                        if (current.Value == 30) {
                            char[] path = current.ToString().ToCharArray();
                            Array.Reverse(path);
                            return new string(path);
                        }
                        continue;
                    }

                    uint x = current.Position & 3;
                    uint y = current.Position >> 2;
                    if (x > 0) {
                        State next = new State(current);
                        next.Position--;
                        next.Value = GetNextValue(next);
                        if (next.Position != 12 && closed.Add(next)) {
                            open.Enqueue(next);
                        }
                    }
                    if (x < 3) {
                        State next = new State(current);
                        next.Position++;
                        next.Value = GetNextValue(next);
                        if (next.Position != 12 && closed.Add(next)) {
                            open.Enqueue(next);
                        }
                    }
                    if (y > 0) {
                        State next = new State(current);
                        next.Position -= 4;
                        next.Value = GetNextValue(next);
                        if (next.Position != 12 && closed.Add(next)) {
                            open.Enqueue(next);
                        }
                    }
                    if (y < 3) {
                        State next = new State(current);
                        next.Position += 4;
                        next.Value = GetNextValue(next);
                        if (next.Position != 12 && closed.Add(next)) {
                            open.Enqueue(next);
                        }
                    }
                }
                return string.Empty;
            }
            private static uint GetNextValue(State next) {
                int op;
                if (next.Previous == null || (op = operators[next.Previous.Position]) == 0) { return next.Value; }

                switch (op) {
                    case 1: return (uint)((next.Value * values[next.Position]) & 32767);
                    case 2: return (uint)((next.Value - values[next.Position]) & 32767);
                    case 3: return (uint)((next.Value + values[next.Position]) & 32767);
                }
                return next.Value;
            }
            private class State : IComparable<State>, IEquatable<State> {
                public uint Position, Value;
                public int Steps;
                public State Previous;

                public State() { }
                public State(State copy) {
                    Position = copy.Position;
                    Value = copy.Value;
                    Steps = copy.Steps + 1;
                    Previous = copy;
                }
                public int CompareTo(State other) {
                    int compare = Steps.CompareTo(other.Steps);
                    if (compare != 0) { return compare; }
                    return Position.CompareTo(other.Position);
                }
                public override string ToString() {
                    StringBuilder sb = new StringBuilder();
                    State current = this;
                    while (current != null) {
                        State prev = current.Previous;
                        if (prev != null) {
                            if (prev.Position + 1 == current.Position) {
                                sb.Append("E ");
                            } else if (prev.Position - 1 == current.Position) {
                                sb.Append("W ");
                            } else if (prev.Position - 4 == current.Position) {
                                sb.Append("N ");
                            } else {
                                sb.Append("S ");
                            }
                        }
                        current = prev;
                    }
                    if (sb.Length > 0) { sb.Length--; }
                    return sb.ToString();
                }
                public override bool Equals(object obj) {
                    return obj is State state && Equals(state);
                }
                public override int GetHashCode() {
                    return (int)(Position | (Value << 4));
                }
                public bool Equals(State other) {
                    return Position == other.Position && Value == other.Value;
                }
            }
        }
        private class Ackermann {
            public static int FindValue() {
                for (uint i = 0; i < 32768; i++) {
                    uint value = Calculate(4, 1, i);
                    if (value == 6) {
                        return (int)i;
                    }
                }
                return 0;
            }
            public static uint ModPow(uint n, uint p) {
                uint r = 1;
                while (p != 0) {
                    if ((p & 1) != 0) { r *= n; }
                    n *= n;
                    p >>= 1;
                }
                return r;
            }
            public static uint PowAdd(uint n, uint p) {
                uint r = 0;
                uint k = 1;
                while (p != 0) {
                    r += k;
                    k *= n;
                    p--;
                }
                return r;
            }
            public static uint Calculate(uint m, uint n, uint k) {
                switch (m) {
                    case 0: return (n + 1) & 32767;
                    case 1: return (n + k + 1) & 32767;
                    case 2: return (n * (k + 1) + 2 * k + 1) & 32767;
                    case 3: return (PowAdd(k + 1, n + 3) - 2) & 32767;
                    default:
                        if (n > 0) {
                            return Calculate(m - 1, Calculate(m, n - 1, k), k);
                        }
                        return Calculate(m - 1, k, k);
                }
            }
        }
    }
    public enum OpCode {
        Halt,
        Set,
        Push,
        Pop,
        Equals,
        GreaterThan,
        Jump,
        JumpTrue,
        JumpFalse,
        Add,
        Multiply,
        Mod,
        And,
        Or,
        Not,
        Read,
        Write,
        Call,
        Return,
        Output,
        Input,
        NoOperation
    }
}