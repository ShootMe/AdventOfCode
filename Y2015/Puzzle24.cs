using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("It Hangs in the Balance")]
    public class Puzzle24 : ASolver {
        private int[] weights;
        private int totalSum;

        public override void Setup() {
            weights = Tools.GetInts(Input);
            totalSum = 0;
            for (int i = 0; i < weights.Length; i++) {
                totalSum += weights[i];
            }
            Array.Sort(weights, delegate (int left, int right) { return right.CompareTo(left); });
        }

        [Description("What is the quantum entanglement of the first group of packages in the ideal configuration?")]
        public override string SolvePart1() {
            int neededSum = totalSum / 3;
            List<int[]> groupOnes = new List<int[]>();
            int minCount = int.MaxValue;
            FindMinGrouping(groupOnes, new bool[weights.Length], new List<int>(), 0, 0, neededSum, ref minCount);

            List<Grouping> groupings = new List<Grouping>();
            for (int i = 0; i < groupOnes.Count; i++) {
                groupings.Add(new Grouping() { One = groupOnes[i], Count = 1, Quantum = Grouping.QuantumEntanglement(groupOnes[i]) });
            }
            groupings.Sort((left, right) => left.Quantum.CompareTo(right.Quantum));

            for (int i = 0; i < groupings.Count; i++) {
                Grouping best = groupings[i];

                bool[] used = new bool[weights.Length];
                int index = 1;
                int current = best.One[0];

                for (int j = 0; j < weights.Length; j++) {
                    int weight = weights[j];
                    if (weight == current) {
                        used[j] = true;
                        if (index == best.One.Length) { break; }
                        current = best.One[index++];
                    }
                }

                AddGrouping(best, used, new List<int>(), 0, 0, 3, neededSum);
                if (best.Count == 3) {
                    return $"{best.Quantum}";
                }
            }

            return string.Empty;
        }

        [Description("What is the quantum entanglement of the first group of packages in the ideal configuration?")]
        public override string SolvePart2() {
            int neededSum = totalSum / 4;
            List<int[]> groupOnes = new List<int[]>();
            int minCount = int.MaxValue;
            FindMinGrouping(groupOnes, new bool[weights.Length], new List<int>(), 0, 0, neededSum, ref minCount);

            List<Grouping> groupings = new List<Grouping>();
            for (int i = 0; i < groupOnes.Count; i++) {
                groupings.Add(new Grouping() { One = groupOnes[i], Count = 1, Quantum = Grouping.QuantumEntanglement(groupOnes[i]) });
            }
            groupings.Sort((left, right) => left.Quantum.CompareTo(right.Quantum));

            for (int i = 0; i < groupings.Count; i++) {
                Grouping best = groupings[i];

                bool[] used = new bool[weights.Length];
                int index = 1;
                int current = best.One[0];

                for (int j = 0; j < weights.Length; j++) {
                    int weight = weights[j];
                    if (weight == current) {
                        used[j] = true;
                        if (index == best.One.Length) { break; }
                        current = best.One[index++];
                    }
                }

                AddGrouping(best, used, new List<int>(), 0, 0, 4, neededSum);
                if (best.Count == 4) {
                    return $"{best.Quantum}";
                }
            }

            return string.Empty;
        }

        private void FindMinGrouping(List<int[]> groupings, bool[] used, List<int> current, int sum, int index, int neededSum, ref int minCount) {
            for (int i = index; i < weights.Length; i++) {
                if (used[i]) { continue; }

                used[i] = true;
                int weight = weights[i];
                current.Add(weight);
                sum += weight;

                if (sum == neededSum && current.Count <= minCount) {
                    if (current.Count < minCount) {
                        groupings.Clear();
                        minCount = current.Count;
                    }
                    groupings.Add(current.ToArray());
                } else if (sum < neededSum && current.Count < minCount) {
                    FindMinGrouping(groupings, used, current, sum, i + 1, neededSum, ref minCount);
                }

                sum -= weight;
                current.RemoveAt(current.Count - 1);
                used[i] = false;
            }
        }
        private bool AddGrouping(Grouping group, bool[] used, List<int> current, int sum, int index, int count, int neededSum) {
            for (int i = index; i < weights.Length; i++) {
                if (used[i]) { continue; }

                used[i] = true;
                int weight = weights[i];
                current.Add(weight);
                sum += weight;

                if (sum == neededSum && current.Count >= group.One.Length) {
                    group.Add(current);
                    if (group.Count == count) {
                        return true;
                    } else {
                        int min = current[0];
                        for (int j = 0; j < weights.Length; j++) {
                            if (weights[j] == min) {
                                min = j + 1;
                                break;
                            }
                        }

                        bool found = AddGrouping(group, used, new List<int>(), 0, min, count, neededSum);
                        if (found) { return true; }
                    }
                    group.Remove();
                } else if (sum < neededSum) {
                    bool found = AddGrouping(group, used, current, sum, i + 1, count, neededSum);
                    if (found) { return true; }
                }

                sum -= weight;
                current.RemoveAt(current.Count - 1);
                used[i] = false;
            }
            return false;
        }

        private class Grouping : IEquatable<Grouping> {
            public int Count;
            public long Quantum = -1;
            public int[] One, Two, Three, Four;

            public void Add(List<int> group) {
                int[] temp = group.ToArray();
                Four = Three;
                Three = Two;
                Two = temp;
                Count++;
            }
            public void Remove() {
                switch (Count) {
                    case 4: Four = null; break;
                    case 3: Three = null; break;
                    case 2: Two = null; break;
                    case 1: One = null; break;
                }
                Count--;
            }
            public static long QuantumEntanglement(int[] weights) {
                long result = 1;
                for (int i = 0; i < weights.Length; i++) {
                    result *= weights[i];
                }
                return result;
            }
            public Grouping Copy() {
                Grouping copy = new Grouping();
                copy.Count = Count;
                copy.One = new int[One.Length];
                copy.Two = new int[Two.Length];
                copy.Three = new int[Three.Length];
                Array.Copy(One, copy.One, One.Length);
                Array.Copy(Two, copy.Two, Two.Length);
                Array.Copy(Three, copy.Three, Three.Length);
                return copy;
            }
            public bool Equals(Grouping other) {
                if (One.Length != other.One.Length || Two.Length != other.Two.Length || Three.Length != other.Three.Length) {
                    return false;
                }

                bool result = ArrayComparer<int>.Comparer.Equals(One, other.One);
                result = result && ArrayComparer<int>.Comparer.Equals(Two, other.Two);
                result = result && ArrayComparer<int>.Comparer.Equals(Three, other.Three);
                return result;
            }
            public override bool Equals(object obj) {
                return obj is Grouping grouping && Equals(grouping);
            }
            public override int GetHashCode() {
                return ArrayComparer<int>.Comparer.GetHashCode(One) + ArrayComparer<int>.Comparer.GetHashCode(Two) + ArrayComparer<int>.Comparer.GetHashCode(Three);
            }
            public override string ToString() {
                return $"{{{ArrayComparer<int>.Comparer.ToString(One)}}} {{{ArrayComparer<int>.Comparer.ToString(Two)}}} {{{ArrayComparer<int>.Comparer.ToString(Three)}}}";
            }
        }
    }
}