using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2019 {
    [Description("Many-Worlds Interpretation")]
    public class Puzzle18 : ASolver {
        private byte[] vault;
        private int width, height, start;
        private uint allKeys;

        public override void Setup() {
            List<string> items = Input.Lines();
            width = items[0].Length;
            height = items.Count;
            vault = new byte[width * height];
            int index = 0;
            allKeys = 0;

            for (int i = 0; i < height; i++) {
                string row = items[i];

                for (int j = 0; j < width; j++) {
                    char tile = row[j];
                    if (tile == '#') {
                        vault[index++] = 255;
                    } else if (tile == '@') {
                        start = index;
                        vault[index++] = 254;
                    } else if (char.IsLetter(tile)) {
                        byte value = tile >= 'a' ? (byte)(tile - 'a') : (byte)(tile - 'A' + 26);
                        if (value < 26) {
                            allKeys |= 1u << value;
                        }
                        vault[index++] = value;
                    } else {
                        vault[index++] = 253;
                    }
                }
            }
        }

        [Description("How many steps is the shortest path that collects all of the keys?")]
        public override string SolvePart1() {
            HashSet<State> closed = new HashSet<State>(2000000);
            Heap<State> open = new Heap<State>(5000);

            State current = new State() { Position = start, Keys = 0, Steps = 0 };
            closed.Add(current);
            open.Enqueue(current);

            while (open.Count > 0) {
                current = open.Dequeue();

                byte tile = vault[current.Position];
                uint nextKeys = current.Keys;
                if (tile < 26) {
                    nextKeys |= 1u << tile;
                }

                if (nextKeys == allKeys) {
                    return $"{current.Steps}";
                }

                UpdateMoves(current, nextKeys, closed, open);
            }

            return string.Empty;
        }

        [Description("What is the fewest steps necessary to collect all of the keys?")]
        public override string SolvePart2() {
            HashSet<State> closed = new HashSet<State>(2000000);
            Heap<State> open = new Heap<State>(5000);

            vault[start - width] = 255;
            vault[start + width] = 255;
            vault[start - 1] = 255;
            vault[start + 1] = 255;

            int start1 = start - width - 1;
            int start2 = start - width + 1;
            int start3 = start + width - 1;
            int start4 = start + width + 1;

            State current = new State() { Position = start1, Position1 = start1, Position2 = start2, Position3 = start3, Position4 = start4, Robot = 0, Keys = 0, Steps = 0 };
            closed.Add(current);
            open.Enqueue(current);

            while (open.Count > 0) {
                current = open.Dequeue();

                byte tile = vault[current.Position];
                uint nextKeys = current.Keys;
                if (tile < 26) {
                    nextKeys |= 1u << tile;
                }

                if (nextKeys == allKeys) {
                    return $"{current.Steps}";
                }

                current.Position = current.Position1;
                current.Robot = 0;
                UpdateMoves(current, nextKeys, closed, open);
                current.Position = current.Position2;
                current.Robot = 1;
                UpdateMoves(current, nextKeys, closed, open);
                current.Position = current.Position3;
                current.Robot = 2;
                UpdateMoves(current, nextKeys, closed, open);
                current.Position = current.Position4;
                current.Robot = 3;
                UpdateMoves(current, nextKeys, closed, open);
            }

            return string.Empty;
        }

        private void UpdateMoves(State current, uint nextKeys, HashSet<State> closed, Heap<State> open) {
            int x = current.Position % width;
            State up = current.Copy(nextKeys, current.Position - width);
            Validate(up, up.Position, closed, open);
            State down = current.Copy(nextKeys, current.Position + width);
            Validate(down, down.Position, closed, open);
            State left = current.Copy(nextKeys, x > 0 ? current.Position - 1 : -1);
            Validate(left, left.Position, closed, open);
            State right = current.Copy(nextKeys, x + 1 < width ? current.Position + 1 : -1);
            Validate(right, right.Position, closed, open);
        }
        private void Validate(State state, int position, HashSet<State> closed, Heap<State> open) {
            byte value = position < 0 || position >= vault.Length ? (byte)255 : vault[position];
            bool valid = value != 255 && (value == 254 || value == 253 || value < 26 || (state.Keys & (1u << (value - 26))) != 0);
            if (valid && !closed.Contains(state)) {
                closed.Add(state);
                open.Enqueue(state);
            }
        }

        private struct State : IComparable<State>, IEquatable<State> {
            public int Position, Position1, Position2, Position3, Position4;
            public int Robot;
            public uint Keys;
            public int Steps;

            public State Copy(uint newKeys, int position) {
                return new State() { Robot = Robot, Keys = newKeys, Position = position, Position1 = Robot == 0 ? position : Position1, Position2 = Robot == 1 ? position : Position2, Position3 = Robot == 2 ? position : Position3, Position4 = Robot == 3 ? position : Position4, Steps = Steps + 1 };
            }
            public int CompareTo(State other) {
                int compare = Steps.CompareTo(other.Steps);
                if (compare != 0) { return compare; }

                return BitOperations.PopCount(Keys).CompareTo(BitOperations.PopCount(other.Keys));
            }
            public bool Equals(State other) {
                return Keys == other.Keys && Position == other.Position;
            }
            public override bool Equals(object obj) {
                return obj is State state && Equals(state);
            }
            public override int GetHashCode() {
                return (int)(Position * 31 + Keys);
            }
            public override string ToString() {
                return $"{Position} {Convert.ToString(Keys, 2)} {Steps} {Robot}";
            }
        }
    }
}