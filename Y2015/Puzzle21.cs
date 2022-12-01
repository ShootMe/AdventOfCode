using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("RPG Simulator 20XX")]
    public class Puzzle21 : ASolver {
        [Description("What is the least amount of gold you can spend and still win the fight?")]
        public override string SolvePart1() {
            List<string> lines = Tools.GetLines(Input);
            int hp = Tools.ParseInt(lines[0]);
            int dmg = Tools.ParseInt(lines[1]);
            int armor = Tools.ParseInt(lines[2]);

            return $"{PlayGame(100, hp, dmg, armor, false)}";
        }

        [Description("What is the most amount of gold you can spend and still lose the fight?")]
        public override string SolvePart2() {
            List<string> lines = Tools.GetLines(Input);
            int hp = Tools.ParseInt(lines[0]);
            int dmg = Tools.ParseInt(lines[1]);
            int armor = Tools.ParseInt(lines[2]);

            return $"{PlayGame(100, hp, dmg, armor, true)}";
        }

        private int PlayGame(int playerHP, int bossHP, int bossDmg, int bossArmor, bool loseFight) {
            Span<int> weapons = stackalloc int[] { 8, 4, 0, 10, 5, 0, 25, 6, 0, 40, 7, 0, 74, 8, 0 };
            Span<int> armors = stackalloc int[] { 13, 0, 1, 31, 0, 2, 53, 0, 3, 75, 0, 4, 102, 0, 5 };
            Span<int> rings = stackalloc int[] { 25, 1, 0, 50, 2, 0, 100, 3, 0, 20, 0, 1, 40, 0, 2, 80, 0, 3 };

            int playerDmg = 0;
            int playerArmor = 0;
            int goldCost = 0;
            int bestGold = loseFight ? int.MinValue : int.MaxValue;

            for (int i = 0; i < weapons.Length; i += 3) {
                goldCost = weapons[i];
                playerDmg = weapons[i + 1];
                playerArmor = weapons[i + 2];

                for (int j = 0; j <= armors.Length; j += 3) {
                    if (j < armors.Length) {
                        goldCost += armors[j];
                        playerDmg += armors[j + 1];
                        playerArmor += armors[j + 2];
                    }

                    for (int k = 0; k <= rings.Length; k += 3) {
                        if (k < rings.Length) {
                            goldCost += rings[k];
                            playerDmg += rings[k + 1];
                            playerArmor += rings[k + 2];
                        }

                        for (int m = 0; m <= rings.Length; m += 3) {
                            if (m != rings.Length && m == k) { continue; }

                            if (m < rings.Length) {
                                goldCost += rings[m];
                                playerDmg += rings[m + 1];
                                playerArmor += rings[m + 2];
                            }

                            int bossHit = bossDmg - playerArmor;
                            if (bossHit <= 0) { bossHit = 1; }

                            int playerHit = playerDmg - bossArmor;
                            if (playerHit <= 0) { playerHit = 1; }

                            int playerDeath = (playerHP + bossHit - 1) / bossHit;
                            int bossDeath = (bossHP + playerHit - 1) / playerHit;
                            if (loseFight) {
                                if (playerDeath < bossDeath && goldCost > bestGold) {
                                    bestGold = goldCost;
                                }
                            } else if (playerDeath >= bossDeath && goldCost < bestGold) {
                                bestGold = goldCost;
                            }

                            if (m < rings.Length) {
                                goldCost -= rings[m];
                                playerDmg -= rings[m + 1];
                                playerArmor -= rings[m + 2];
                            }
                        }

                        if (k < rings.Length) {
                            goldCost -= rings[k];
                            playerDmg -= rings[k + 1];
                            playerArmor -= rings[k + 2];
                        }
                    }

                    if (j < armors.Length) {
                        goldCost -= armors[j];
                        playerDmg -= armors[j + 1];
                        playerArmor -= armors[j + 2];
                    }
                }
            }
            return bestGold;
        }
    }
}