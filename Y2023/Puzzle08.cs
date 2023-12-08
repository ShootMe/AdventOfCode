using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Haunted Wasteland")]
    public class Puzzle08 : ASolver {
        private List<Node> nodes = new();
        private string directions;
        public override void Setup() {
            Node.ALL.Clear();
            string[] lines = Input.Split('\n');
            directions = lines[0];
            for (int i = 2; i < lines.Length; i++) {
                string line = lines[i];
                string[] splits = line.SplitOn(" = (", ", ", ")");
                nodes.Add(new Node(splits[0], splits[1], splits[2]));
            }
        }

        [Description("How many steps are required to reach ZZZ?")]
        public override string SolvePart1() {
            return $"{GetStepCount(Node.ALL["AAA"])}";
        }

        [Description("How many steps does it take before you're only on nodes that end with Z?")]
        public override string SolvePart2() {
            long total = 1;
            for (int i = 0; i < nodes.Count; i++) {
                Node node = nodes[i];
                if (node.Type == NodeType.Start) {
                    long steps = GetStepCount(node, true);
                    total *= steps / Extensions.GCD(total, steps);
                }
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
                    case 'R': node = node.RightNode; break;
                    case 'L': node = node.LeftNode; break;
                }
                if ((!endsInZ && node.Name == "ZZZ") || (endsInZ && node.Type == NodeType.End)) { break; }
                if (index >= directions.Length) { index = 0; }
            }
            return total;
        }

        private class Node {
            public static Dictionary<string, Node> ALL = new Dictionary<string, Node>();
            public string Name, Left, Right;
            public NodeType Type;
            public Node(string name, string left, string right) {
                Name = name;
                Type = name.EndsWith('A') ? NodeType.Start : name.EndsWith('Z') ? NodeType.End : NodeType.Normal;
                Left = left;
                Right = right;
                ALL.Add(name, this);
            }
            public Node RightNode { get { return ALL[Right]; } }
            public Node LeftNode { get { return ALL[Left]; } }
        }
        private enum NodeType {
            Start,
            Normal,
            End
        }
    }
}