using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2018 {
    [Description("Beverage Bandits")]
    public class Puzzle15 : ASolver {
        [Description("What is the outcome of the combat?")]
        public override string SolvePart1() {
            Area area = new Area(Tools.GetLines(Input), 3);
            while (area.Step()) {
                //Console.WriteLine($"Round {area.Rounds}");
                //Console.WriteLine(area);
            }
            //Console.WriteLine($"Round {area.Rounds}");
            //Console.WriteLine(area);
            return $"{area.Score()}";
        }

        [Description("What is the outcome of the combat with no Elves dying?")]
        public override string SolvePart2() {
            int elfAttack = 4;
            while (true) {
                Area area = new Area(Tools.GetLines(Input), elfAttack++);
                int startingElves = area.Elves;
                while (area.Step()) { }

                if (area.Elves == startingElves) {
                    return $"{area.Score()}";
                }
            }
        }

        private class Area {
            private bool[,] grid;
            private Unit[,] units;
            public int Elves, Goblins;
            private readonly HashSet<Point> expanded;
            private readonly Queue<Path> open;
            private readonly Point[] directions;
            public int Width, Height, Rounds;

            public Area(List<string> map, int elfAttack) {
                Rounds = 0;
                Width = map[0].Length;
                Height = map.Count;
                grid = new bool[Height, Width];
                units = new Unit[Height, Width];
                expanded = new HashSet<Point>();
                open = new Queue<Path>();
                directions = new Point[] { Point.NORTH, Point.WEST, Point.EAST, Point.SOUTH };

                for (int i = 0; i < Height; i++) {
                    string line = map[i];

                    for (int j = 0; j < Width; j++) {
                        char sq = line[j];

                        if (sq == '#') {
                            grid[i, j] = true;
                        } else {
                            grid[i, j] = false;
                        }

                        if (sq == 'G' || sq == 'E') {
                            units[i, j] = new Unit(200, sq == 'G' ? 3 : elfAttack, sq == 'G', j, i);
                            if (sq == 'G') { Goblins++; } else { Elves++; }
                        }
                    }
                }
            }

            public int Score() {
                int score = 0;

                for (int i = 0; i < Height; i++) {
                    for (int j = 0; j < Width; j++) {
                        Unit unit = units[i, j];
                        if (unit == null) { continue; }

                        score += unit.Health;
                    }
                }

                return score * (Rounds - 1);
            }
            public bool Step() {
                bool foundTarget = false;
                bool finishedEarly = false;
                Rounds++;

                for (int i = 0; i < Height; i++) {
                    for (int j = 0; j < Width; j++) {
                        Unit unit = units[i, j];
                        if (unit == null || unit.Rounds == Rounds) { continue; }
                        if (finishedEarly) { return false; }

                        unit.Target = null;
                        unit.Rounds = Rounds;
                        GetTarget(unit);

                        if (unit.Position != unit.TargetPosition) {
                            foundTarget = true;

                            if (unit.Target == null) {
                                units[unit.TargetPosition.Y, unit.TargetPosition.X] = unit;
                                units[unit.Position.Y, unit.Position.X] = null;
                                unit.Position = unit.TargetPosition;

                                GetTarget(unit);
                            }

                            if (unit.Target != null) {
                                unit.Target.Health -= unit.Attack;

                                if (unit.Target.Health < 0) {
                                    if (unit.Target.IsGoblin) { Goblins--; } else { Elves--; }

                                    units[unit.TargetPosition.Y, unit.TargetPosition.X] = null;
                                    if (Goblins == 0 || Elves == 0) {
                                        finishedEarly = true;
                                    }
                                }
                            }
                        }
                    }
                }

                return foundTarget;
            }
            private void GetTarget(Unit unit) {
                if (unit.Target != null) {
                    if (unit.Target.Health > 0) { return; }
                    unit.Target = null;
                }

                Point target = NextPosition(unit);
                unit.TargetPosition = target;
                if (target != unit.Position) {
                    unit.Target = units[target.Y, target.X];
                }
            }
            private Point NextPosition(Unit unit) {
                expanded.Clear();
                expanded.Add(unit.Position);
                open.Clear();
                Path current = new Path() { Position = unit.Position };
                AddNextDirections(current);

                Path best = null;
                int lowestHealth = int.MaxValue;

                while (open.Count > 0) {
                    current = open.Dequeue();

                    if (best != null && current.Steps > best.Steps) { return best.First(); }

                    Unit next = units[current.Position.Y, current.Position.X];
                    if (next != null) {
                        if (next.IsGoblin != unit.IsGoblin && (current.Steps > 1 || next.Health < lowestHealth)) {
                            if (best == null || (current.Steps == 1 && next.Health < lowestHealth) || best.Position.Y > current.Position.Y || (best.Position.Y == current.Position.Y && best.Position.X > current.Position.X)) {
                                best = current;
                                lowestHealth = next.Health;
                            }
                        }
                        continue;
                    }

                    AddNextDirections(current);
                }

                return best != null ? best.First() : unit.Position;
            }
            private void AddNextDirections(Path current) {
                foreach (Point dir in directions) {
                    Point test = dir + current.Position;
                    if (!grid[test.Y, test.X] && expanded.Add(test)) {
                        open.Enqueue(new Path() { Position = test, Previous = current, Steps = current.Steps + 1 });
                    }
                }
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Height; i++) {
                    for (int j = 0; j < Width; j++) {
                        if (grid[i, j]) {
                            sb.Append('#');
                        } else {
                            Unit unit = units[i, j];
                            if (unit != null) {
                                sb.Append(unit.IsGoblin ? 'G' : 'E');
                            } else {
                                sb.Append('.');
                            }
                        }
                    }

                    for (int j = 0; j < Width; j++) {
                        Unit unit = units[i, j];
                        if (unit != null) {
                            sb.Append($" {unit}");
                        }
                    }

                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }
        private class Unit {
            public int Health;
            public bool IsGoblin;
            public int Attack;
            public Point Position;
            public int Rounds;
            public Unit Target;
            public Point TargetPosition;
            public Unit(int health, int attack, bool isGoblin, int x, int y) {
                Health = health;
                Attack = attack;
                IsGoblin = isGoblin;
                Position = new Point() { X = x, Y = y };
                Rounds = 0;
            }

            public override string ToString() {
                char unitType = IsGoblin ? 'G' : 'E';
                return $"{unitType}({Health})";
            }
        }
        private class Path {
            public Point Position;
            public int Steps;
            public Path Previous;

            public Point First() {
                Path path = this;
                while (path.Previous != null && path.Steps > 1) {
                    path = path.Previous;
                }
                return path.Position;
            }
            public override string ToString() {
                return $"{Position}={Steps}";
            }
        }
    }
}