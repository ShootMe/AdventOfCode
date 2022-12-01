using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2020 {
    [Description("Operation Order")]
    public class Puzzle18 : ASolver {
        private List<Operand> expressions;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            expressions = new List<Operand>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index = -1;
                expressions.Add(new Operator(item, ref index));
            }
        }

        [Description("What is the sum of the resulting values?")]
        public override string SolvePart1() {
            long sum = 0;
            for (int i = 0; i < expressions.Count; i++) {
                sum += expressions[i].Evaluate();
            }
            return $"{sum}";
        }

        [Description("What do you get if you add up the resulting values using these new rules?")]
        public override string SolvePart2() {
            long sum = 0;
            for (int i = 0; i < expressions.Count; i++) {
                sum += expressions[i].Evaluate(true);
            }
            return $"{sum}";
        }

        private class Operator : Operand {
            public List<Operand> Operands;
            public List<bool> Operators;

            public Operator() { }
            public Operator(string exp, ref int index) {
                Operands = new List<Operand>();
                Operators = new List<bool>();

                for (int j = index + 1; j < exp.Length; j++) {
                    char c = exp[j];
                    if (char.IsWhiteSpace(c)) { continue; }

                    if (c == '(') {
                        Operands.Add(new Operator(exp, ref j));
                    } else if (c == ')') {
                        index = j;
                        return;
                    } else if (char.IsDigit(c)) {
                        Operands.Add(new ValueOperand(exp, ref j));
                    } else if (c == '+' || c == '*') {
                        Operators.Add(c == '+');
                    }
                }

                index = exp.Length;
            }
            public override long Evaluate(bool additionFirst = false) {
                if (!additionFirst) {
                    long value = Operands.Count >= 1 ? Operands[0].Evaluate() : 0;

                    for (int i = 1; i < Operands.Count; i++) {
                        bool isAddition = Operators[i - 1];
                        if (isAddition) {
                            value += Operands[i].Evaluate();
                        } else {
                            value *= Operands[i].Evaluate();
                        }
                    }

                    return value;
                } else {
                    int index = 0;
                    Operator newOp = new Operator(ToString(true), ref index);
                    return newOp.Evaluate();
                }
            }
            public override string ToString() {
                return ToString(false);
            }
            public override string ToString(bool additionFirst) {
                string first = Operands.Count >= 1 ? Operands[0].ToString(additionFirst) : string.Empty;

                StringBuilder exp = new StringBuilder();
                if (additionFirst) {
                    AppendParens(exp, 0);
                }
                exp.Append($"({first}");

                for (int i = 1; i < Operands.Count; i++) {
                    bool isAddition = Operators[i - 1];
                    if (isAddition) {
                        exp.Append($" + {Operands[i].ToString(additionFirst)}");
                        if (additionFirst) { exp.Append(')'); }
                    } else {
                        exp.Append(" * ");
                        if (additionFirst) { AppendParens(exp, i); }
                        exp.Append($"{Operands[i].ToString(additionFirst)}");
                    }
                }
                exp.Append(')');
                return exp.ToString();
            }
            private void AppendParens(StringBuilder exp, int index) {
                for (int i = index; i < Operators.Count; i++) {
                    if (Operators[i]) {
                        exp.Append('(');
                    } else {
                        break;
                    }
                }
            }
        }
        private class ValueOperand : Operand {
            public long Value;
            public ValueOperand(string exp, ref int index) {
                for (int j = index; j < exp.Length; j++) {
                    char c = exp[j];

                    if (c == '+' || c == '*' || c == ')' || char.IsWhiteSpace(c)) {
                        Value = Tools.ParseLong(exp, index, j - index);
                        index = char.IsWhiteSpace(c) ? j : j - 1;
                        return;
                    }
                }
                Value = Tools.ParseLong(exp, index, exp.Length - index);
                index = exp.Length;
            }
            public override long Evaluate(bool additionFirst = false) { return Value; }
            public override string ToString() { return $"{Value}"; }
            public override string ToString(bool additionFirst) { return $"{Value}"; }
        }
        private class Operand {
            public virtual long Evaluate(bool additionFirst = false) { return 0; }
            public override string ToString() { return string.Empty; }
            public virtual string ToString(bool additionFirst) { return string.Empty; }
        }
    }
}