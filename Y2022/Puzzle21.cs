using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Monkey Math")]
    public class Puzzle21 : ASolver {
        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                int index = line.IndexOf(':');
                string name = line[..index];
                string[] operations = line[(index + 2)..].Split(' ');

                if (operations.Length == 1) {
                    Operation.ALL[name] = new ValueOp() { Value = operations[0].ToInt() };
                    continue;
                }

                OpType type = operations[1] switch { "-" => OpType.Subtract, "+" => OpType.Add, "*" => OpType.Multiply, _ => OpType.Divide };
                Operation.ALL[name] = new BinaryOp() { Type = type, Left = operations[0], Right = operations[2] };
            }
        }

        [Description("What number will the monkey named root yell?")]
        public override string SolvePart1() {
            return $"{Operation.ALL["root"].Eval()}";
        }

        [Description("What number do you yell to pass root's equality test?")]
        public override string SolvePart2() {
            BinaryOp root = (BinaryOp)Operation.ALL["root"];
            ValueOp humn = (ValueOp)Operation.ALL["humn"];
            humn.Value = null;

            Expression left = Operation.ALL[root.Left].Eval();
            Expression right = Operation.ALL[root.Right].Eval();
            //Console.WriteLine($"{left} == {right}");

            left.Coef1 -= right.Coef1;
            right.Coef1 = 0;
            right.Coef0 -= left.Coef0;
            left.Coef0 = 0;

            return $"{(long)(right.Coef0 / left.Coef1)}";
        }

        private interface Operation {
            public static Dictionary<string, Operation> ALL = new();
            public Expression Eval();
        }
        private class BinaryOp : Operation {
            public string Left, Right;
            public OpType Type;
            public Expression Eval() {
                return Type switch {
                    OpType.Add => Operation.ALL[Left].Eval() + Operation.ALL[Right].Eval(),
                    OpType.Subtract => Operation.ALL[Left].Eval() - Operation.ALL[Right].Eval(),
                    OpType.Divide => Operation.ALL[Left].Eval() / Operation.ALL[Right].Eval(),
                    OpType.Multiply => Operation.ALL[Left].Eval() * Operation.ALL[Right].Eval(),
                    OpType.Equal => Operation.ALL[Left].Eval() == Operation.ALL[Right].Eval() ? 1 : 0,
                    _ => 0
                };
            }
            public override string ToString() {
                char type = Type == OpType.Add ? '+' : Type == OpType.Subtract ? '-' : Type == OpType.Multiply ? '*' : Type == OpType.Divide ? '/' : '=';
                return $"({Operation.ALL[Left]} {type} {Operation.ALL[Right]})";
            }
        }
        private class ValueOp : Operation {
            public double? Value;
            public Expression Eval() {
                return Value.HasValue ? new Expression() { Coef0 = Value.Value } : new Expression() { Coef1 = 1 };
            }
            public override string ToString() {
                return Value.HasValue ? $"{Value}" : "?";
            }
        }
        private struct Expression {
            public double Coef0, Coef1;
            public static implicit operator Expression(double value) {
                return new Expression() { Coef0 = value };
            }
            public override bool Equals(object obj) {
                return obj is Expression exp && Equals(exp);
            }
            public bool Equals(Expression exp) {
                return exp.Coef0 == Coef0 && exp.Coef1 == Coef1;
            }
            public override int GetHashCode() {
                return (int)(Coef0 * 31 + Coef1);
            }
            public static bool operator ==(Expression left, Expression right) {
                return left.Equals(right);
            }
            public static bool operator !=(Expression left, Expression right) {
                return !left.Equals(right);
            }
            public static Expression operator +(Expression left, Expression right) {
                return new Expression() { Coef0 = left.Coef0 + right.Coef0, Coef1 = left.Coef1 + right.Coef1 };
            }
            public static Expression operator -(Expression left, Expression right) {
                return new Expression() { Coef0 = left.Coef0 - right.Coef0, Coef1 = left.Coef1 - right.Coef1 };
            }
            public static Expression operator *(Expression left, Expression right) {
                return new Expression() { Coef0 = left.Coef0 * right.Coef0, Coef1 = left.Coef1 * right.Coef0 + right.Coef1 * left.Coef0 };
            }
            public static Expression operator /(Expression left, Expression right) {
                return new Expression() { Coef0 = left.Coef0 / right.Coef0, Coef1 = left.Coef1 / right.Coef0 };
            }
            public override string ToString() {
                if (Coef1 != 0 && Coef0 != 0) {
                    return $"{Coef1}? + {Coef0}";
                } else if (Coef1 != 0) {
                    return $"{Coef1}?";
                }
                return $"{Coef0}";
            }
        }
        private enum OpType {
            Add,
            Subtract,
            Multiply,
            Divide,
            Equal,
            None
        }
    }
}