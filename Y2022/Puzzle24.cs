using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Blizzard Basin")]
    public class Puzzle24 : ASolver {
        private Area area = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            area.Width = lines[0].Length - 2;
            area.Height = lines.Length - 2;
            for (int y = 1; y < lines.Length - 1; y++) {
                string line = lines[y];
                for (int x = 1; x < line.Length - 1; x++) {
                    char c = line[x];
                    if (c != '.') {
                        area.Blizzards.Add(new Blizzard() { X = x - 1, Y = y - 1, Dir = c switch { '^' => 0, '>' => 1, 'v' => 2, _ => 3 } });
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

                while (area.Rounds > person.Rounds) {
                    area.Reverse();
                }
                while (area.Rounds < person.Rounds) {
                    area.Step();
                }
                if (person.X == endX && person.Y == endY) {
                    return person.Rounds;
                }
                area.Step();

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
            public int Width, Height;
            public List<Blizzard> Blizzards = new();
            public int Rounds;
            public bool IsValidPosition(int x, int y) {
                for (int i = 0; i < Blizzards.Count; i++) {
                    Blizzard blizzard = Blizzards[i];
                    if (blizzard.X == x && blizzard.Y == y) {
                        return false;
                    }
                }
                return true;
            }
            public void Step() {
                Rounds++;
                for (int i = 0; i < Blizzards.Count; i++) {
                    Blizzard blizzard = Blizzards[i];
                    blizzard.Step(Width, Height);
                }
            }
            public void Reverse() {
                Rounds--;
                for (int i = 0; i < Blizzards.Count; i++) {
                    Blizzard blizzard = Blizzards[i];
                    blizzard.Reverse(Width, Height);
                }
            }
        }
        private class Blizzard {
            public int X, Y, Dir;
            public void Step(int width, int height) {
                X += Dir switch { 0 => 0, 1 => 1, 2 => 0, _ => -1 };
                Y += Dir switch { 0 => -1, 1 => 0, 2 => 1, _ => 0 };
                CheckBounds(width, height);
            }
            public void Reverse(int width, int height) {
                X -= Dir switch { 0 => 0, 1 => 1, 2 => 0, _ => -1 };
                Y -= Dir switch { 0 => -1, 1 => 0, 2 => 1, _ => 0 };
                CheckBounds(width, height);
            }
            private void CheckBounds(int width, int height) {
                if (X < 0) { X = width - 1; }
                if (X >= width) { X = 0; }
                if (Y < 0) { Y = height - 1; }
                if (Y >= height) { Y = 0; }
            }
        }
    }
}