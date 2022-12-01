using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Space Stoichiometry")]
    public class Puzzle14 : ASolver {
        private Dictionary<string, Reaction> reactions;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            reactions = new Dictionary<string, Reaction>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                string[] components = item.Split(", ");
                string last = components[^1];

                int index1 = last.IndexOf(" => ");
                int index2 = last.IndexOf(' ', index1 + 4);
                string name = last.Substring(index2 + 1);
                int amount = Tools.ParseInt(last, index1 + 4, index2 - index1 - 4);
                Reaction reaction = new Reaction() { Name = name, Amount = amount };
                components[^1] = last.Substring(0, index1);

                for (int j = 0; j < components.Length; j++) {
                    string component = components[j];
                    index1 = component.IndexOf(' ');
                    amount = Tools.ParseInt(component, 0, index1);
                    name = component.Substring(index1 + 1);
                    reaction.Components.Add(name, amount);
                }

                reactions.Add(reaction.Name, reaction);
            }
        }

        [Description("What is the minimum amount of ORE required to produce exactly 1 FUEL?")]
        public override string SolvePart1() {
            Dictionary<string, long> available = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase) { { "ORE", long.MaxValue } };
            ConsumeChemical(available, "FUEL", 1);
            return $"{long.MaxValue - available["ORE"]}";
        }

        [Description("Given 1 trillion ORE, what is the maximum amount of FUEL you can produce?")]
        public override string SolvePart2() {
            Dictionary<string, long> available = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase) { { "ORE", 1000000000000 } };
            ProduceChemical(available, "FUEL", 1);

            long usedForFuel = 1000000000000 - available["ORE"];
            long estimate = available["ORE"] / usedForFuel;
            while (ProduceChemical(available, "FUEL", estimate)) {
                estimate = available["ORE"] / usedForFuel;
                estimate = estimate < 1 ? 1 : estimate;
            }

            return $"{available["FUEL"]}";
        }

        private bool ConsumeChemical(Dictionary<string, long> available, string chemical, long amount) {
            long chemicalAvailable;
            if (!available.TryGetValue(chemical, out chemicalAvailable)) {
                available[chemical] = 0;
            }

            if (chemicalAvailable < amount && !ProduceChemical(available, chemical, amount - chemicalAvailable)) {
                return false;
            }

            available[chemical] -= amount;
            return true;
        }
        private bool ProduceChemical(Dictionary<string, long> available, string chemical, long quanity) {
            if (chemical == "ORE") {
                return false;
            }

            Reaction reaction = reactions[chemical];
            long multiplier = (quanity + reaction.Amount - 1) / reaction.Amount;

            foreach (KeyValuePair<string, int> component in reaction.Components) {
                if (!ConsumeChemical(available, component.Key, component.Value * multiplier)) {
                    return false;
                }
            }

            long chemicalAvailable;
            if (!available.TryGetValue(chemical, out chemicalAvailable)) {
                chemicalAvailable = 0;
            }
            available[chemical] = chemicalAvailable + reaction.Amount * multiplier;
            return true;
        }

        private class Reaction : IEquatable<Reaction> {
            public string Name;
            public int Amount;
            public Dictionary<string, int> Components = new Dictionary<string, int>();

            public bool Equals(Reaction other) {
                return other.Name == Name;
            }
            public override bool Equals(object obj) {
                return obj is Reaction reaction && Equals(reaction);
            }
            public override int GetHashCode() {
                return Name.GetHashCode();
            }
            public override string ToString() {
                return Name;
            }
        }
    }
}