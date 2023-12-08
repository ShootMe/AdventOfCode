using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Haunted Wasteland")]
    public class Puzzle08 : ASolver {
        private List<Node> startNodes = new();
        private string directions;
        public override void Setup() {
            string[] lines = Input.Split('\n');
            directions = lines[0];
            Dictionary<string, Node> allNodes = new(StringComparer.OrdinalIgnoreCase);
            for (int i = 2; i < lines.Length; i++) {
                string line = lines[i];
                int index = line.IndexOf(' ');
                string name = line.Substring(0, index);
                Node node = new Node(name);
                if (node.Type == NodeType.Start) {
                    if (node.Name == "AAA") {
                        startNodes.Insert(0, node);
                    } else {
                        startNodes.Add(node);
                    }
                }
                allNodes.Add(name, node);
            }
            for (int i = 2; i < lines.Length; i++) {
                string line = lines[i];
                string[] splits = line.SplitOn(" = (", ", ", ")");
                Node node = allNodes[splits[0]];
                node.Left = allNodes[splits[1]];
                node.Right = allNodes[splits[2]];
            }
        }

        [Description("How many steps are required to reach ZZZ?")]
        public override string SolvePart1() {
            return $"{GetStepCount(startNodes[0])}";
        }

        [Description("How many steps does it take before you're only on nodes that end with Z?")]
        public override string SolvePart2() {
            long total = 1;
            for (int i = 0; i < startNodes.Count; i++) {
                Node node = startNodes[i];
                long steps = GetStepCount(node, true);
                total *= steps / Extensions.GCD(total, steps);
            }
            return $"{total}";
        }
        private int GetStepCount(Node node, bool endsInZ = false) {
            int total = 0;
            int index = 0;
            while (true) {
                char c = directions[index++];
                total++;
                switch (c) {
                    case 'R': node = node.Right; break;
                    case 'L': node = node.Left; break;
                }
                if ((!endsInZ && node.Name == "ZZZ") || (endsInZ && node.Type == NodeType.End)) { break; }
                if (index >= directions.Length) { index = 0; }
            }
            return total;
        }

        private class Node {
            public string Name;
            public Node Left, Right;
            public NodeType Type;
            public Node(string name) {
                Name = name;
                Type = name.EndsWith('A') ? NodeType.Start : name.EndsWith('Z') ? NodeType.End : NodeType.Normal;
            }
        }
        private enum NodeType {
            Start,
            Normal,
            End
        }
    }
}