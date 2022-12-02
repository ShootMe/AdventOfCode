using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2019 {
    [Description("Planet of Discord")]
    public class Puzzle24 : ASolver {
        private bool[] grid, tempState, original;
        private int width, height;

        public override void Setup() {
            List<string> items = Input.Lines();

            height = items.Count;
            width = items[0].Length;
            grid = new bool[height * width];
            tempState = new bool[height * width];
            original = new bool[height * width];
            int index = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                for (int j = 0; j < item.Length; j++) {
                    char s = item[j];
                    grid[index++] = s != '.';
                }
            }
            Array.Copy(grid, tempState, grid.Length);
            Array.Copy(grid, original, grid.Length);
        }

        [Description("What is the biodiversity rating for the first layout that appears twice?")]
        public override string SolvePart1() {
            Array.Copy(original, grid, grid.Length);
            HashSet<bool[]> states = new HashSet<bool[]>(ArrayComparer<bool>.Comparer);

            while (Simulate()) {
                bool[] newState = new bool[grid.Length];
                Array.Copy(grid, newState, grid.Length);
                if (!states.Add(newState)) {
                    int biodiversity = 0;
                    int add = 1;
                    for (int i = 0; i < grid.Length; i++) {
                        if (grid[i]) {
                            biodiversity += add;
                        }
                        add <<= 1;
                    }
                    return $"{biodiversity}";
                }
            }

            return string.Empty;
        }

        [Description("How many bugs are present after 200 minutes?")]
        public override string SolvePart2() {
            Fold fold = new Fold();
            for (int i = 0; i < grid.Length; i++) {
                fold[i] = original[i] ? 1 : 0;
            }

            for (int i = 0; i < 200; i++) {
                if (!fold.Simulate(true)) {
                    fold = fold.Minus;
                    i--;
                }
            }

            return $"{fold.CountFilled()}";
        }

        private bool Simulate() {
            int x = 0;
            int y = 0;
            bool changedState = false;
            for (int i = 0; i < grid.Length; i++) {
                bool pos = grid[i];
                byte tm = GetValue(x, y, 0, -1);
                byte ml = GetValue(x, y, -1, 0);
                byte mr = GetValue(x, y, 1, 0);
                byte bm = GetValue(x, y, 0, 1);
                int count = tm + ml + mr + bm;

                if (pos && count != 1) {
                    tempState[i] = false;
                    changedState = true;
                } else if (!pos && (count == 1 || count == 2)) {
                    tempState[i] = true;
                    changedState = true;
                }

                x++;
                if (x == width) {
                    x = 0;
                    y++;
                }
            }

            Array.Copy(tempState, grid, grid.Length);
            return changedState;
        }
        private byte GetValue(int x, int y, int moveX, int moveY) {
            bool seat = false;
            if ((x += moveX) >= 0 && (y += moveY) >= 0 && x < width && y < height) {
                seat = grid[y * width + x];
            }
            return (byte)(seat ? 1 : 0);
        }
        private void Display() {
            int x = 0;
            int y = 0;
            for (int i = 0; i < grid.Length; i++) {
                Console.Write(!grid[i] ? '.' : '#');
                x++;
                if (x == width) {
                    x = 0;
                    y++;
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        private class Fold {
            public int Grid, Temp;
            public Fold Plus, Minus;
            public int MiddleUp { get { return (Grid & 128) >> 7; } }
            public int MiddleLeft { get { return (Grid & 2048) >> 11; } }
            public int MiddleRight { get { return (Grid & 8192) >> 13; } }
            public int MiddleDown { get { return (Grid & 131072) >> 17; } }
            public int Up { get { return (Grid & 1) + ((Grid & 2) >> 1) + ((Grid & 4) >> 2) + ((Grid & 8) >> 3) + ((Grid & 16) >> 4); } }
            public int Left { get { return (Grid & 1) + ((Grid & 32) >> 5) + ((Grid & 1024) >> 10) + ((Grid & 32768) >> 15) + ((Grid & 1048576) >> 20); } }
            public int Right { get { return ((Grid & 16) >> 4) + ((Grid & 512) >> 9) + ((Grid & 16384) >> 14) + ((Grid & 524288) >> 19) + ((Grid & 16777216) >> 24); } }
            public int Down { get { return ((Grid & 1048576) >> 20) + ((Grid & 2097152) >> 21) + ((Grid & 4194304) >> 22) + ((Grid & 8388608) >> 23) + ((Grid & 16777216) >> 24); } }
            public int this[int index] {
                get { return (Grid & (1 << index)) >> index; }
                set {
                    if (value == 1) {
                        Grid |= (1 << index);
                    } else {
                        Grid &= ~(1 << index);
                    }
                }
            }
            public void Display() {
                int x = 0;
                for (int i = 0; i < 25; i++) {
                    if (i == 12) {
                        Console.Write('?');
                    } else {
                        Console.Write(this[i] == 1 ? '#' : '.');
                    }

                    x++;
                    if (x == 5) {
                        x = 0;
                        Console.WriteLine();
                    }
                }
                Console.WriteLine();
                if (Plus != null) {
                    Plus.Display();
                }
            }
            public int CountFilled() {
                int count = BitOperations.PopCount((uint)Grid);
                Fold next = Plus;
                while (next != null) {
                    count += BitOperations.PopCount((uint)next.Grid);
                    next = next.Plus;
                }

                return count;
            }
            public bool Simulate(bool checkMinus = false) {
                Temp = Grid;

                if (checkMinus) {
                    CheckValue(Up);
                    CheckValue(Left);
                    CheckValue(Right);
                    CheckValue(Down);

                    if (Minus != null) {
                        return false;
                    }
                }

                for (int i = 0; i < 25; i++) {
                    if (i == 12) { continue; }

                    int pos = this[i];
                    int count = Count(i);

                    if (pos == 1 && count != 1) {
                        Temp &= ~(1 << i);
                    } else if (pos == 0 && (count == 1 || count == 2)) {
                        Temp |= 1 << i;
                    }
                }

                CheckMiddleValue(MiddleUp);
                CheckMiddleValue(MiddleLeft);
                CheckMiddleValue(MiddleRight);
                CheckMiddleValue(MiddleDown);

                if (Plus != null) {
                    Plus.Simulate();
                }

                Grid = Temp;
                return true;
            }
            private int Count(int index) {
                int total = 0;
                if (Minus != null) {
                    if (index < 5) {
                        total += Minus.MiddleUp;
                    } else if (index >= 20) {
                        total += Minus.MiddleDown;
                    }

                    if ((index % 5) == 0) {
                        total += Minus.MiddleLeft;
                    } else if (((index + 1) % 5) == 0) {
                        total += Minus.MiddleRight;
                    }
                }

                if (Plus != null) {
                    if (index == 7) {
                        total += Plus.Up;
                    } else if (index == 11) {
                        total += Plus.Left;
                    } else if (index == 13) {
                        total += Plus.Right;
                    } else if (index == 17) {
                        total += Plus.Down;
                    }
                }

                if (index >= 5) {
                    total += this[index - 5];
                }
                if (index < 20) {
                    total += this[index + 5];
                }
                if ((index % 5) != 0) {
                    total += this[index - 1];
                }
                if (((index + 1) % 5) != 0) {
                    total += this[index + 1];
                }
                return total;
            }
            private void CheckValue(int value) {
                if (Minus != null) { return; }

                if (value == 1 || value == 2) {
                    Minus = new Fold();
                    Minus.Plus = this;
                }
            }
            private void CheckMiddleValue(int value) {
                if (Plus != null) { return; }

                if (value == 1 || value == 2) {
                    Plus = new Fold();
                    Plus.Minus = this;
                }
            }
        }
    }
}