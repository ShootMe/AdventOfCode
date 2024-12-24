using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2024 {
    [Description("Crossed Wires")]
    public class Puzzle24 : ASolver {
        private Dictionary<string, ICircuit> circuits = new();
        public override void Setup() {
            string[] lines = Input.Split('\n');
            int i = 0;
            while (i < lines.Length) {
                string line = lines[i++];
                if (string.IsNullOrEmpty(line)) { break; };

                int index = line.IndexOf(':');
                InputWire value = new() { Name = line.Substring(0, index), Value = (byte)line.Substring(index + 2).ToInt() };
                circuits.Add(value.Name, value);
            }

            bool[] completed = new bool[lines.Length - i];
            while (true) {
                bool finished = true;

                for (int j = i; j < lines.Length; j++) {
                    if (completed[j - i]) { continue; };

                    finished = false;
                    string[] operation = lines[j].SplitOn(" ", " ", " -> ");
                    if (!circuits.TryGetValue(operation[0], out ICircuit left)) { continue; }
                    if (!circuits.TryGetValue(operation[2], out ICircuit right)) { continue; }

                    ICircuit circuit = null;
                    switch (operation[1]) {
                        case "XOR": circuit = new Gate() { ID = j - i, Op = Operation.XOR, Name = operation[3], LeftInput = left, RightInput = right }; break;
                        case "OR": circuit = new Gate() { ID = j - i, Op = Operation.OR, Name = operation[3], LeftInput = left, RightInput = right }; break;
                        case "AND": circuit = new Gate() { ID = j - i, Op = Operation.AND, Name = operation[3], LeftInput = left, RightInput = right }; break;
                    }
                    circuits.Add(circuit.Name, circuit);
                    completed[j - i] = true;
                }

                if (finished) { break; }
            }

            Gate.InitializeCache(lines.Length - i);

            foreach (var circuit in circuits.Values) {
                if (circuit is Gate gate) { gate.ArrangeInputs(); }
            }
        }

        [Description("What decimal number does it output on the wires starting with z?")]
        public override string SolvePart1() {
            return $"{GetValue('z')}";
        }

        [Description("What are the names of the eight wires involved in a swap?")]
        public override string SolvePart2() {
            if (circuits.Count < 100) { return string.Empty; }

            //z11 <-> rpv
            //ctg <-> rpb
            //z31 <-> dmh
            //z38 <-> dvq

            List<ICircuit> fix = new();
            Gate gate1 = circuits["z11"] as Gate;
            Gate gate2 = circuits["rpv"] as Gate;
            fix.Add(gate1); fix.Add(gate2);
            Extensions.Swap(ref gate1.LeftInput, ref gate2.LeftInput);
            Extensions.Swap(ref gate1.RightInput, ref gate2.RightInput);
            Extensions.Swap(ref gate1.Op, ref gate2.Op);
            //Console.WriteLine(IsValid(62));

            gate1 = circuits["ctg"] as Gate;
            gate2 = circuits["rpb"] as Gate;
            fix.Add(gate1); fix.Add(gate2);
            Extensions.Swap(ref gate1.LeftInput, ref gate2.LeftInput);
            Extensions.Swap(ref gate1.RightInput, ref gate2.RightInput);
            Extensions.Swap(ref gate1.Op, ref gate2.Op);
            //Console.WriteLine(IsValid(62));

            gate1 = circuits["z31"] as Gate;
            gate2 = circuits["dmh"] as Gate;
            fix.Add(gate1); fix.Add(gate2);
            Extensions.Swap(ref gate1.LeftInput, ref gate2.LeftInput);
            Extensions.Swap(ref gate1.RightInput, ref gate2.RightInput);
            Extensions.Swap(ref gate1.Op, ref gate2.Op);
            //Console.WriteLine(IsValid(62));

            gate1 = circuits["z38"] as Gate;
            gate2 = circuits["dvq"] as Gate;
            fix.Add(gate1); fix.Add(gate2);
            Extensions.Swap(ref gate1.LeftInput, ref gate2.LeftInput);
            Extensions.Swap(ref gate1.RightInput, ref gate2.RightInput);
            Extensions.Swap(ref gate1.Op, ref gate2.Op);
            //Console.WriteLine(IsValid(62));

            //List<ICircuit> all = new(circuits.Values);
            //all.Sort();
            //Doesn't quite work
            //HashSet<ICircuit> fixSet = new();
            //while (fixSet.Count < 8) {
            //    int currentDiff = IsValid(62);

            //    for (int i = 0; i < all.Count; i++) {
            //        Gate gate1 = all[i] as Gate;
            //        if (gate1 == null || fixSet.Contains(gate1)) { continue; }

            //        int j = i + 1;
            //        for (; j < all.Count; j++) {
            //            Gate gate2 = all[j] as Gate;
            //            if (gate2 == null || fixSet.Contains(gate2) || gate1.IsCircular(gate2) || gate2.IsCircular(gate1)) { continue; }

            //            Extensions.Swap(ref gate1.LeftInput, ref gate2.LeftInput);
            //            Extensions.Swap(ref gate1.RightInput, ref gate2.RightInput);
            //            Extensions.Swap(ref gate1.Op, ref gate2.Op);

            //            int diff = IsValid(62);
            //            if (diff > currentDiff + 1) {
            //                currentDiff = diff;
            //                fixSet.Add(gate1); fixSet.Add(gate2);
            //                Console.WriteLine($"{diff} = {gate1.Name} <-> {gate2.Name}");
            //                break;
            //            }

            //            Extensions.Swap(ref gate1.LeftInput, ref gate2.LeftInput);
            //            Extensions.Swap(ref gate1.RightInput, ref gate2.RightInput);
            //            Extensions.Swap(ref gate1.Op, ref gate2.Op);
            //        }
            //        if (j != all.Count) { break; }
            //    }
            //}

            fix.Sort((left, right) => left.Name.CompareTo(right.Name));

            StringBuilder sb = new();
            for (int i = 0; i < fix.Count; i++) {
                sb.Append(fix[i].Name).Append(',');
            }
            sb.Length--;

            return sb.ToString();
        }
        private void Print() {
            List<ICircuit> all = new(circuits.Values);
            all.Sort();
            List<string> outs = new(all.Count);
            for (int j = 0; j < all.Count; j++) {
                ICircuit circuit = all[j];
                string val = all[j].ToString();
                if (val.Length > 3) {
                    outs.Add(circuit.Name + ": " + val);
                }
            }
            outs.Sort((left, right) => {
                int comp = left.Length.CompareTo(right.Length);
                if (comp != 0) { return comp; }
                return left.Substring(4).CompareTo(right.Substring(4));
            });
            for (int j = 0; j < outs.Count; j++) {
                Console.WriteLine(outs[j]);
            }
        }
        private int IsValid(int bits) {
            int count = 0;
            Random rnd = new();
            while (bits-- > 0) {
                long mask = (1L << (count + 1)) - 1;

                for (int i = 0; i < 30; i++) {
                    Gate.ClearCache();
                    long val = rnd.NextInt64() & mask;
                    SetValue('y', val);
                    SetValue('x', val);
                    long z = GetValue('z');

                    if ((z & mask) != ((val + val) & mask)) { return count; }
                }
                count++;
            }
            return count;
        }
        private long GetValue(char v) {
            long value = 0;
            for (int i = 64; i >= 0; i--) {
                string name = $"{v}{i:00}";
                if (circuits.TryGetValue(name, out ICircuit circuit)) {
                    value = (value << 1) | circuit.Evaluate();
                }
            }
            return value;
        }
        private void SetValue(char v, long value) {
            for (int i = 0; i < 64; i++) {
                string name = $"{v}{i:00}";
                if (circuits.TryGetValue(name, out ICircuit circuit) && circuit is InputWire wire) {
                    wire.Value = (byte)(value & 1);
                    value >>= 1;
                }
            }
        }
        private interface ICircuit : IComparable<ICircuit>, IEquatable<ICircuit> {
            string Name { get; }
            byte Evaluate();
        }
        private class InputWire : ICircuit {
            public byte Value;
            public string Name { get; set; }
            public byte Evaluate() => Value;
            public override string ToString() => $"{Name}";
            public override int GetHashCode() => Name.GetHashCode();
            public int CompareTo(ICircuit other) => Name.CompareTo(other.Name);
            public bool Equals(ICircuit other) => other is InputWire && Name == other.Name;
        }
        private enum Operation { AND, OR, XOR }
        private class Gate : ICircuit {
            protected static byte?[] Cache;
            public static void InitializeCache(int size) => Cache = new byte?[size];
            public static void ClearCache() => Array.Fill(Cache, null);

            public ICircuit LeftInput, RightInput;
            public Operation Op;
            public int ID { get; set; }
            public string Name { get; set; }
            public void ArrangeInputs() {
                if (LeftInput.CompareTo(RightInput) > 0) {
                    Extensions.Swap(ref LeftInput, ref RightInput);
                }
            }
            public byte Evaluate() {
                if (!Cache[ID].HasValue) {
                    switch (Op) {
                        case Operation.AND: Cache[ID] = (byte)(LeftInput.Evaluate() & RightInput.Evaluate()); break;
                        case Operation.OR: Cache[ID] = (byte)(LeftInput.Evaluate() | RightInput.Evaluate()); break;
                        case Operation.XOR: Cache[ID] = (byte)(LeftInput.Evaluate() ^ RightInput.Evaluate()); break;
                    }
                }
                return Cache[ID].Value;
            }
            public bool IsCircular(Gate gate) => gate != null && (gate.ID == ID || IsCircular(gate.LeftInput as Gate) || IsCircular(gate.RightInput as Gate));
            public override string ToString() {
                switch (Op) {
                    case Operation.AND: return $"({LeftInput} & {RightInput})";
                    case Operation.OR: return $"({LeftInput} | {RightInput})";
                    case Operation.XOR: return $"({LeftInput} ^ {RightInput})";
                }
                return string.Empty;
            }
            public override int GetHashCode() => ID;
            public int CompareTo(ICircuit other) {
                if (other is Gate gate) {
                    int comp = LeftInput.Name.CompareTo(gate.LeftInput.Name);
                    if (comp != 0) { return comp; }
                    comp = RightInput.Name.CompareTo(gate.RightInput.Name);
                    if (comp != 0) { return comp; }
                }
                return LeftInput.Name.CompareTo(other.Name);
            }
            public bool Equals(ICircuit other) => other is Gate gate && ID == gate.ID;
        }
    }
}