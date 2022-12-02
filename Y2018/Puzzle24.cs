using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Immune System Simulator 20XX")]
    public class Puzzle24 : ASolver {
        private List<Group> ImmuneSystem, Infection, All;

        public override void Setup() {
            List<string> items = Input.Lines();
            All = new List<Group>();
            ImmuneSystem = new List<Group>();
            int i = 1;
            int id = 1;
            while (i < items.Count) {
                string item = items[i++];
                if (string.IsNullOrEmpty(item)) { break; }
                Group group = new Group(item) { IsImmuneSystem = true, ID = id };
                ImmuneSystem.Add(group);
                All.Add(group);
                id++;
            }

            Infection = new List<Group>();
            i++;
            id = 1;
            while (i < items.Count) {
                string item = items[i++];
                if (string.IsNullOrEmpty(item)) { break; }
                Group group = new Group(item) { ID = id };
                Infection.Add(group);
                All.Add(group);
                id++;
            }
        }

        [Description("How many units would the winning army have?")]
        public override string SolvePart1() {
            return $"{Math.Abs(RunSimulation())}";
        }
        private int RunSimulation() {
            bool killedUnits = true;
            while (ImmuneSystem.Count > 0 && Infection.Count > 0 && killedUnits) {
                killedUnits = false;
                SelectDefender(ImmuneSystem, Infection);
                SelectDefender(Infection, ImmuneSystem);

                All.Sort(delegate (Group left, Group right) {
                    return left.Initiative.CompareTo(right.Initiative);
                });

                for (int i = All.Count - 1; i >= 0; i--) {
                    Group attacker = All[i];
                    if (attacker.Units == 0) {
                        All.RemoveAt(i);
                        continue;
                    }

                    Group defender = attacker.Attacking;
                    if (defender == null) { continue; }

                    bool dealtDamage = attacker.DealDamage(defender);
                    killedUnits |= dealtDamage;
                    if (defender.Units == 0) {
                        if (defender.IsImmuneSystem) {
                            ImmuneSystem.Remove(defender);
                        } else {
                            Infection.Remove(defender);
                        }
                    }
                }
            }

            int total = 0;
            for (int i = All.Count - 1; i >= 0; i--) {
                total += All[i].Units;
            }
            return Infection.Count > 0 ? -total : total;
        }
        private void SelectDefender(List<Group> attackers, List<Group> defenders) {
            attackers.Sort();
            for (int i = 0; i < defenders.Count; i++) {
                defenders[i].Taken = false;
            }

            for (int i = 0; i < attackers.Count; i++) {
                Group attacker = attackers[i];
                attacker.Attacking = null;
                if (attacker.Units == 0) { continue; }
                int maxDamage = -1;

                for (int j = 0; j < defenders.Count; j++) {
                    Group defender = defenders[j];
                    if (defender.Taken || defender.Units == 0) { continue; }

                    int damage = attacker.DamagePossible(defender);
                    if (damage > 0 && (damage > maxDamage || (damage == maxDamage && attacker.Attacking.IsGreaterThreat(defender)))) {
                        maxDamage = damage;
                        if (attacker.Attacking != null) {
                            attacker.Attacking.Taken = false;
                        }
                        attacker.Attacking = defender;
                        defender.Taken = true;
                    }
                }
            }
        }

        [Description("How many units does the immune system have after the smallest boost needed to win?")]
        public override string SolvePart2() {
            int total = -1;
            int boost = 1;
            while (total < 0) {
                Setup();
                AddBoost(boost);
                total = RunSimulation();
                boost++;
            }
            return $"{total}";
        }
        private void AddBoost(int boost) {
            for (int i = 0; i < ImmuneSystem.Count; i++) {
                ImmuneSystem[i].Damage += boost;
            }
        }

        private class Group : IComparable<Group>, IEquatable<Group> {
            public int ID;
            public int Units;
            public int Health;
            public int Damage;
            public int Initiative;
            public string DamageType;
            public bool IsImmuneSystem;
            public HashSet<string> Weaknesses = new HashSet<string>();
            public HashSet<string> Immunities = new HashSet<string>();
            public Group Attacking;
            public bool Taken;
            public int EffectivePower { get { return Units * Damage; } }

            public Group(string input) {
                int index = input.IndexOf(" units ");
                Units = Tools.ParseInt(input, 0, index);

                index = input.IndexOf(" with ", index);
                int endIndex = input.IndexOf(" hit ", index);
                Health = Tools.ParseInt(input, index + 6, endIndex - index - 6);

                index = input.IndexOf('(', endIndex);
                if (index > 0) {
                    endIndex = input.IndexOf(' ', index);
                    string type = input.Substring(index + 1, endIndex - index - 1);
                    endIndex += 4;
                    EnumerateTypes(input, type, ref endIndex);
                }

                index = input.IndexOf(" does ", endIndex);
                endIndex = input.IndexOf(' ', index + 6);
                Damage = Tools.ParseInt(input, index + 6, endIndex - index - 6);

                index = input.IndexOf(' ', endIndex + 1);
                DamageType = input.Substring(endIndex + 1, index - endIndex - 1);

                index = input.IndexOf("initiative", index);
                Initiative = Tools.ParseInt(input, index + 11);
            }
            private void EnumerateTypes(string input, string type, ref int index) {
                int endIndex = input.IndexOfAny(new char[] { ',', ';', ')' }, index);
                char c = input[endIndex];
                string dmgType = input.Substring(index, endIndex - index);
                switch (type) {
                    case "weak": Weaknesses.Add(dmgType); break;
                    case "immune": Immunities.Add(dmgType); break;
                }

                index = endIndex + 2;
                if (c == ',') {
                    EnumerateTypes(input, type, ref index);
                } else if (c == ';') {
                    endIndex = input.IndexOf(' ', index);
                    type = input.Substring(index, endIndex - index);
                    index = endIndex + 4;
                    EnumerateTypes(input, type, ref index);
                }
            }
            public bool DealDamage(Group other) {
                int damage = DamagePossible(other);
                int unitsKilled = damage / other.Health;
                other.Units -= unitsKilled;
                if (other.Units < 0) { other.Units = 0; }
                return unitsKilled > 0;
            }
            public bool IsGreaterThreat(Group other) {
                if (other.EffectivePower == EffectivePower) {
                    return other.Initiative > Initiative;
                }
                return other.EffectivePower > EffectivePower;
            }
            public int DamagePossible(Group other) {
                if (other.Immunities.Contains(DamageType)) {
                    return 0;
                } else if (other.Weaknesses.Contains(DamageType)) {
                    return EffectivePower * 2;
                }
                return EffectivePower;
            }
            public int CompareTo(Group other) {
                int compare = other.EffectivePower.CompareTo(EffectivePower);
                if (compare != 0) { return compare; }
                return other.Initiative.CompareTo(Initiative);
            }
            public bool Equals(Group other) {
                return ID == other.ID;
            }
            public override bool Equals(object obj) {
                return obj is Group other && Equals(other);
            }
            public override int GetHashCode() {
                return ID;
            }
            public override string ToString() {
                return $"ID: {ID} Units: {Units} Health: {Health} Damage: {Damage} Type: {DamageType} Initiative: {Initiative} Power: {EffectivePower} System: {IsImmuneSystem}";
            }
        }
    }
}