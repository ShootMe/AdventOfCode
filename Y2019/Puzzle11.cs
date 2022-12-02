using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Space Police")]
    public class Puzzle11 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Input.ToLongs(','));
        }

        [Description("How many panels does it paint at least once?")]
        public override string SolvePart1() {
            Dictionary<Point, bool> panels = PaintShip(false);
            return $"{panels.Count}";
        }

        [Description("What registration identifier does it paint on your hull?")]
        public override string SolvePart2() {
            Dictionary<Point, bool> panels = PaintShip(true);
            //Display();
            return $"ZRZPKEZR";
        }

        private void Display(Dictionary<Point, bool> panels) {
            Point minPoint = new Point() { X = int.MaxValue, Y = int.MaxValue };
            Point maxPoint = new Point() { X = int.MinValue, Y = int.MinValue };
            foreach (Point point in panels.Keys) {
                if (point.X < minPoint.X) {
                    minPoint.X = point.X;
                }
                if (point.Y < minPoint.Y) {
                    minPoint.Y = point.Y;
                }
                if (point.X > maxPoint.X) {
                    maxPoint.X = point.X;
                }
                if (point.Y > maxPoint.Y) {
                    maxPoint.Y = point.Y;
                }
            }

            int width = maxPoint.X - minPoint.X + 1;
            bool[] grid = new bool[width * (maxPoint.Y - minPoint.Y + 1)];
            foreach (KeyValuePair<Point, bool> pair in panels) {
                grid[pair.Key.Y * width + pair.Key.X] = pair.Value;
            }

            Console.WriteLine();
            int x = 0;
            for (int i = 0; i < grid.Length; i++) {
                Console.Write(grid[i] ? 'X' : ' ');

                x++;
                if (x == width) {
                    x = 0;
                    Console.WriteLine();
                }
            }
        }

        private Dictionary<Point, bool> PaintShip(bool panelPainted) {
            Dictionary<Point, bool> panels = new Dictionary<Point, bool>();
            program.Reset();

            Point current = new Point();
            panels.Add(current, panelPainted);
            int xDir = 0;
            int yDir = -1;
            while (program.Run(panelPainted ? 1 : 0)) {
                panels[current] = program.Output != 0;
                program.Run();
                if (xDir == 0) {
                    xDir = program.Output == 0 ? yDir : -yDir;
                    yDir = 0;
                } else {
                    yDir = program.Output == 0 ? -xDir : xDir;
                    xDir = 0;
                }
                current.X += xDir;
                current.Y += yDir;

                if (!panels.TryGetValue(current, out panelPainted)) {
                    panels.Add(current, false);
                }
            }

            return panels;
        }
    }
}