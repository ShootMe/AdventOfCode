using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Balance Bots")]
    public class Puzzle10 : ASolver {
        private HashSet<Microchip> Chips;
        private HashSet<Holder> Holders;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            Chips = new HashSet<Microchip>();
            Holders = new HashSet<Holder>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                if (item.IndexOf("value", StringComparison.OrdinalIgnoreCase) == 0) {
                    int index = item.IndexOf(' ', 6);
                    int value = Tools.ParseInt(item, 6, index - 6);
                    index = item.IndexOf(" bot ", StringComparison.OrdinalIgnoreCase);
                    int id = Tools.ParseInt(item, index + 5);

                    Microchip chip = GetChip(value);
                    Holder bot = GetHolder(id, HolderType.Bot);
                    bot.Add(chip);
                } else {
                    int index = item.IndexOf(' ', 4);
                    int idBot = Tools.ParseInt(item, 4, index - 4);

                    bool isBotLow = item[index + 14] == 'b';
                    index += isBotLow ? 17 : 20;

                    int indexHigh = item.IndexOf(" and high");
                    int idLow = Tools.ParseInt(item, index, indexHigh - index);

                    bool isBotHigh = item[indexHigh + 13] == 'b';
                    indexHigh += isBotHigh ? 16 : 19;

                    int idHigh = Tools.ParseInt(item, indexHigh);

                    Bot bot = (Bot)GetHolder(idBot, HolderType.Bot);
                    Holder holderLow = GetHolder(idLow, isBotLow ? HolderType.Bot : HolderType.Output);
                    Holder holderHigh = GetHolder(idHigh, isBotHigh ? HolderType.Bot : HolderType.Output);
                    bot.GivesLowTo = holderLow;
                    bot.GivesHighTo = holderHigh;
                }
            }
        }

        [Description("What is the number of the bot that compares 61 with 17?")]
        public override string SolvePart1() {
            bool didSomething;
            do {
                didSomething = false;
                foreach (Holder holder in Holders) {
                    if (holder.Has(17) && holder.Has(61)) {
                        return $"{holder.ID}";
                    }
                    didSomething |= holder.DoAction();
                }
            } while (didSomething);
            return string.Empty;
        }

        [Description("What do you get if you multiply together the values of one chip in outputs 0, 1, and 2?")]
        public override string SolvePart2() {
            bool didSomething;
            do {
                didSomething = false;
                foreach (Holder holder in Holders) {
                    didSomething |= holder.DoAction();
                }
            } while (didSomething);

            FactoryPort out0 = (FactoryPort)GetHolder(0, HolderType.Output);
            FactoryPort out1 = (FactoryPort)GetHolder(1, HolderType.Output);
            FactoryPort out2 = (FactoryPort)GetHolder(2, HolderType.Output);
            return $"{out0.Chips[0].Value * out1.Chips[0].Value * out2.Chips[0].Value}";
        }

        private Microchip GetChip(int value) {
            Microchip chip;
            Microchip temp = new Microchip() { Value = value };
            if (!Chips.TryGetValue(temp, out chip)) {
                Chips.Add(temp);
                return temp;
            }
            return chip;
        }
        private Holder GetHolder(int id, HolderType type) {
            Holder holder;
            Holder temp = type == 0 ? new Bot(id) : new FactoryPort(id, type == HolderType.Output);
            if (!Holders.TryGetValue(temp, out holder)) {
                Holders.Add(temp);
                return temp;
            }
            return holder;
        }

        private enum HolderType {
            Bot,
            Output,
            Input
        }
        private class Bot : Holder {
            public Holder GivesLowTo, GivesHighTo;
            public Microchip Low, High;

            public Bot(int id) : base(id, 0) { }
            public override void Add(Microchip chip) {
                chip.Holder = this;
                if (Low > chip) {
                    High = Low;
                    Low = chip;
                } else {
                    High = chip;
                }
            }
            public override bool Has(int value) {
                if (Low != null && Low.Value == value) {
                    return true;
                } else if (High != null && High.Value == value) {
                    return true;
                }
                return false;
            }
            public override bool DoAction() {
                if (Low == null || High == null) {
                    return false;
                }

                GivesLowTo.Add(Low);
                Low = null;
                GivesHighTo.Add(High);
                High = null;
                return true;
            }
            public override string ToString() {
                return $"{Type} {ID} [{Low?.Value}, {High?.Value}]";
            }
        }
        private class FactoryPort : Holder {
            public List<Microchip> Chips = new List<Microchip>();

            public FactoryPort(int id, bool isOut) : base(id, isOut ? HolderType.Output : HolderType.Input) { }
            public override void Add(Microchip chip) {
                chip.Holder = this;
                Chips.Add(chip);
            }
            public override bool Has(int value) {
                for (int i = 0; i < Chips.Count; i++) {
                    if (Chips[i].Value == value) {
                        return true;
                    }
                }
                return false;
            }
            public override bool DoAction() {
                return false;
            }
            public override string ToString() {
                return $"{Type} {ID} ({Chips.Count} chips)";
            }
        }
        private class Microchip : IEquatable<Microchip> {
            public int Value;
            public Holder Holder;

            public static bool operator >(Microchip left, Microchip right) {
                return left == null || right == null || left.Value > right.Value;
            }
            public static bool operator <(Microchip left, Microchip right) {
                return left == null || right == null || left.Value < right.Value;
            }
            public bool Equals(Microchip other) {
                return other != null && Value == other.Value;
            }
            public override bool Equals(object obj) {
                return obj is Microchip chip && chip.Value == Value;
            }
            public override int GetHashCode() {
                return Value;
            }
            public override string ToString() {
                return $"{Holder.Type} {Holder.ID} has {Value}";
            }
        }
        private abstract class Holder : IEquatable<Holder> {
            public int ID;
            public HolderType Type;

            public Holder(int id, HolderType type) {
                ID = id;
                Type = type;
            }
            public abstract bool Has(int value);
            public abstract void Add(Microchip chip);
            public abstract bool DoAction();

            public bool Equals(Holder other) {
                return other != null && ID == other.ID && Type == other.Type;
            }
            public override bool Equals(object obj) {
                return obj is Holder holder && holder.ID == ID && holder.Type == Type;
            }
            public override int GetHashCode() {
                return ID | ((int)Type << 16);
            }
        }
    }
}