using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Grid Computing")]
    public class Puzzle22 : ASolver {
        private Node[] nodes, original;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            nodes = new Node[items.Count - 2];
            original = new Node[nodes.Length];
            int maxX = 0;
            int maxIndex = -1;
            for (int i = 2; i < items.Count; i++) {
                Node node = new Node(items[i]);
                nodes[i - 2] = node;
                if (node.X > maxX && node.Y == 0) {
                    maxX = node.X;
                    maxIndex = i - 2;
                }
            }

            nodes[maxIndex].HasGoal = true;
            Array.Copy(nodes, original, nodes.Length);
        }

        [Description("How many viable pairs of nodes are there?")]
        public override string SolvePart1() {
            return $"{GetMoves().Count}";
        }

        [Description("What is the fewest steps required to move the goal data to node-x0-y0?")]
        public override string SolvePart2() {
            HashSet<int> closed = new HashSet<int>();
            Heap<Move> open = new Heap<Move>();
            Move[] storage = new Move[400];

            open.Enqueue(new Move() { Start = -1, Distance = int.MaxValue });

            Node goal = FindGoal();
            Move finalMove = null;

            while (open.Count > 0) {
                Move current = open.Dequeue();

                if (current.Distance == 0) {
                    finalMove = current;
                    break;
                }

                Reset();

                int movesMade = CopyMoves(storage, current);
                for (int i = movesMade - 1; i >= 0; i--) {
                    Move move = storage[i];
                    ref Node start = ref nodes[move.Start];
                    ref Node end = ref nodes[move.End];

                    end.HasGoal |= start.HasGoal;
                    end.Used += start.Used;
                    start.Used = 0;
                    start.HasGoal = false;
                }

                List<Move> moves = GetMoves(current.Steps + 1, current.Start >= 0 ? current : null, goal);
                for (int i = 0; i < moves.Count; i++) {
                    Move move = moves[i];

                    if (closed.Add(move.Start)) {
                        open.Enqueue(move);
                    }
                }
            }

            Node lastNode = nodes[finalMove.End];
            finalMove.Distance = lastNode.X + lastNode.Y;

            return $"{finalMove.Distance * 5 + finalMove.Steps}";
        }

        private Node FindGoal() {
            for (int i = 0; i < nodes.Length; i++) {
                if (nodes[i].HasGoal) {
                    return nodes[i];
                }
            }
            return default;
        }
        private void Reset() {
            Array.Copy(original, nodes, nodes.Length);
        }
        private int CopyMoves(Move[] storage, Move current) {
            int count = 0;
            if (current.Start < 0) { return count; }

            while (current != null) {
                storage[count++] = current;
                current = current.Previous;
            }

            return count;
        }
        private List<Move> GetMoves(int steps, Move parent, Node goal) {
            List<Move> result = new List<Move>();
            for (int i = 0; i < nodes.Length; i++) {
                Node start = nodes[i];
                if (start.Used == 0) { continue; }

                for (int j = 0; j < nodes.Length; j++) {
                    if (i == j) { continue; }

                    Node end = nodes[j];
                    if (end.Used > 0) { continue; }

                    int xDis = Math.Abs(end.X - start.X);
                    int yDis = Math.Abs(end.Y - start.Y);
                    if (xDis + yDis <= 1 && end.Size > end.Used + start.Used) {
                        result.Add(new Move() { Steps = steps, Start = i, End = j, Previous = parent, Distance = Math.Abs(goal.X - start.X) + Math.Abs(goal.Y - start.Y) });
                    }
                }
            }
            return result;
        }
        private List<Move> GetMoves() {
            List<Move> result = new List<Move>();
            for (int i = 0; i < nodes.Length; i++) {
                Node start = nodes[i];
                if (start.Used == 0) { continue; }

                for (int j = 0; j < nodes.Length; j++) {
                    if (i == j) { continue; }

                    Node end = nodes[j];

                    if (end.Size > end.Used + start.Used) {
                        result.Add(new Move() { Start = i, End = j });
                    }
                }
            }
            return result;
        }
        private class Move : IComparable<Move> {
            public int Start;
            public int End;
            public int Steps;
            public int Distance;
            public Move Previous;

            public int CompareTo(Move other) {
                int compare = Distance.CompareTo(other.Distance);
                if (compare != 0) { return compare; }

                return Steps.CompareTo(other.Steps);
            }
            public override string ToString() {
                return $"{Start} -> {End} ({Distance})({Steps})";
            }
        }
        private struct Node {
            public bool HasGoal;
            public int X;
            public int Y;
            public int Size;
            public int Used;

            public Node(string value) {
                int index1 = value.IndexOf("node-");
                int index2 = value.IndexOf('-', index1 + 5);
                int index3 = value.IndexOf(' ', index2);

                X = Tools.ParseInt(value, index1 + 6, index2 - index1 - 6);
                Y = Tools.ParseInt(value, index2 + 2, index3 - index2 - 2);

                index1 = value.IndexOf('T', index3);
                Size = Tools.ParseInt(value, index3, index1 - index3);
                index2 = value.IndexOf('T', index1 + 1);
                Used = Tools.ParseInt(value, index1 + 1, index2 - index1 - 1);
                HasGoal = false;
            }
            public override string ToString() {
                return $"({X},{Y}) [{Used}/{Size}]";
            }
        }
    }
}