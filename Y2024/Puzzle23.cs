using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2024 {
    [Description("LAN Party")]
    public class Puzzle23 : ASolver {
        private List<HashSet<Node>> cliques = new();
        private List<Node> nodes = new();
        public override void Setup() {
            Dictionary<string, Node> nodesByName = new();
            string[] lines = Input.Split('\n');
            
            foreach (string line in lines) {
                int index = line.IndexOf('-');
                string name = line.Substring(0, index);

                if (!nodesByName.TryGetValue(name, out Node node)) {
                    node = new Node(nodes.Count, name);
                    nodesByName.Add(node.Name, node);
                    nodes.Add(node);
                }

                string other = line.Substring(index + 1);
                if (!nodesByName.TryGetValue(other, out Node otherNode)) {
                    otherNode = new Node(nodes.Count, other);
                    nodesByName.Add(otherNode.Name, otherNode);
                    nodes.Add(otherNode);
                }

                node.Connections.Add(otherNode);
                otherNode.Connections.Add(node);
            }
            nodes.Sort();

            for (int i = 0; i < nodes.Count; i++) {
                Node node = nodes[i];
                Find(cliques, node, new(), new());
            }
        }

        [Description("How many contain at least one computer with a name that starts with t?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < nodes.Count; i++) {
                Node node1 = nodes[i];
                bool hasT1 = node1.Name[0] == 't';

                for (int j = 0; j < node1.Connections.Count; j++) {
                    Node node2 = node1.Connections[j];
                    bool hasT2 = node2.Name[0] == 't';

                    for (int k = 0; k < node2.Connections.Count; k++) {
                        Node node3 = node2.Connections[k];
                        if (node3.Equals(node1)) { continue; }

                        if (!hasT1 && !hasT2 && node3.Name[0] != 't') { continue; }

                        for (int m = 0; m < node3.Connections.Count; m++) {
                            Node node4 = node3.Connections[m];
                            if (node4.Equals(node1)) {
                                total++;
                                break;
                            }
                        }
                    }
                }
            }
            return $"{total / 6}";
        }

        [Description("What is the password to get into the LAN party?")]
        public override string SolvePart2() {
            int best = 0;
            HashSet<Node> maxClique = null;
            for (int i = 0; i < cliques.Count; i++) {
                HashSet<Node> clique = cliques[i];
                if (clique.Count > best) {
                    best = clique.Count;
                    maxClique = clique;
                }
            }

            List<Node> list = new(maxClique);
            list.Sort();
            StringBuilder sb = new();
            for (int i = 0; i < list.Count; i++) {
                sb.Append(list[i].Name).Append(',');
            }
            sb.Length--;

            return sb.ToString();
        }
        private void Find(List<HashSet<Node>> cliques, Node node, HashSet<Node> set, HashSet<Node> exc) {
            List<Node> connections = node.Connections;
            int count = 0;
            for (int i = 0; i < connections.Count && count < set.Count; i++) {
                Node connection = connections[i];
                if (set.Contains(connection)) { count++; }
            }

            if (count == set.Count) {
                set.Add(node);
                exc.Add(node);
                if (set.Count > 2) { cliques.Add(new(set)); }

                for (int i = 0; i < connections.Count; i++) {
                    Node connection = connections[i];
                    if (!exc.Contains(connection)) {
                        Find(cliques, connection, set, exc);
                    }
                }
                set.Remove(node);
            }
        }
        private class Node : IComparable<Node>, IEquatable<Node> {
            public int ID;
            public string Name;
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