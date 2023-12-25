using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2023 {
    [Description("Snowverload")]
    public class Puzzle25 : ASolver {
        private List<Node> nodes = new();

        public override void Setup() {
            Dictionary<string, Node> nodesByName = new();
            int count = 0;
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                int index = line.IndexOf(':');
                string name = line.Substring(0, index);

                if (!nodesByName.TryGetValue(name, out Node node)) {
                    node = new Node(nodes.Count, name);
                    nodesByName.Add(node.Name, node);
                    nodes.Add(node);
                }

                int start = index + 2;
                while (start < line.Length) {
                    index = line.IndexOf(' ', start);
                    if (index < 0) { index = line.Length; }

                    string other = line.Substring(start, index - start);
                    start = index + 1;

                    if (!nodesByName.TryGetValue(other, out Node otherNode)) {
                        otherNode = new Node(nodes.Count, other);
                        nodesByName.Add(otherNode.Name, otherNode);
                        nodes.Add(otherNode);
                    }

                    Wire wire = new Wire(count++, node, otherNode);
                    node.Wires.Add(wire);
                    otherNode.Wires.Add(wire);
                }
            }
            nodes.Sort();
        }

        [Description("What do you get if you multiply the sizes of these two groups together?")]
        public override string SolvePart1() {
            HashSet<Wire> used = new();
            for (int i = 0; i < nodes.Count; i++) {
                Node node1 = nodes[i];
                for (int j = i + 1; j < nodes.Count; j++) {
                    Node node2 = nodes[j];
                    used.Clear();
                    int size;
                    if (FindPath(node1, node2, used) == 0 &&
                        FindPath(node1, node2, used) == 0 &&
                        FindPath(node1, node2, used) == 0 &&
                        (size = FindPath(node1, node2, used)) > 0) {
                        return $"{size * (nodes.Count - size)}";
                    }
                }
            }
            return string.Empty;
        }
        private int FindPath(Node node1, Node node2, HashSet<Wire> used) {
            Queue<Node> open = new();
            HashSet<Node> closed = new();
            open.Enqueue(node1);
            closed.Add(node1);
            Dictionary<Node, (Node, Wire)> paths = new();

            while (open.Count > 0) {
                Node current = open.Dequeue();
                if (current == node2) {
                    while (current != node1) {
                        (current, Wire last) = paths[current];
                        used.Add(last);
                    }
                    return 0;
                }

                for (int i = 0; i < current.Wires.Count; i++) {
                    Wire wire = current.Wires[i];
                    if (used.Contains(wire)) { continue; }

                    Node next = wire.Left;
                    if (wire.Left == current) { next = wire.Right; }
                    if (closed.Add(next)) {
                        open.Enqueue(next);
                        paths.Add(next, (current, wire));
                    }
                }
            }

            return closed.Count;
        }

        private struct Wire : IComparable<Wire>, IEquatable<Wire> {
            public int ID;
            public Node Left, Right;
            public Wire(int id, Node left, Node right) {
                ID = id;
                int comp = left.Name.CompareTo(right.Name);
                Left = comp < 0 ? left : right; Right = comp < 0 ? right : left;
            }
            public Node GetOther(Node node) { return Left.ID == node.ID ? Right : Left; }
            public int CompareTo(Wire other) { return Left.Name.CompareTo(other.Left.Name); }
            public bool Equals(Wire other) { return ID == other.ID; }
            public override int GetHashCode() { return ID; }
            public override string ToString() { return $"{Left.Name} {Right.Name}"; }
        }
        private class Node : IComparable<Node>, IEquatable<Node> {
            public int ID;
            public string Name;
            public List<Wire> Wires = new();
            public Node(int id, string name) { ID = id; Name = name; }
            public int CompareTo(Node other) { return Wires.Count.CompareTo(other.Wires.Count); }
            public bool Equals(Node other) { return ID == other.ID; }
            public override int GetHashCode() { return ID; }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{Name}: ");
                for (int i = 0; i < Wires.Count; i++) {
                    sb.Append($"{Wires[i].GetOther(this).Name} ");
                }
                sb.Length--;
                return sb.ToString();
            }
        }
    }
}