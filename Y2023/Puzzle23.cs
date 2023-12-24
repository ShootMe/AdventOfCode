using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("A Long Walk")]
    public class Puzzle23 : ASolver {
        private char[,] grid;
        private int width, height;
        private HashSet<Node> nodes;
        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;
            grid = new char[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    grid[y, x] = line[x];
                }
            }
            nodes = GenerateGraph();
        }

        [Description("How many steps long is the longest hike?")]
        public override string SolvePart1() {
            return $"{FindLongestPath(true)}";
        }

        [Description("How many steps long is the longest hike?")]
        public override string SolvePart2() {
            return $"{FindLongestPath(false)}";
        }
        private int FindLongestPath(bool original) {
            Node start = new Node(0, 1, 0);
            nodes.TryGetValue(start, out start);

            bool[] used = new bool[nodes.Count + 1];
            used[start.ID] = true;

            int[] dirD = { 0, 1, 0, -1, 0 };
            char[] dirC = { '>', 'v', '<', '^' };
            Stack<(Node, int)> open = new();

            for (int i = 0; i < 4; i++) {
                (Node node, int length) = start.Connections[i];
                if (node == null) { continue; }
                open.Push((node, length));
            }

            int max = 0;
            while (open.Count > 0) {
                (start, int total) = open.Pop();

                if (total >= 0) {
                    used[start.ID] = true;
                    open.Push((start, -1));

                    for (int i = 0; i < 4; i++) {
                        if (original) {
                            char c = grid[start.Y - dirD[i], start.X - dirD[i + 1]];
                            if (c == dirC[i]) { continue; }
                        }

                        (Node node, int length) = start.Connections[i];
                        if (node == null || used[node.ID]) { continue; }
                        if (node.Y == height - 1) {
                            if (total + length > max) { max = total + length; }
                            continue;
                        }
                        open.Push((node, total + length));
                    }
                } else {
                    used[start.ID] = false;
                }
            }
            return max;
        }
        private HashSet<Node> GenerateGraph() {
            int[,] gridMoves = new int[height, width];
            Queue<(Node, int)> open = new();
            Node start = new Node(1, 1, 0);
            open.Enqueue((start, 1));
            List<(int, int, int)> adjacent = new();
            HashSet<Node> nodes = new();
            nodes.Add(start);

            void AddAdjacent(int dir, int x, int y, Node node) {
                if (x < 0 || y < 0 || x >= width || y >= height || grid[y, x] == '#' || gridMoves[y, x] == node.ID) { return; }
                adjacent.Add((x, y, dir));
                gridMoves[y, x] = node.ID;
            }

            int[] dirD = { 0, 1, 0, -1, 0 };
            int nextNode = 2;
            while (open.Count > 0) {
                (Node node, int dir) = open.Dequeue();
                if (node.Connections[(dir + 2) & 3].Length > 0) { continue; }

                gridMoves[node.Y, node.X] = node.ID;

                int x = node.X + dirD[dir + 1];
                int y = node.Y + dirD[dir];
                gridMoves[y, x] = node.ID;
                int length = 1; int dirS = dir;

                while (true) {
                    adjacent.Clear();
                    AddAdjacent(0, x + 1, y, node);
                    AddAdjacent(1, x, y + 1, node);
                    AddAdjacent(2, x - 1, y, node);
                    AddAdjacent(3, x, y - 1, node);
                    if (adjacent.Count == 1) {
                        (x, y, dir) = adjacent[0];
                        length++;
                        continue;
                    }
                    break;
                }

                if (adjacent.Count > 0) {
                    Node newNode = new Node(nextNode, x, y);
                    bool addMore = true;
                    if (nodes.Add(newNode)) {
                        node.Connections[(dirS + 2) & 3] = (newNode, length);
                        newNode.Connections[dir] = (node, length);
                        nextNode++;
                    } else {
                        nodes.TryGetValue(newNode, out newNode);
                        newNode.Connections[dir] = (node, length);
                        node.Connections[(dirS + 2) & 3] = (newNode, length);
                        addMore = false;
                    }

                    for (int i = adjacent.Count - 1; i >= 0; i--) {
                        (x, y, dir) = adjacent[i];
                        gridMoves[y, x] = 0;
                        if (addMore) { open.Enqueue((newNode, dir)); }
                    }
                } else if (y == height - 1) {
                    Node end = new Node(nextNode, x, y);
                    nodes.Add(end);
                    end.Connections[dir] = (node, length);
                    node.Connections[(dirS + 2) & 3] = (end, length);
                    continue;
                }
            }

            return nodes;
        }
        private class Node : IEquatable<Node> {
            public int ID, X, Y;
            public (Node Node, int Length)[] Connections = new (Node, int)[4];
            public Node(int id, int x, int y) { ID = id; X = x; Y = y; }
            public override string ToString() { return $"({ID},{X},{Y})=({Connections[0].Length},{Connections[1].Length},{Connections[2].Length},{Connections[3].Length})"; }
            public override int GetHashCode() { return X * 999 + Y; }
            public bool Equals(Node other) { return X == other.X && Y == other.Y; }
        }
    }
}