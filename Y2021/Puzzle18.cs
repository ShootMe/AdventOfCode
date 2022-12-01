using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
namespace AdventOfCode.Y2021 {
    [Description("Snailfish")]
    public class Puzzle18 : ASolver {
        private List<Snailfish> fishes;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            fishes = new List<Snailfish>();
            for (int i = 0; i < items.Count; i++) {
                fishes.Add(Snailfish.ReadSnailfish(items[i]));
            }
        }

        [Description("What is the magnitude of the final sum?")]
        public override string SolvePart1() {
            Snailfish snailfish = fishes[0];
            for (int i = 1; i < fishes.Count; i++) {
                snailfish += fishes[i];
            }
            return $"{snailfish.Magnitude()}";
        }

        [Description("What is the largest magnitude of two different snailfish numbers?")]
        public override string SolvePart2() {
            int best = 0;
            for (int i = 0; i < fishes.Count; i++) {
                Snailfish snailfish = fishes[i];
                for (int j = i + 1; j < fishes.Count; j++) {
                    Snailfish sum = snailfish + fishes[j];
                    int magnitude = sum.Magnitude();
                    if (magnitude > best) {
                        best = magnitude;
                    }
                    sum = fishes[j] + snailfish;
                    magnitude = sum.Magnitude();
                    if (magnitude > best) {
                        best = magnitude;
                    }
                }
            }
            return $"{best}";
        }

        private class Snailfish {
            private SnailfishNode[] nodes;
            private Snailfish(SnailfishNode[] nodes) {
                this.nodes = nodes;
            }

            public static Snailfish ReadSnailfish(string input) {
                SnailfishNode[] nodes = new SnailfishNode[32];
                int index = 0;
                byte level = 0;
                int start = 0;
                int add = nodes.Length;
                for (int i = 0; i < input.Length; i++) {
                    char current = input[i];
                    if (char.IsDigit(current)) { continue; }

                    if (i - start > 0) {
                        nodes[index] = new SnailfishNode() {
                            Level = level,
                            Value = (byte)Tools.ParseInt(input, start, i - start)
                        };
                    }

                    switch (current) {
                        case '[': add >>= 1; level++; break;
                        case ']': index -= add; add <<= 1; level--; break;
                        case ',': index += add; break;
                    }
                    start = i + 1;
                }

                return new Snailfish(nodes);
            }
            public static Snailfish operator +(Snailfish left, Snailfish right) {
                SnailfishNode[] nodes = new SnailfishNode[32];
                for (int i = 0, j = 0; i < 16; i++, j += 2) {
                    nodes[i] = left.nodes[j];
                    if (nodes[i].Level > 0) { nodes[i].Level++; }
                    nodes[i + 16] = right.nodes[j];
                    if (nodes[i + 16].Level > 0) { nodes[i + 16].Level++; }
                }

                Verify(nodes);
                return new Snailfish(nodes);
            }
            private static readonly int[] threes = new int[] { 1, 3, 9, 27, 81, 243 };
            public int Magnitude() {
                int magnitude = 0;
                for (int i = 0; i < 32; i += 2) {
                    SnailfishNode node = nodes[i];
                    if (node.Level == 0) { continue; }

                    int level = node.Level;
                    int index = i >> (5 - level);
                    int ones = BitOperations.PopCount((uint)index);
                    magnitude += node.Value * (1 << ones) * threes[4 - ones];
                }

                return magnitude;
            }
            private static void Verify(SnailfishNode[] nodes) {
                int index = HasMaxLevel(nodes);
                if (index < 0) { return; }

                byte newLevel;
                do {
                    do {
                        SnailfishNode node = nodes[index];
                        ExplodeLeft(nodes, index);
                        ExplodeRight(nodes, index + (nodes.Length >> node.Level));
                        nodes[index] = new SnailfishNode() { Level = 4 };
                    } while ((index = HasMaxLevel(nodes)) >= 0);

                    newLevel = 0;
                    while (newLevel < 5 && (index = HasSplit(nodes)) >= 0) {
                        SnailfishNode node = nodes[index];
                        newLevel = (byte)(node.Level + 1);
                        nodes[index].Value /= 2;
                        nodes[index].Level++;
                        int right = index + (nodes.Length >> newLevel);
                        nodes[right].Value = (byte)((node.Value + 1) / 2);
                        nodes[right].Level = newLevel;
                    }
                } while (newLevel > 4);
            }
            public static void ExplodeLeft(SnailfishNode[] nodes, int index) {
                int left = index;
                while (--left >= 0 && nodes[left].Level == 0) { }
                if (left < 0) { return; }

                nodes[left].Value += nodes[index].Value;
            }
            public static void ExplodeRight(SnailfishNode[] nodes, int index) {
                int right = index;
                while (++right < nodes.Length && nodes[right].Level == 0) { }
                byte value = nodes[index].Value;
                nodes[index] = default;
                if (right >= nodes.Length) { return; }

                nodes[right].Value += value;
            }
            private static int HasMaxLevel(SnailfishNode[] nodes) {
                for (int i = 0; i < 32; i += 2) {
                    if (nodes[i].Level == 5) { return i; }
                }
                return -1;
            }
            private static int HasSplit(SnailfishNode[] nodes) {
                for (int i = 0; i < 32; i += 2) {
                    SnailfishNode node = nodes[i];
                    if (node.Value > 9) { return i; }
                }
                return -1;
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                Serialize(sb, 0, nodes.Length / 2, 1);
                return sb.ToString();
            }
            private void Serialize(StringBuilder sb, int left, int right, int level) {
                SnailfishNode leftNode = nodes[left];
                if (leftNode.Level == level) {
                    sb.Append($"[{leftNode.Value},");
                } else {
                    sb.Append('[');
                    Serialize(sb, left, left + (nodes.Length >> (level + 1)), level + 1);
                    sb.Append(',');
                }

                SnailfishNode rightNode = nodes[right];
                if (rightNode.Level == level) {
                    sb.Append($"{rightNode.Value}]");
                } else {
                    Serialize(sb, right, right + (nodes.Length >> (level + 1)), level + 1);
                    sb.Append(']');
                }
            }
        }
        private struct SnailfishNode {
            public byte Level;
            public byte Value;
            public override string ToString() {
                return Level == 0 ? "Node" : $"Level={Level} Value={Value}";
            }
        }
    }
}