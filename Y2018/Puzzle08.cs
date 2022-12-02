using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Memory Maneuver")]
    public class Puzzle08 : ASolver {
        private Node root;

        public override void Setup() {
            string[] items = Input.Split(' ');
            root = new Node();
            int index = 0;
            ParseNode(items, ref index, root);
        }

        private void ParseNode(string[] items, ref int index, Node node) {
            int nodeCount = items[index++].ToInt();
            int metaDataCount = items[index++].ToInt();

            for (int j = 0; j < nodeCount; j++) {
                Node newNode = new Node();
                node.Children.Add(newNode);
                ParseNode(items, ref index, newNode);
            }

            for (int j = 0; j < metaDataCount; j++) {
                int value = items[index++].ToInt();
                node.Metadata.Add(value);
            }
        }

        [Description("What is the sum of all metadata entries?")]
        public override string SolvePart1() {
            return $"{root.SumMetadata()}";
        }

        [Description("What is the value of the root node?")]
        public override string SolvePart2() {
            return $"{root.GetValue()}";
        }

        private class Node {
            public List<Node> Children = new List<Node>();
            public List<int> Metadata = new List<int>();

            public int GetValue() {
                int value = 0;
                if (Children.Count == 0) {
                    for (int i = 0; i < Metadata.Count; i++) {
                        value += Metadata[i];
                    }
                    return value;
                }
                for (int i = 0; i < Metadata.Count; i++) {
                    int index = Metadata[i] - 1;
                    if (index >= 0 && index < Children.Count) {
                        value += Children[index].GetValue();
                    }
                }
                return value;
            }
            public int SumMetadata() {
                int sum = 0;
                for (int i = 0; i < Metadata.Count; i++) {
                    sum += Metadata[i];
                }
                for (int i = 0; i < Children.Count; i++) {
                    sum += Children[i].SumMetadata();
                }
                return sum;
            }
        }
    }
}