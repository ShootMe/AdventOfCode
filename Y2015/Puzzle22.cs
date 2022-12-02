using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Wizard Simulator 20XX")]
    public class Puzzle22 : ASolver {
        [Description("What is the least amount of mana you can spend and still win the fight?")]
        public override string SolvePart1() {
            List<string> lines = Input.Lines();
            int hp = lines[0].ToInt();
            int dmg = lines[1].ToInt();

            return $"{PlayGame(50, 500, hp, dmg, false)}";
        }

        [Description("What is the least amount of mana you can spend and still win the fight?")]
        public override string SolvePart2() {
            List<string> lines = Input.Lines();
            int hp = lines[0].ToInt();
            int dmg = lines[1].ToInt();

            return $"{PlayGame(50, 500, hp, dmg, true)}";
        }

        private int PlayGame(int playerHP, int playerMP, int bossHP, int bossDmg, bool hardMode) {
            HashSet<State> closed = new HashSet<State>();
            Heap<State> open = new Heap<State>();
            State current = new State() { PlayerHP = (short)playerHP, PlayerMana = (short)playerMP, BossHP = (byte)bossHP, BossDmg = (byte)bossDmg, Steps = 0 };
            closed.Add(current);
            open.Enqueue(current);

            while (open.Count > 0) {
                current = open.Dequeue();

                //Statuses
                State next = new State(current);
                if (hardMode && (current.Steps & 1) == 0) {
                    next.PlayerHP--;
                }
                next.Steps++;
                if (next.Recharge > 0) {
                    next.Recharge--;
                    next.PlayerMana += 101;
                }
                if (next.Poison > 0) {
                    next.Poison--;
                    next.BossHP -= 3;
                }
                if (next.Shield > 0) {
                    next.Shield--;
                }

                if (next.PlayerHP <= 0) {
                    continue;
                }
                if (next.BossHP <= 0) {
                    return next.SpentMana;
                }

                if ((current.Steps & 1) == 0) {
                    if (next.Recharge == 0 && next.PlayerMana >= 229) {
                        State recharge = new State(next);
                        recharge.PlayerMana -= 229;
                        recharge.SpentMana += 229;
                        recharge.Recharge = 5;

                        if (closed.Add(recharge)) { open.Enqueue(recharge); }
                    }

                    if (next.Poison == 0 && next.PlayerMana >= 173) {
                        State poison = new State(next);
                        poison.PlayerMana -= 173;
                        poison.SpentMana += 173;
                        poison.Poison = 6;

                        if (closed.Add(poison)) { open.Enqueue(poison); }
                    }

                    if (next.Shield == 0 && next.PlayerMana >= 113) {
                        State shield = new State(next);
                        shield.PlayerMana -= 113;
                        shield.SpentMana += 113;
                        shield.Shield = 6;

                        if (closed.Add(shield)) { open.Enqueue(shield); }
                    }

                    if (next.PlayerMana >= 73) {
                        State drain = new State(next);
                        drain.PlayerMana -= 73;
                        drain.SpentMana += 73;
                        drain.BossHP -= 2;
                        drain.PlayerHP += 2;

                        if (closed.Add(drain)) { open.Enqueue(drain); }
                    }

                    if (next.PlayerMana >= 53) {
                        State missile = new State(next);
                        missile.PlayerMana -= 53;
                        missile.SpentMana += 53;
                        missile.BossHP -= 4;

                        if (closed.Add(missile)) { open.Enqueue(missile); }
                    }
                } else {
                    next.PlayerHP -= next.Shield > 0 ? (short)1 : (short)8;

                    if (next.PlayerHP > 0 && closed.Add(next)) {
                        open.Enqueue(next);
                    }
                }
            }

            return -1;
        }

        private struct State : IComparable<State>, IEquatable<State> {
            public byte Recharge;
            public byte Poison;
            public byte Shield;
            public short PlayerHP;
            public short BossHP;
            public byte BossDmg;
            public short PlayerMana;
            public short SpentMana;
            public int Steps;

            public State(State copy) {
                Recharge = copy.Recharge;
                Poison = copy.Poison;
                Shield = copy.Shield;
                PlayerHP = copy.PlayerHP;
                PlayerMana = copy.PlayerMana;
                BossHP = copy.BossHP;
                BossDmg = copy.BossDmg;
                SpentMana = copy.SpentMana;
                Steps = copy.Steps;
            }
            public int CompareTo(State other) {
                int compare = SpentMana.CompareTo(other.SpentMana);
                if (compare != 0) { return compare; }

                compare = Steps.CompareTo(other.Steps);
                if (compare != 0) { return compare; }

                return BossHP.CompareTo(other.BossHP);
            }
            public bool Equals(State other) {
                return PlayerMana == other.PlayerMana && PlayerHP == other.PlayerHP
                    && BossHP == other.BossHP && BossDmg == other.BossDmg
                    && SpentMana == other.SpentMana && Recharge == other.Recharge
                    && Poison == other.Poison && Shield == other.Shield;
            }
            public override bool Equals(object obj) {
                return obj is State state && Equals(state);
            }
            public override int GetHashCode() {
                return ((((PlayerHP * 31 + PlayerMana) * 31 + BossHP) * 31 + Recharge) * 31 + Poison) * 31 + Shield;
            }
            public override string ToString() {
                return $"HP: {PlayerHP,2} EN: {PlayerMana,3} Boss: {BossHP,2} Recharge: {Recharge} Poison: {Poison} Shield: {Shield} Spent: {SpentMana}";
            }
        }
    }
}