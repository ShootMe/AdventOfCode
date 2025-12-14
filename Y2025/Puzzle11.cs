using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Reactor")]
    public class Puzzle11 : ASolver {
        private Dictionary<string, Node> nodesByName = new();
        public override void Setup() {
            string[] lines = Input.Split('\n');
            int nodes = 0;
            foreach (string line in lines) {
                int index = line.IndexOf(':');
                string name = line.Substring(0, index);
                if (!nodesByName.TryGetValue(name, out Node node)) {
                    node = new Node(nodes++, name);
                    nodesByName[name] = node;
                }

                string[] others = line.Substring(index + 2).Split(' ');
                for (int i = 0; i < others.Length; i++) {
                    string other = others[i];
                    if (!nodesByName.TryGetValue(other, out Node otherNode)) {
                        otherNode = new Node(nodes++, other);
                        nodesByName[other] = otherNode;
                    }
                    otherNode.Parents.Add(node);
                    node.Connections.Add(otherNode);
                }
            }
        }

        [Description("How many different paths lead from you to out?")]
        public override string SolvePart1() {
            if (nodesByName.TryGetValue("you", out Node node)) {
                Dictionary<Node, long> cache = new();
                return $"{Traverse(node, nodesByName["out"].ID, cache)}";
            }
            return string.Empty;
        }

        [Description("How many of those paths visit both dac and fft?")]
        public override string SolvePart2() {
            Dictionary<Node, long> cache = new();
            long route = Traverse(nodesByName["svr"], nodesByName["fft"].ID, cache);
            cache.Clear();
            route *= Traverse(nodesByName["fft"], nodesByName["dac"].ID, cache);
            cache.Clear();
            route *= Traverse(nodesByName["dac"], nodesByName["out"].ID, cache);
            long total = route;
            cache.Clear();
            route = Traverse(nodesByName["svr"], nodesByName["dac"].ID, cache);
            cache.Clear();
            route *= Traverse(nodesByName["dac"], nodesByName["fft"].ID, cache);
            cache.Clear();
            route *= Traverse(nodesByName["fft"], nodesByName["out"].ID, cache);
            total += route;
            return $"{total}";
        }

        private long Traverse(Node node, int targetID, Dictionary<Node, long> cache) {
            if (cache.TryGetValue(node, out long result)) { return result; }

            long found = 0;
            for (int i = 0; i < node.Connections.Count; i++) {
                Node other = node.Connections[i];
                if (other.ID == targetID) {
                    found++;
                } else {
                    found += Traverse(other, targetID, cache);
                }
            }
            cache[node] = found;
            return found;
        }

        private class Node : IComparable<Node>, IEquatable<Node> {
            public int ID;
            public string Name;
            public List<Node> Parents = new();
            public List<Node> Connections = new();
            public Node(int id, string name) { ID = id; Name = name; }
            public int CompareTo(Node other) {
                int comp = Connections.Count.CompareTo(other.Connections.Count);
                if (comp != 0) { return comp; }
                return Name.CompareTo(other.Name);
            }
            public bool Equals(Node other) { return ID == other.ID; }
            public override int GetHashCode() { return ID; }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{Name}: ");
                for (int i = 0; i < Connections.Count; i++) {
                    sb.Append($"{Connections[i].Name} ");
                }
                sb.Length--;
                return sb.ToString();
            }
        }
    }
}