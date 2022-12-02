using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Crab Combat")]
    public class Puzzle22 : ASolver {
        private List<int> player1, player2;

        public override void Setup() {
            List<string> items = Input.Lines();

            player1 = new List<int>();
            player2 = new List<int>();

            int i = 1;
            while (i < items.Count) {
                string item = items[i++];
                if (string.IsNullOrEmpty(item)) { break; }

                player1.Add(Tools.ParseInt(item));
            }

            i++;
            while (i < items.Count) {
                string item = items[i++];
                if (string.IsNullOrEmpty(item)) { break; }

                player2.Add(Tools.ParseInt(item));
            }
        }

        [Description("What is the winning player's score?")]
        public override string SolvePart1() {
            List<int> p1 = new List<int>(player1);
            List<int> p2 = new List<int>(player2);
            while (p1.Count > 0 && p2.Count > 0) {
                int val1 = p1[0];
                p1.RemoveAt(0);

                int val2 = p2[0];
                p2.RemoveAt(0);

                if (val1 > val2) {
                    p1.Add(val1);
                    p1.Add(val2);
                } else {
                    p2.Add(val2);
                    p2.Add(val1);
                }
            }

            return $"{CalculateScore(p1.Count > 0 ? p1 : p2)}";
        }

        [Description("What is the winning player's score?")]
        public override string SolvePart2() {
            List<int> p1 = new List<int>(player1);
            List<int> p2 = new List<int>(player2);
            bool p1Won = PlayRecursive(p1, p2);

            return $"{CalculateScore(p1Won ? p1 : p2)}";
        }

        private bool PlayRecursive(List<int> p1, List<int> p2) {
            HashSet<List<int>> seen = new HashSet<List<int>>(ArrayComparer<int>.Comparer);

            while (p1.Count > 0 && p2.Count > 0) {
                if (!seen.Add(p1) || !seen.Add(p2)) { return true; }

                int val1 = p1[0];
                p1.RemoveAt(0);

                int val2 = p2[0];
                p2.RemoveAt(0);

                bool p1Won = val1 > val2;
                if (val1 <= p1.Count && val2 <= p2.Count) {
                    List<int> p1Copy = GetCopy(p1, val1);
                    List<int> p2Copy = GetCopy(p2, val2);
                    p1Won = PlayRecursive(p1Copy, p2Copy);
                }

                if (p1Won) {
                    p1.Add(val1);
                    p1.Add(val2);
                } else {
                    p2.Add(val2);
                    p2.Add(val1);
                }
            }

            return p1.Count > 0;
        }
        private List<int> GetCopy(List<int> player, int count) {
            List<int> copy = new List<int>(count);
            for (int i = 0; i < count; i++) {
                copy.Add(player[i]);
            }
            return copy;
        }
        private int CalculateScore(List<int> winner) {
            int score = 0;
            for (int i = winner.Count - 1, j = 1; i >= 0; i--, j++) {
                score += winner[i] * j;
            }
            return score;
        }
    }
}