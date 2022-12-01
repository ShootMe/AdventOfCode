using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2016 {
    [Description("Air Duct Spelunking")]
    public class Puzzle24 : ASolver {
        private byte[] ducting;
        private int width, height, start;
        private uint allKeys;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            width = items[0].Length;
            height = items.Count;
            ducting = new byte[width * height];
            int index = 0;
            allKeys = 0;

            for (int i = 0; i < height; i++) {
                string row = items[i];

                for (int j = 0; j < width; j++) {
                    char tile = row[j];
                    if (tile == '#') {
                        ducting[index++] = 255;
                    } else if (tile == '0') {
                        start = index;
                        ducting[index++] = 254;
                    } else if (char.IsDigit(tile)) {
                        byte value = (byte)(tile - '0');
                        allKeys |= 1u << value;
                        ducting[index++] = value;
                    } else {
                        ducting[index++] = 253;
                    }
                }
            }
        }

        [Description("What is the fewest steps required to visit every non-0 number marked on the map at least once?")]
        public override string SolvePart1() {
            HashSet<State> closed = new HashSet<State>(400000);
            Heap<State> open = new Heap<State>(5000);

            State current = new State() { Position = start, Keys = 0, Steps = 0 };
            closed.Add(current);
            open.Enqueue(current);

            while (open.Count > 0) {
                current = open.Dequeue();

                byte tile = ducting[current.Position];
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

        [Description("What is the fewest steps required to visit every non-0 number and then return to 0?")]
        public override string SolvePart2() {
            HashSet<State> closed = new HashSet<State>(600000);
            Heap<State> open = new Heap<State>(5000);

            State current = new State() { Position = start, Keys = 0, Steps = 0 };
            closed.Add(current);
            open.Enqueue(current);

            while (open.Count > 0) {
                current = open.Dequeue();

                byte tile = ducting[current.Position];
                uint nextKeys = current.Keys;
                if (tile < 26) {
                    nextKeys |= 1u << tile;
                }

                if (nextKeys == allKeys && current.Position == start) {
                    return $"{current.Steps}";
                }

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
            byte value = position < 0 || position >= ducting.Length ? (byte)255 : ducting[position];
            bool valid = value != 255 && (value == 254 || value == 253 || value < 26);
            if (valid && !closed.Contains(state)) {
                closed.Add(state);
                open.Enqueue(state);
            }
        }
        private struct State : IComparable<State>, IEquatable<State> {
            public int Position;
            public uint Keys;
            public int Steps;

            public State Copy(uint newKeys, int position) {
                return new State() { Keys = newKeys, Position = position, Steps = Steps + 1 };
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
                return $"{Position} {Convert.ToString(Keys, 2)} {Steps}";
            }
        }
    }
}