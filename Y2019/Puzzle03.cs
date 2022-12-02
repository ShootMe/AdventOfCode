using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Crossed Wires")]
    public class Puzzle03 : ASolver {
        private List<Line> wire1 = new List<Line>();
        private List<Line> wire2 = new List<Line>();

        public override void Setup() {
            List<string> wires = Input.Lines();
            string line1 = wires[0];
            string line2 = wires[1];

            string[] directions = line1.Split(',');

            int x = 0;
            int y = 0;
            for (int i = 0; i < directions.Length; i++) {
                string code = directions[i];
                char dir = code[0];
                int length = code.Substring(1).ToInt();

                int xAdd = 0;
                int yAdd = 0;
                switch (dir) {
                    case 'U': yAdd = -length; break;
                    case 'D': yAdd = length; break;
                    case 'R': xAdd = length; break;
                    case 'L': xAdd = -length; break;
                }

                wire1.Add(new Line() {
                    StartPosition = new Point() { X = x, Y = y },
                    EndPosition = new Point() { X = x + xAdd, Y = y + yAdd }
                });

                x += xAdd;
                y += yAdd;
            }

            x = 0;
            y = 0;
            directions = line2.Split(',');
            for (int i = 0; i < directions.Length; i++) {
                string code = directions[i];
                char dir = code[0];
                int length = code.Substring(1).ToInt();

                int xAdd = 0;
                int yAdd = 0;
                switch (dir) {
                    case 'U': yAdd = -length; break;
                    case 'D': yAdd = length; break;
                    case 'R': xAdd = length; break;
                    case 'L': xAdd = -length; break;
                }

                wire2.Add(new Line() {
                    StartPosition = new Point() { X = x, Y = y },
                    EndPosition = new Point() { X = x + xAdd, Y = y + yAdd }
                });

                x += xAdd;
                y += yAdd;
            }
        }

        [Description("What is the distance from the central port to the closest intersection?")]
        public override string SolvePart1() {
            Point closest = new Point() { X = int.MaxValue, Y = 0 };
            for (int i = 0; i < wire1.Count; i++) {
                Line segment1 = wire1[i];

                for (int j = i == 0 ? 1 : 0; j < wire2.Count; j++) {
                    Line segment2 = wire2[j];

                    Point? cross = segment1.Overlaps(segment2);
                    if (cross.HasValue && cross.Value.Distance() < closest.Distance()) {
                        closest = cross.Value;
                    }
                }
            }
            return $"{closest.Distance()}";
        }

        [Description("What is the fewest steps the wires must take to reach an intersection?")]
        public override string SolvePart2() {
            List<int> crossTotals = new List<int>();

            int steps1 = 0;
            for (int i = 0; i < wire1.Count; i++) {
                Line segment1 = wire1[i];

                int steps2 = 0;
                for (int j = 0; j < wire2.Count; j++) {
                    Line segment2 = wire2[j];
                    if (i == 0) {
                        steps2 += segment2.Length();
                        continue;
                    }

                    Point? cross = segment1.Overlaps(segment2);
                    if (cross.HasValue) {
                        Line hit = new Line() {
                            StartPosition = segment1.StartPosition,
                            EndPosition = cross.Value
                        };
                        int steps1Add = hit.Length();

                        hit = new Line() {
                            StartPosition = segment2.StartPosition,
                            EndPosition = cross.Value
                        };
                        int steps2Add = hit.Length();

                        crossTotals.Add(steps1 + steps1Add + steps2 + steps2Add);
                        break;
                    }

                    steps2 += segment2.Length();
                }

                steps1 += segment1.Length();
            }

            crossTotals.Sort();

            return $"{crossTotals[0]}";
        }
    }
}