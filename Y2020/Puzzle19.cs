using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2020 {
    [Description("Monster Messages")]
    public class Puzzle19 : ASolver {
        private Dictionary<int, Rule> rules;
        private List<string> messages;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            rules = new Dictionary<int, Rule>();
            messages = new List<string>();

            int i = 0;
            for (; i < items.Count; i++) {
                string item = items[i];
                if (string.IsNullOrEmpty(item)) { break; }

                Rule rule = new Rule(item);
                rules[rule.ID] = rule;
            }

            i++;
            for (; i < items.Count; i++) {
                string item = items[i];
                messages.Add(item);
            }
        }

        [Description("How many messages completely match rule 0?")]
        public override string SolvePart1() {
            Rule zero = rules[0];
            int count = 0;
            for (int i = 0; i < messages.Count; i++) {
                if (zero.Match(rules, messages[i])) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("After updating rules 8 and 11, how many messages completely match rule 0?")]
        public override string SolvePart2() {
            Rule eight = rules[8];
            eight.Rules2.Clear();
            eight.Rules2.AddRange(new int[] { 42, 8 });

            Rule eleven = rules[11];
            eleven.Rules2.Clear();
            eleven.Rules2.AddRange(new int[] { 42, 11, 31 });

            Rule zero = rules[0];
            int count = 0;
            for (int i = 0; i < messages.Count; i++) {
                if (zero.Match(rules, messages[i])) {
                    count++;
                }
            }
            return $"{count}";
        }

        private class Rule {
            public int ID;
            public List<int> Rules1;
            public List<int> Rules2;
            public char Value;

            public Rule(string rules) {
                int index1 = rules.IndexOf(':');
                ID = Tools.ParseInt(rules, 0, index1);

                Rules1 = new List<int>();
                Rules2 = new List<int>();

                int index2 = rules.IndexOf('|');
                if (index2 < 0) { index2 = rules.Length; }

                index1++;
                if (rules[index1 + 1] == '"') {
                    Value = rules[index1 + 2];
                    return;
                }

                while (index1 < index2) {
                    int index3 = rules.IndexOf(' ', index1 + 1);
                    if (index3 < 0) { index3 = rules.Length; }

                    if (index3 <= index2) {
                        Rules1.Add(Tools.ParseInt(rules, index1 + 1, index3 - index1 - 1));
                    }
                    index1 = index3;
                }

                index1 = index2 + 1;
                index2 = rules.Length;
                while (index1 < index2) {
                    int index3 = rules.IndexOf(' ', index1 + 1);
                    if (index3 < 0) { index3 = rules.Length; }

                    if (index3 <= index2) {
                        Rules2.Add(Tools.ParseInt(rules, index1 + 1, index3 - index1 - 1));
                    }
                    index1 = index3;
                }
            }

            public bool Match(Dictionary<int, Rule> rules, string message) {
                foreach (int matched in Match(rules, message, 0)) {
                    if (matched == message.Length) {
                        return true;
                    }
                }
                return false;
            }
            private IEnumerable<int> Match(Dictionary<int, Rule> rules, string message, int index) {
                if (index < message.Length) {
                    if (Rules1.Count > 0) {
                        foreach (int matched in Match(rules, Rules1, 0, message, index)) {
                            yield return matched;
                        }
                    }

                    if (Rules2.Count > 0) {
                        foreach (int matched in Match(rules, Rules2, 0, message, index)) {
                            yield return matched;
                        }
                    }
                }
            }
            private IEnumerable<int> Match(Dictionary<int, Rule> rules, List<int> ruleIDs, int ruleIndex, string message, int index) {
                if (index < message.Length) {
                    int id = ruleIDs[ruleIndex];
                    Rule rule = rules[id];

                    if (rule.Value != '\0') {
                        if (message[index] == rule.Value) {
                            index++;
                            if (ruleIndex + 1 < ruleIDs.Count) {
                                foreach (int matched in Match(rules, ruleIDs, ruleIndex + 1, message, index)) {
                                    yield return matched;
                                }
                            } else {
                                yield return index;
                            }
                        }
                    } else {
                        foreach (int matched in rule.Match(rules, message, index)) {
                            if (matched <= message.Length) {
                                if (ruleIndex + 1 < ruleIDs.Count) {
                                    foreach (int matched2 in Match(rules, ruleIDs, ruleIndex + 1, message, matched)) {
                                        yield return matched2;
                                    }
                                } else {
                                    yield return matched;
                                }
                            }
                        }
                    }
                }
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{ID}:");
                if (Value != '\0') { sb.Append($" {Value}"); }

                for (int i = 0; i < Rules1.Count; i++) {
                    sb.Append($" {Rules1[i]}");
                }

                if (Rules2.Count > 0) {
                    sb.Append(" |");
                    for (int i = 0; i < Rules2.Count; i++) {
                        sb.Append($" {Rules2[i]}");
                    }
                }
                return sb.ToString();
            }
        }
    }
}