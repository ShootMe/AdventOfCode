using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Conway Cubes")]
    public class Puzzle17 : ASolver {
        [Description("How many cubes are left in the active state after the sixth cycle?")]
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

        [Description("How many cubes are left in the active state after the sixth cycle?")]
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
                List<string> items = input.Lines();
                Width = items[0].Length + 12;
                Height = items.Count + 12;
                Depth = 13;
                Hyper = is3d ? 1 : 13;
                Grid = new bool[Width * Height * Depth * Hyper];
                State = new bool[Grid.Length];
                int x = Width / 2 - items[0].Length / 2;
                int y = Height / 2 - items.Count / 2;
                int z = (Depth / 2) * Height * Width;
                int w = (Hyper / 2) * Depth * Height * Width;

                for (int i = 0; i < items.Count; i++) {
                    string item = items[i];

                    for (int j = 0; j < item.Length; j++) {
                        Grid[w + z + y * Width + x] = item[j] == '#';
                        x++;
                    }
                    y++;
                    x = Width / 2 - item.Length / 2;
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

                    int count = CountNeighbors(x, y, z, w) - (active ? 1 : 0);

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
                int xe = x + 1 >= Width ? x + 1 : x + 2;
                int ye = y + 1 >= Height ? y + 1 : y + 2;
                int ze = z + 1 >= Depth ? z + 1 : z + 2;
                int we = w + 1 >= Hyper ? w + 1 : w + 2;
                int xi = x == 0 ? x : x - 1;
                int yi = y == 0 ? y : y - 1;
                int zi = z == 0 ? z : z - 1;
                int ws = w == 0 ? w : w - 1;
                int count = 0;
                while (ws < we) {
                    int wo = ws * Depth * Height * Width;
                    int zs = zi;

                    while (zs < ze) {
                        int zo = zs * Height * Width;
                        int ys = yi;

                        while (ys < ye) {
                            int index = ys * Height + wo + zo;
                            int xs = xi;
                            count += Grid[index + xs++] ? 1 : 0;
                            count += xs < xe && Grid[index + xs++] ? 1 : 0;
                            count += xs < xe && Grid[index + xs] ? 1 : 0;
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