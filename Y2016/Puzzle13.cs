using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2016 {
    [Description("A Maze of Twisty Little Cubicles")]
    public class Puzzle13 : ASolver {
        private bool[] grid;

        public override void Setup() {
            int id = Input.ToInt();
            grid = new bool[100 * 100];
            int width = 100;
            int x = 0;
            int y = 0;
            for (int i = 0; i < grid.Length; i++) {
                uint value = (uint)(x * x + 3 * x + 2 * x * y + y + y * y + id);
                int bits = BitOperations.PopCount(value);
                if ((bits & 1) == 1) {
                    grid[i] = true;
                }

                x++;
                if (x == width) {
                    x = 0;
                    y++;
                }
            }
        }

        [Description("What is the fewest number of steps required for you to reach 31,39?")]
        public override string SolvePart1() {
            return $"{Explore()}";
        }

        [Description("How many distinct locations can you reach in at most 50 steps?")]
        public override string SolvePart2() {
            return $"{Explore(true)}";
        }

        private int Explore(bool limitSteps = false) {
            HashSet<Path> closed = new HashSet<Path>();
            Heap<Path> open = new Heap<Path>();
            Path current = new Path() { Position = new Point() { X = 1, Y = 1 }, Steps = 0 };
            closed.Add(current);
            open.Enqueue(current);

            while (open.Count > 0) {
                current = open.Dequeue();
                if (current.Position == Path.Goal) {
                    break;
                }

                Point position = current.Position;
                if (position.X > 0 && !grid[position.Y * 100 + position.X - 1]) {
                    Path next = new Path() { Position = new Point() { X = position.X - 1, Y = position.Y }, Steps = current.Steps + 1 };
                    if (!closed.Contains(next)) {
                        closed.Add(next);
                        if (!limitSteps || next.Steps < 50) {
                            open.Enqueue(next);
                        }
                    }
                }
                if (position.X + 1 < 100 && !grid[position.Y * 100 + position.X + 1]) {
                    Path next = new Path() { Position = new Point() { X = position.X + 1, Y = position.Y }, Steps = current.Steps + 1 };
                    if (!closed.Contains(next)) {
                        closed.Add(next);
                        if (!limitSteps || next.Steps < 50) {
                            open.Enqueue(next);
                        }
                    }
                }
                if (position.Y > 0 && !grid[(position.Y - 1) * 100 + position.X]) {
                    Path next = new Path() { Position = new Point() { X = position.X, Y = position.Y - 1 }, Steps = current.Steps + 1 };
                    if (!closed.Contains(next)) {
                        closed.Add(next);
                        if (!limitSteps || next.Steps < 50) {
                            open.Enqueue(next);
                        }
                    }
                }
                if (position.Y + 1 < 100 && !grid[(position.Y + 1) * 100 + position.X]) {
                    Path next = new Path() { Position = new Point() { X = position.X, Y = position.Y + 1 }, Steps = current.Steps + 1 };
                    if (!closed.Contains(next)) {
                        closed.Add(next);
                        if (!limitSteps || next.Steps < 50) {
                            open.Enqueue(next);
                        }
                    }
                }
            }
            return limitSteps ? closed.Count : current.Steps;
        }

        private struct Path : IComparable<Path>, IEquatable<Path> {
            public static Point Goal = new Point() { X = 31, Y = 39 };
            public Point Position;
            public int Steps;

            public int CompareTo(Path other) {
                int compare = Steps.CompareTo(other.Steps);
                if (compare != 0) { return compare; }
                return Position.Distance(Goal).CompareTo(other.Position.Distance(Goal));
            }
            public bool Equals(Path other) {
                return Position == other.Position;
            }
            public override bool Equals(object obj) {
                return obj is Path path && path.Equals(this);
            }
            public override int GetHashCode() {
                return Position.GetHashCode();
            }
        }
    }
}