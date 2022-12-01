using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("I Heard You Like Registers")]
    public class Puzzle08 : ASolver {
        private HashSet<Register> registers;
        private int maxValueDuring;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);

            registers = new HashSet<Register>();
            maxValueDuring = int.MinValue;
            Register temp;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int index1 = item.IndexOf(" inc ");
                string action = "inc";
                if (index1 < 0) {
                    index1 = item.IndexOf(" dec ");
                    action = "dec";
                }
                int index2 = item.IndexOf(" if ", index1);
                int index3 = item.IndexOf(' ', index2 + 4);
                int index4 = item.IndexOf(' ', index3 + 1);

                Register reg1 = item.Substring(0, index1);
                if (registers.TryGetValue(reg1, out temp)) {
                    reg1 = temp;
                } else {
                    registers.Add(reg1);
                }
                Register reg2 = item.Substring(index1 + 5, index2 - index1 - 5);
                if (!reg2.IsValueType) {
                    if (registers.TryGetValue(reg2, out temp)) {
                        reg2 = temp;
                    } else {
                        registers.Add(reg2);
                    }
                }
                Register reg3 = item.Substring(index2 + 4, index3 - index2 - 4);
                if (!reg3.IsValueType) {
                    if (registers.TryGetValue(reg3, out temp)) {
                        reg3 = temp;
                    } else {
                        registers.Add(reg3);
                    }
                }
                string type = item.Substring(index3 + 1, index4 - index3 - 1);
                Register reg4 = item.Substring(index4 + 1);
                if (!reg4.IsValueType) {
                    if (registers.TryGetValue(reg4, out temp)) {
                        reg4 = temp;
                    } else {
                        registers.Add(reg4);
                    }
                }

                if (reg3.Compare(reg4, type)) {
                    reg1.DoAction(action, reg2);
                }
                if (reg1.Value > maxValueDuring) {
                    maxValueDuring = reg1.Value;
                }
            }
        }

        [Description("What is the largest value in any register after completing the instructions?")]
        public override string SolvePart1() {
            int maxVal = int.MinValue;
            foreach (Register register in registers) {
                if (register.Value > maxVal) {
                    maxVal = register.Value;
                }
            }
            return $"{maxVal}";
        }

        [Description("What is the highest value held in any register during this process?")]
        public override string SolvePart2() {
            return $"{maxValueDuring}";
        }

        private class Register : IEquatable<Register> {
            public string Name;
            public int Value;

            public bool IsValueType { get { return Name == "Value"; } }
            public bool Compare(Register other, string compareType) {
                switch (compareType) {
                    case ">": return Value > other.Value;
                    case ">=": return Value >= other.Value;
                    case "<": return Value < other.Value;
                    case "<=": return Value <= other.Value;
                    case "==": return Value == other.Value;
                    case "!=": return Value != other.Value;
                }
                return false;
            }
            public void DoAction(string action, Register value) {
                switch (action) {
                    case "inc": Value += value.Value; break;
                    case "dec": Value -= value.Value; break;
                }
            }

            public static implicit operator Register(string value) {
                int val;
                if (int.TryParse(value, out val)) {
                    return new Register() { Name = "Value", Value = val };
                }
                return new Register() { Name = value };
            }
            public override string ToString() {
                return $"{Name} = {Value}";
            }
            public bool Equals(Register other) {
                return other.Name == Name;
            }
            public override bool Equals(object obj) {
                return obj is Register other && other.Name == Name;
            }
            public override int GetHashCode() {
                return Name.GetHashCode();
            }
        }
    }
}