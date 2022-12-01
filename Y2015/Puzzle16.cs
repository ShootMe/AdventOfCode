using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2015 {
    [Description("Aunt Sue")]
    public class Puzzle16 : ASolver {
        private List<Aunt> aunts;
        private List<Item> mfcsam;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            aunts = new List<Aunt>();
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index = item.IndexOf(':');
                int id = Tools.ParseInt(item, 4, index - 4);
                string[] things = item.Substring(index + 2).Split(", ");
                Aunt sue = new Aunt() { ID = id };
                for (int j = 0; j < things.Length; j++) {
                    string thing = things[j];
                    index = thing.IndexOf(':');
                    int amount = Tools.ParseInt(thing, index + 2);
                    sue.Items.Add(new Item() { Name = thing.Substring(0, index), Amount = amount });
                }
                aunts.Add(sue);
            }

            Item children = new Item() { Name = "children", Amount = 3 };
            Item cats = new Item() { Name = "cats", Amount = 7 };
            Item samoyeds = new Item() { Name = "samoyeds", Amount = 2 };
            Item pomeranians = new Item() { Name = "pomeranians", Amount = 3 };
            Item akitas = new Item() { Name = "akitas", Amount = 0 };
            Item vizslas = new Item() { Name = "vizslas", Amount = 0 };
            Item goldfish = new Item() { Name = "goldfish", Amount = 5 };
            Item trees = new Item() { Name = "trees", Amount = 3 };
            Item cars = new Item() { Name = "cars", Amount = 2 };
            Item perfumes = new Item() { Name = "perfumes", Amount = 1 };
            mfcsam = new List<Item>() { children, cats, samoyeds, pomeranians, akitas, vizslas, goldfish, trees, cars, perfumes };
        }

        [Description("What is the number of the Sue that got you the gift?")]
        public override string SolvePart1() {
            for (int i = 0; i < aunts.Count; i++) {
                Aunt sue = aunts[i];
                bool isPossible = true;

                for (int j = 0; j < mfcsam.Count; j++) {
                    Item item = mfcsam[j];
                    Item suesItem = sue.Items.Find(it => it.Name == item.Name);
                    if (suesItem != null && item.Amount != suesItem.Amount) {
                        isPossible = false;
                        break;
                    }
                }

                if (isPossible) {
                    return $"{sue.ID}";
                }
            }
            return string.Empty;
        }

        [Description("What is the number of the real Aunt Sue?")]
        public override string SolvePart2() {
            for (int i = 0; i < aunts.Count; i++) {
                Aunt sue = aunts[i];
                bool isPossible = true;

                for (int j = 0; j < mfcsam.Count; j++) {
                    Item item = mfcsam[j];
                    Item suesItem = sue.Items.Find(it => it.Name == item.Name);
                    if (suesItem != null) {
                        if (item.Name == "trees" || item.Name == "cats") {
                            if (suesItem.Amount <= item.Amount) {
                                isPossible = false;
                                break;
                            }
                        } else if (item.Name == "pomeranians" || item.Name == "goldfish") {
                            if (suesItem.Amount >= item.Amount) {
                                isPossible = false;
                                break;
                            }
                        } else if (item.Amount != suesItem.Amount) {
                            isPossible = false;
                            break;
                        }
                    }
                }

                if (isPossible) {
                    return $"{sue.ID}";
                }
            }
            return string.Empty;
        }

        private class Aunt {
            public int ID;
            public List<Item> Items = new List<Item>();
            public override string ToString() {
                StringBuilder sb = new StringBuilder($"Sue {ID}: ");
                for (int i = 0; i < Items.Count; i++) {
                    sb.Append(Items[i]).Append(", ");
                }
                sb.Length -= 2;
                return sb.ToString();
            }
        }
        private class Item : IEquatable<Item> {
            public string Name;
            public int Amount;

            public bool Equals(Item other) {
                return Name == other.Name;
            }
            public override bool Equals(object obj) {
                return obj is Item item && Equals(item);
            }
            public override int GetHashCode() {
                return Name.GetHashCode();
            }
            public override string ToString() {
                return $"{Name}: {Amount}";
            }
        }
    }
}