using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Handy Haversacks")]
    public class Puzzle07 : ASolver {
        private HashSet<Bag> bags;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);

            bags = new HashSet<Bag>();
            Bag temp;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int index1 = item.IndexOf(" bags contain ");
                Bag bag = new Bag(item.Substring(0, index1));

                if (bags.TryGetValue(bag, out temp)) {
                    bag = temp;
                } else {
                    bags.Add(bag);
                }

                index1 += 12;
                while (index1 > 0) {
                    int index2 = item.IndexOf(' ', index1 + 2);
                    int index3 = item.IndexOf(" bag", index2);
                    Bag sub = new Bag(item.Substring(index2 + 1, index3 - index2 - 1));
                    if (!sub.Type.Equals("other", StringComparison.OrdinalIgnoreCase)) {
                        if (bags.TryGetValue(sub, out temp)) {
                            sub = temp;
                        } else {
                            bags.Add(sub);
                        }
                        int amount = Tools.ParseInt(item, index1 + 2, index2 - index1 - 2);
                        bag.CanHold.Add(sub, amount);
                    }

                    index1 = item.IndexOf(", ", index2);
                }
            }
        }

        [Description("How many bag colors can eventually contain at least one shiny gold bag?")]
        public override string SolvePart1() {
            Bag gold;
            bags.TryGetValue(new Bag("shiny gold"), out gold);

            int count = 0;
            foreach (Bag bag in bags) {
                if (bag.CanHoldBag(gold)) {
                    count++;
                }
            }

            return $"{count}";
        }

        [Description("How many individual bags are required inside your single shiny gold bag?")]
        public override string SolvePart2() {
            Bag gold;
            bags.TryGetValue(new Bag("shiny gold"), out gold);

            return $"{gold.CountInsideBags()}";
        }

        private class Bag : IEquatable<Bag> {
            public string Type;
            public Dictionary<Bag, int> CanHold = new Dictionary<Bag, int>();

            public Bag(string type) {
                Type = type;
            }
            public int CountInsideBags() {
                int count = 0;
                foreach (KeyValuePair<Bag, int> pair in CanHold) {
                    count += pair.Value + pair.Value * pair.Key.CountInsideBags();
                }
                return count;
            }
            public bool CanHoldBag(Bag bag) {
                foreach (Bag sub in CanHold.Keys) {
                    if (sub == bag || sub.CanHoldBag(bag)) {
                        return true;
                    }
                }
                return false;
            }
            public override bool Equals(object obj) {
                return obj is Bag bag && bag.Type.Equals(Type, StringComparison.OrdinalIgnoreCase);
            }
            public override int GetHashCode() {
                return Type.GetHashCode();
            }
            public bool Equals(Bag other) {
                return Type.Equals(other.Type, StringComparison.OrdinalIgnoreCase);
            }
            public override string ToString() {
                return $"{Type} {CanHold.Count}";
            }
        }
    }
}