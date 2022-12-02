using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Mine Cart Madness")]
    public class Puzzle13 : ASolver {
        private List<Cart> carts;
        private int[] grid;
        private int width, height;

        public override void Setup() {
            List<string> items = Input.Lines();
            carts = new List<Cart>();

            width = items[0].Length;
            height = items.Count;
            grid = new int[width * height];
            int index = 0;
            for (int i = 0; i < height; i++) {
                string row = items[i];

                for (int j = 0; j < width; j++) {
                    char c = row[j];

                    switch (c) {
                        case '-':
                        case '|': grid[index] = 1; break;
                        case '/': grid[index] = 2; break;
                        case '\\': grid[index] = 3; break;
                        case '+': grid[index] = 4; break;
                        case '^':
                            carts.Add(new Cart() { Position = index, Direction = 1 });
                            grid[index] = 1;
                            break;
                        case 'v':
                            carts.Add(new Cart() { Position = index, Direction = 3 });
                            grid[index] = 1;
                            break;
                        case '>':
                            carts.Add(new Cart() { Position = index, Direction = 2 });
                            grid[index] = 1;
                            break;
                        case '<':
                            carts.Add(new Cart() { Position = index, Direction = 4 });
                            grid[index] = 1;
                            break;
                    }
                    index++;
                }
            }
        }

        [Description("What is the location fo the first crash?")]
        public override string SolvePart1() {
            List<Cart> part1 = new List<Cart>();
            for (int i = 0; i < carts.Count; i++) {
                part1.Add(new Cart(carts[i]));
            }

            while (true) {
                part1.Sort();

                for (int i = 0; i < part1.Count; i++) {
                    Cart cart = part1[i];
                    if (cart.Step(part1, grid, width, height) >= 0) {
                        return $"{cart.Position % width},{cart.Position / width}";
                    }
                }
            }
        }

        [Description("What is the location of the last cart after all other crashes?")]
        public override string SolvePart2() {
            while (true) {
                carts.Sort();

                for (int i = 0; i < carts.Count; i++) {
                    Cart cart = carts[i];
                    int crash = cart.Step(carts, grid, width, height);
                    if (crash >= 0) {
                        carts.RemoveAt(crash);

                        if (crash < i) { i--; }
                        carts.RemoveAt(i);
                        i--;
                    }
                }

                if (carts.Count == 1) {
                    return $"{carts[0].Position % width},{carts[0].Position / width}";
                }
            }
        }

        private class Cart : IComparable<Cart> {
            public int Position;
            public int Direction;
            public int NextTurn;//Left,Striaght,Right

            public Cart() { }
            public Cart(Cart copy) {
                Position = copy.Position;
                Direction = copy.Direction;
                NextTurn = copy.NextTurn;
            }

            public int Step(List<Cart> carts, int[] grid, int width, int height) {
                switch (Direction) {
                    case 1: Position -= width; break;
                    case 2: Position++; break;
                    case 3: Position += width; break;
                    case 4: Position--; break;
                }

                for (int i = 0; i < carts.Count; i++) {
                    Cart cart = carts[i];
                    if (cart.Position == Position && cart != this) {
                        return i;
                    }
                }

                int value = grid[Position];
                switch (value) {
                    case 2: Direction += (Direction & 1) == 1 ? 1 : -1; break;
                    case 3: Direction += (Direction & 1) == 1 ? Direction == 1 ? 3 : -1 : Direction == 4 ? -3 : 1; break;
                    case 4:
                        switch (NextTurn) {
                            case 0: Direction = ((Direction + 2) % 4) + 1; break;
                            case 2: Direction = (Direction % 4) + 1; break;
                        }
                        NextTurn = (NextTurn + 1) % 3;
                        break;
                }

                return -1;
            }
            public int CompareTo(Cart other) {
                return Position.CompareTo(other.Position);
            }
            public override string ToString() {
                return $"{Position} {Direction}";
            }
        }
    }
}