using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    public class Puzzle17 : ASolver {
        public Puzzle17(string input) : base(input) { Name = "Conway Cubes"; }

        [Description("What is the answer?")]
        public override string SolvePart1() {
            Cube cube = new Cube(Input);
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            return $"{cube.ActiveCount()}";
        }

        [Description("What is the answer?")]
        public override string SolvePart2() {
            Cube cube = new Cube(Input, false);
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            cube.Cycle();
            return $"{cube.ActiveCount()}";
        }

        private class Cube {
            public int Width;
            public int Height;
            public int Depth;
            public int Hyper;
            public bool[] Grid;
            private bool[] State;

            public Cube(string input, bool is3d = true) {
                List<string> items = Tools.GetLines(input);
                Width = (items[0].Length + 17);
                Height = items.Count + 17;
                Depth = Width;
                Hyper = is3d ? 1 : Width;
                Grid = new bool[Width * Height * Depth * Hyper];
                State = new bool[Grid.Length];
                int x = Width / 2;
                int y = Height / 2;
                int z = (Depth / 2) * Height * Width;
                int w = (Hyper / 2) * Depth * Height * Width;

                for (int i = 0; i < items.Count; i++) {
                    string item = items[i];

                    for (int j = 0; j < item.Length; j++) {
                        Grid[w + z + y * Width + x] = item[j] == '#';
                        x++;
                    }
                    y++;
                    x = Width / 2;
                }
            }

            public int ActiveCount() {
                int count = 0;
                for (int i = 0; i < Grid.Length; i++) {
                    if (Grid[i]) {
                        count++;
                    }
                }
                return count;
            }
            public void Cycle() {
                int x = 0;
                int y = 0;
                int z = 0;
                int w = 0;
                for (int i = 0; i < Grid.Length; i++) {
                    bool active = Grid[i];

                    int count = CountNeighbors(x, y, z, w);

                    if (active && (count < 2 || count > 3)) {
                        State[i] = false;
                    } else if (!active && count == 3) {
                        State[i] = true;
                    } else {
                        State[i] = active;
                    }

                    x++;
                    if (x == Width) {
                        x = 0;
                        y++;
                        if (y == Height) {
                            y = 0;
                            z++;
                            if (z == Depth) {
                                z = 0;
                                w++;
                            }
                        }
                    }
                }

                bool[] temp = Grid;
                Grid = State;
                State = temp;
            }
            private int CountNeighbors(int x, int y, int z, int w) {
                int xe = x + 2;
                int ye = y + 2;
                int ze = z + 2;
                int we = w + 2;
                int ws = w - 1;
                int count = 0;
                while (ws < we) {
                    if (ws < 0 || ws >= Hyper) { ws++; continue; }
                    int wo = ws * Depth * Height * Width;
                    int zs = z - 1;

                    while (zs < ze) {
                        if (zs < 0 || zs >= Depth) { zs++; continue; }
                        int zo = zs * Height * Width;
                        int ys = y - 1;

                        while (ys < ye) {
                            if (ys < 0 || ys >= Height) { ys++; continue; }
                            int yo = ys * Height;
                            int xs = x - 1;

                            while (xs < xe) {
                                if (xs < 0 || xs >= Width) { xs++; continue; }
                                if (ws == w && zs == z && ys == y && xs == x) { xs++; continue; }

                                count += Grid[wo + zo + yo + xs] ? 1 : 0;
                                xs++;
                            }

                            ys++;
                        }

                        zs++;
                    }

                    ws++;
                }

                return count;
            }
        }
    }
}