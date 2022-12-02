using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Ticket Translation")]
    public class Puzzle16 : ASolver {
        private List<Rule> rules;
        private List<Ticket> nearbyTickets;
        private Ticket yourTicket;

        public override void Setup() {
            List<string> lines = Input.Lines();

            rules = new List<Rule>();
            int i = 0;
            while (true) {
                string line = lines[i++];
                if (string.IsNullOrEmpty(line)) {
                    break;
                }

                int index1 = line.IndexOf(':');
                Rule rule = new Rule() { ID = rules.Count };
                rule.Name = line.Substring(0, index1);

                int index2 = line.IndexOf('-', index1);
                rule.Range1Start = Tools.ParseInt(line, index1 + 2, index2 - index1 - 2);
                index1 = line.IndexOf(' ', index2);
                rule.Range1End = Tools.ParseInt(line, index2 + 1, index1 - index2 - 1);
                index2 = line.IndexOf(' ', index1 + 1);
                index1 = line.IndexOf('-', index2);
                rule.Range2Start = Tools.ParseInt(line, index2 + 1, index1 - index2 - 1);
                rule.Range2End = Tools.ParseInt(line, index1 + 1);

                rules.Add(rule);
            }

            i++;
            yourTicket = GetTicket(lines[i++]);

            i += 2;
            nearbyTickets = new List<Ticket>();
            while (i < lines.Count) {
                string line = lines[i++];
                nearbyTickets.Add(GetTicket(line));
            }
        }

        [Description("What is your ticket scanning error rate?")]
        public override string SolvePart1() {
            int errorRate = 0;
            for (int i = nearbyTickets.Count - 1; i >= 0; i--) {
                Ticket ticket = nearbyTickets[i];
                for (int j = 0; j < rules.Count; j++) {
                    rules[j].Verify(ticket);
                }

                for (int j = 0; j < ticket.Values.Length; j++) {
                    if (!ticket.Verified[j]) {
                        errorRate += ticket.Values[j];
                        ticket.Valid = false;
                    }
                }

                if (!ticket.Valid) {
                    nearbyTickets.RemoveAt(i);
                }
            }

            return $"{errorRate}";
        }

        [Description("What do you get if you multiply those six values together?")]
        public override string SolvePart2() {
            HashSet<int>[] invalids = new HashSet<int>[rules.Count];
            for (int i = 0; i < rules.Count; i++) {
                HashSet<int> invalid = new HashSet<int>();
                invalids[i] = invalid;
                for (int j = 0; j < rules.Count; j++) {
                    invalid.Add(j);
                }
            }

            for (int i = 0; i < nearbyTickets.Count; i++) {
                Ticket ticket = nearbyTickets[i];

                for (int j = 0; j < rules.Count; j++) {
                    List<int> rulesInvalid = ticket.RulesInvalid[j];
                    for (int k = 0; k < rulesInvalid.Count; k++) {
                        invalids[j].Remove(rulesInvalid[k]);
                    }
                }
            }

            int[] actualIDS = new int[rules.Count];
            for (int i = 0; i < rules.Count; i++) {
                int value = 0;
                for (int k = 0; k < rules.Count; k++) {
                    HashSet<int> invalid = invalids[k];
                    if (invalid.Count == 1) {
                        foreach (int val in invalid) {
                            value = val;
                            break;
                        }
                        actualIDS[k] = value;
                        break;
                    }
                }

                for (int k = 0; k < rules.Count; k++) {
                    HashSet<int> invalid = invalids[k];
                    invalid.Remove(value);
                }
            }

            long result = 1;
            for (int i = 0; i < rules.Count; i++) {
                Rule rule = rules[actualIDS[i]];
                if (rule.Name.StartsWith("departure")) {
                    result *= yourTicket.Values[i];
                }
            }
            return $"{result}";
        }

        private Ticket GetTicket(string ticket) {
            Ticket result = new Ticket();
            string[] values = ticket.Split(',');
            int[] nums = new int[values.Length];
            for (int j = 0; j < values.Length; j++) {
                string value = values[j];
                nums[j] = Tools.ParseInt(value);
            }
            result.Values = nums;
            result.Verified = new bool[nums.Length];
            result.RulesInvalid = new List<int>[nums.Length];
            for (int j = 0; j < nums.Length; j++) {
                result.RulesInvalid[j] = new List<int>();
            }
            result.Valid = true;
            return result;
        }
        private class Rule {
            public int ID;
            public string Name;
            public int Range1Start, Range1End;
            public int Range2Start, Range2End;

            public void Verify(Ticket ticket) {
                for (int i = 0; i < ticket.Values.Length; i++) {
                    int value = ticket.Values[i];
                    if ((value >= Range1Start && value <= Range1End) || (value >= Range2Start && value <= Range2End)) {
                        ticket.Verified[i] = true;
                    } else {
                        ticket.RulesInvalid[i].Add(ID);
                    }
                }
            }
            public bool Verify(int value) {
                return (value >= Range1Start && value <= Range1End) || (value >= Range2Start && value <= Range2End);
            }
            public override string ToString() {
                return $"{ID} {Name}";
            }
        }
        private class Ticket {
            public int[] Values;
            public bool[] Verified;
            public List<int>[] RulesInvalid;
            public bool Valid;
        }
    }
}