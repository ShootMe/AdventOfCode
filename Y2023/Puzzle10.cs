using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Pipe Maze")]
    public class Puzzle10 : ASolver {
        private string[] grid;
        private int width, height;
        private Node start;

        public override void Setup() {
            grid = Input.Split('\n');
            width = grid[0].Length;
            height = grid.Length;

            for (int y = 0; y < grid.Length; y++) {
                string row = grid[y];
                for (int x = 0; x < row.Length; x++) {
                    char c = row[x];
                    if (c == 'S') {
                        start = new Node() { X = x, Y = y };
                        return;
                    }
                }
            }
        }

        [Description("How many steps along the loop does it take to get to the point farthest from the starting position?")]
        public override string SolvePart1() {
            Queue<(Node Node, int Steps)> positions = new();
            HashSet<(int, int)> seen = new();
            positions.Enqueue((start, 0));

            while (positions.Count > 0) {
                (Node Node, int Steps) position = positions.Dequeue();
                position.Node.Value = grid[position.Node.Y][position.Node.X];

                if (position.Node.X - 1 >= 0 && ValidToW(position.Node, -1, 0) && seen.Add((position.Node.X - 1, position.Node.Y))) {
                    Node node = CreateNode(position.Node, -1, 0);
                    seen.Add((node.X, node.Y));
                    positions.Enqueue((node, position.Steps + 1));
                }
                if (position.Node.X + 1 < width && ValidToE(position.Node, 1, 0) && seen.Add((position.Node.X + 1, position.Node.Y))) {
                    Node node = CreateNode(position.Node, 1, 0);
                    seen.Add((node.X, node.Y));
                    positions.Enqueue((node, position.Steps + 1));
                }
                if (position.Node.Y - 1 >= 0 && ValidToN(position.Node, 0, -1) && seen.Add((position.Node.X, position.Node.Y - 1))) {
                    Node node = CreateNode(position.Node, 0, -1);
                    seen.Add((node.X, node.Y));
                    positions.Enqueue((node, position.Steps + 1));
                }
                if (position.Node.Y + 1 < height && ValidToS(position.Node, 0, 1) && seen.Add((position.Node.X, position.Node.Y + 1))) {
                    Node node = CreateNode(position.Node, 0, 1);
                    seen.Add((node.X, node.Y));
                    positions.Enqueue((node, position.Steps + 1));
                }
            }
            return $"{(seen.Count + 1) / 2}";
        }

        [Description("How many tiles are enclosed by the loop?")]
        public override string SolvePart2() {
            Node current = start;
            int sum = 0;
            int length = 1;
            while (true) {
                length++;
                Node last = current.Last;
                sum += current.X * last.Y - current.Y * last.X;
                if (last.Last == null) {
                    sum += last.X * start.Y - last.Y * start.X;
                    break;
                }
                current = current.Last;
            }
            return $"{(Math.Abs(sum) - length) / 2 + 1}";
        }

        private Node CreateNode(Node last, int x, int y) {
            Node node = new Node() { X = last.X + x, Y = last.Y + y };
            if (last.Last == null) {
                last.Last = node;
            } else {
                node.Last = last;
                start = node;
            }
            return node;
        }
        private bool ValidToW(Node node, int x, int y) {
            char c = grid[node.Y + y][node.X + x];
            return (node.Value == 'J' || node.Value == '-' || node.Value == '7' || node.Value == 'S') && (c == '-' || c == 'L' || c == 'F');
        }
        private bool ValidToE(Node node, int x, int y) {
            char c = grid[node.Y + y][node.X + x];
            return (node.Value == 'F' || node.Value == '-' || node.Value == 'L' || node.Value == 'S') && (c == '-' || c == 'J' || c == '7');
        }
        private bool ValidToN(Node node, int x, int y) {
            char c = grid[node.Y + y][node.X + x];
            return (node.Value == 'J' || node.Value == '|' || node.Value == 'L' || node.Value == 'S') && (c == '|' || c == 'F' || c == '7');
        }
        private bool ValidToS(Node node, int x, int y) {
            char c = grid[node.Y + y][node.X + x];
            return (node.Value == 'F' || node.Value == '|' || node.Value == '7' || node.Value == 'S') && (c == '|' || c == 'J' || c == 'L');
        }

        private class Node {
            public Node Last;
            public int X, Y;
            public char Value;
            public override string ToString() { return $"{X},{Y}"; }
        }
    }
}