using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Blizzard Basin")]
    public class Puzzle24 : ASolver {
        private Area area;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            area = new Area(lines[0].Length - 2, lines.Length - 2);
            for (int y = 1; y < lines.Length - 1; y++) {
                string line = lines[y];
                for (int x = 1; x < line.Length - 1; x++) {
                    char c = line[x];
                    if (c != '.') {
                        area.AddBlizzard(x - 1, y - 1, c);
                    }
                }
            }
        }

        [Description("What is the fewest number of minutes required to avoid the blizzards and reach the goal?")]
        public override string SolvePart1() {
            return $"{FindPath(0, -1, area.Width - 1, area.Height)}";
        }

        [Description("What is the fewest number of minutes required to reach the goal, go back to the start, then reach the goal again?")]
        public override string SolvePart2() {
            FindPath(area.Width - 1, area.Height, 0, -1);
            return $"{FindPath(0, -1, area.Width - 1, area.Height)}";
        }

        private int FindPath(int startX, int startY, int endX, int endY) {
            List<(int, int)> directions = new List<(int, int)>() { (0, 1), (1, 0), (0, -1), (-1, 0), (0, 0) };
            HashSet<Person> closed = new();
            Queue<Person> open = new();
            open.Enqueue(new Person() { X = startX, Y = startY, Rounds = area.Rounds });
            while (open.Count > 0) {
                Person person = open.Dequeue();

                while (area.Rounds < person.Rounds + 1) {
                    area.Step();
                }
                if (person.X == endX && person.Y == endY) {
                    return person.Rounds;
                }

                for (int i = 0; i < directions.Count; i++) {
                    (int x, int y) = directions[i];
                    int nx = person.X + x; int ny = person.Y + y;
                    if ((nx >= 0 && nx < area.Width && ny >= 0 && ny < area.Height) || (ny == endY && nx == endX) || (ny == startY && nx == startX)) {
                        Person newPerson = new Person() { X = nx, Y = ny, Rounds = area.Rounds };
                        if (area.IsValidPosition(newPerson.X, newPerson.Y) && closed.Add(newPerson)) {
                            open.Enqueue(newPerson);
                        }
                    }
                }
            }
            return 0;
        }
        private struct Person {
            public int X, Y, Rounds;
            public override int GetHashCode() {
                return X + Y * 31 + Rounds * 67;
            }
            public override bool Equals(object obj) {
                return obj is Person person && person.X == X && person.Y == Y && person.Rounds == Rounds;
            }
        }
        private class Area {
            public const byte UP = 1; public const byte RIGHT = 2;
            public const byte DOWN = 4; public const byte LEFT = 8;
            public int Width, Height;
            public byte[,] Blizzards;
            public int Rounds;
            public Area(int width, int height) {
                Width = width;
                Height = height;

                Blizzards = new byte[Height, Width];
            }
            public void AddBlizzard(int x, int y, char type) {
                Blizzards[y, x] = type switch { '^' => UP, '>' => RIGHT, 'v' => DOWN, _ => LEFT };
            }
            public bool IsValidPosition(int x, int y) {
                return x < 0 || y < 0 || x >= Width || y >= Height || Blizzards[y, x] == 0;
            }
            public void Step() {
                Rounds++;
                for (int y = 0; y < Height; y++) {
                    int yU = (y + Height - 1) % Height;
                    int yD = (y + 1) % Height;

                    for (int x = 0; x < Width; x++) {
                        byte value = Blizzards[y, x];
                        if ((value & UP) != 0) { Blizzards[yU, x] |= UP << 4; }
                        if ((value & RIGHT) != 0) { Blizzards[y, (x + 1) % Width] |= RIGHT << 4; }
                        if ((value & DOWN) != 0) { Blizzards[yD, x] |= DOWN << 4; }
                        if ((value & LEFT) != 0) { Blizzards[y, (x + Width - 1) % Width] |= LEFT << 4; }
                    }
                }
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        Blizzards[y, x] >>= 4;
                    }
                }
            }
        }
    }
}