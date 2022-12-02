using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Some Assembly Required")]
    public class Puzzle07 : ASolver {
        public override void Setup() {
            foreach (string item in Input.Split('\n')) {
                AddCircuit(item);
            }
        }

        [Description("What signal is ultimately provided to wire a?")]
        public override string SolvePart1() {
            VarOp var = VarOp.GetVar("A");
            return $"{var.Evaluate()}";
        }

        [Description("What new signal is ultimately provided to wire a?")]
        public override string SolvePart2() {
            VarOp.ClearCalculations();
            VarOp var = VarOp.GetVar("A");
            ValueOp circuitB = (ValueOp)VarOp.GetExistingCircuit("B");
            circuitB.Value = var.Evaluate();
            VarOp.ClearCalculations();

            return $"{var.Evaluate()}";
        }

        private void AddCircuit(string item) {
            int index1 = item.IndexOf(' ');
            string item1 = item.Substring(0, index1).ToUpper();
            int outIndex = item.IndexOf(" -> ");
            string output = item.Substring(outIndex + 4).ToUpper();
            int index2 = item.IndexOf(' ', index1 + 1);
            string item2 = item.Substring(index1 + 1, index2 - index1 - 1).ToUpper();

            ushort value1;
            if (item2 == "->" && ushort.TryParse(item1, out value1)) {
                ValueOp value = new ValueOp() { Value = value1 };
                VarOp.AddCircuit(output, value);
            } else {
                if (item1 == "NOT") {
                    UnaryOp unary = new UnaryOp() { Operator = item1, Input = VarOp.GetVar(item2) };
                    VarOp.AddCircuit(output, unary);
                } else if (item2 == "->") {
                    VarOp.AddCircuit(output, VarOp.GetVar(item1));
                } else {
                    int index3 = item.IndexOf(' ', index2 + 1);
                    string item3 = item.Substring(index2 + 1, index3 - index2 - 1).ToUpper();

                    ICircuit left;
                    if (ushort.TryParse(item1, out value1)) {
                        left = new ValueOp() { Value = value1 };
                    } else {
                        left = VarOp.GetVar(item1);
                    }

                    ICircuit right;
                    if (ushort.TryParse(item3, out value1)) {
                        right = new ValueOp() { Value = value1 };
                    } else {
                        right = VarOp.GetVar(item3);
                    }
                    BinaryOp binary = new BinaryOp() { Operator = item2, Left = left, Right = right };
                    VarOp.AddCircuit(output, binary);
                }
            }
        }

        private interface ICircuit {
            ushort Evaluate();
        }
        private class BinaryOp : ICircuit {
            public ICircuit Left, Right;
            public string Operator;

            public ushort Evaluate() {
                ushort leftValue = EvaluateSide(Left);
                ushort rightValue = EvaluateSide(Right);

                switch (Operator) {
                    case "AND": return (ushort)(leftValue & rightValue);
                    case "OR": return (ushort)(leftValue | rightValue);
                    case "LSHIFT": return (ushort)(leftValue << rightValue);
                    case "RSHIFT": return (ushort)(leftValue >> rightValue);
                }
                return 0;
            }
            private ushort EvaluateSide(ICircuit side) {
                ushort? value = VarOp.GetValue(side);
                if (!value.HasValue) {
                    if (side == null) {
                        value = 0;
                    } else {
                        value = side.Evaluate();
                    }
                    VarOp.AddValue(side, value.Value);
                }
                return value.Value;
            }
            public override string ToString() {
                ushort leftValue = EvaluateSide(Left);
                ushort rightValue = EvaluateSide(Right);

                string left;
                if (Left is VarOp varL) {
                    left = $"({varL.Name} = {leftValue})";
                } else {
                    left = $"{Left}";
                }

                string right;
                if (Right is VarOp varR) {
                    right = $"({varR.Name} = {rightValue})";
                } else {
                    right = $"{Right}";
                }

                switch (Operator) {
                    case "AND": return $"{left} AND {right} = {(ushort)(leftValue & rightValue)}";
                    case "OR": return $"{left} OR {right} = {(ushort)(leftValue | rightValue)}";
                    case "LSHIFT": return $"{left} << {right} = {(ushort)(leftValue << rightValue)}";
                    case "RSHIFT": return $"{left} >> {right} = {(ushort)(leftValue >> rightValue)}";
                }
                return "()";
            }
        }
        private class UnaryOp : ICircuit {
            public ICircuit Input;
            public string Operator;

            public ushort Evaluate() {
                ushort? value = VarOp.GetValue(Input);
                if (!value.HasValue) {
                    if (Input == null) {
                        value = 0;
                    } else {
                        switch (Operator) {
                            case "NOT": value = (ushort)~Input.Evaluate(); break;
                            default: value = 0; break;
                        }
                    }
                    VarOp.AddValue(Input, value.Value);
                }
                return value.Value;
            }
            public override string ToString() {
                if (Input is VarOp var) {
                    return $"(!{var.Name} = {Evaluate()})";
                }
                return $"!{Input}";
            }
        }
        private class ValueOp : ICircuit {
            public ushort Value;
            public ushort Evaluate() {
                return Value;
            }
            public override string ToString() {
                return $"{Value}";
            }
        }
        private class VarOp : ICircuit {
            public const int MaxLevels = 60;
            private static Dictionary<string, ICircuit> Circuits = new Dictionary<string, ICircuit>(StringComparer.OrdinalIgnoreCase);
            private static Dictionary<string, VarOp> Variables = new Dictionary<string, VarOp>(StringComparer.OrdinalIgnoreCase);
            private static Dictionary<ICircuit, ushort> Calculated = new Dictionary<ICircuit, ushort>();
            public string Name;
            public ICircuit Circuit { get { return GetExistingCircuit(Name); } }
            public ushort Evaluate() {
                ICircuit circuit = GetExistingCircuit(Name);
                ushort? value = GetValue(circuit);
                if (!value.HasValue) {
                    value = circuit == null ? (ushort)0 : circuit.Evaluate();
                    AddValue(circuit, value.Value);
                }
                return value.Value;
            }
            public static void AddValue(ICircuit circuit, ushort value) {
                if (!Calculated.ContainsKey(circuit)) {
                    Calculated.Add(circuit, value);
                }
            }
            public static ushort? GetValue(ICircuit circuit) {
                ushort value;
                if (circuit == null || !Calculated.TryGetValue(circuit, out value)) {
                    return null;
                }
                return value;
            }
            public static void AddCircuit(string name, ICircuit circuit) {
                Circuits.Add(name, circuit);
            }
            public static void ClearVars() {
                Circuits.Clear();
                Variables.Clear();
                Calculated.Clear();
            }
            public static void ClearCalculations() {
                Calculated.Clear();
            }
            public static VarOp GetVar(string name) {
                VarOp var;
                if (!Variables.TryGetValue(name, out var)) {
                    var = new VarOp() { Name = name };
                    Variables.Add(name, var);
                }
                return var;
            }
            public static ICircuit GetExistingCircuit(string name) {
                ICircuit circuit;
                if (!Circuits.TryGetValue(name, out circuit)) {
                    return null;
                }
                return circuit;
            }
            public override string ToString() {
                ICircuit circuit = GetExistingCircuit(Name);
                return $"({Name} = {circuit})";
            }
        }
    }
}