using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("No Time for a Taxicab")]
    public class Puzzle01 : ASolver {
        [Description("How many blocks away is Easter Bunny HQ?")]
        public override string SolvePart1() {
            string[] directions = Input.Split(", ");
            int x = 0;
            int y = 0;
            int angle = 0;
            for (int i = 0; i < directions.Length; i++) {
                string dir = directions[i];
                char d = dir[0];
                int length = Tools.ParseInt(dir.Substring(1));
                switch (d) {
                    case 'L': angle -= 90; break;
                    case 'R': angle += 90; break;
                }
                if (angle < 0) { angle += 360; }
                if (angle == 360) { angle = 0; }

                switch (angle) {
                    case 0: y -= length; break;
                    case 90: x += length; break;
                    case 180: y += length; break;
                    case 270: x -= length; break;
                }
            }

            return $"{Math.Abs(x) + Math.Abs(y)}";
        }

        [Description("How many blocks away is the first location you visit twice?")]
        public override string SolvePart2() {
            string[] directions = Input.Split(", ");
            int x = 0;
            int y = 0;
            int angle = 0;
            List<Line> lines = new List<Line>();

            for (int i = 0; i < directions.Length; i++) {
                string dir = directions[i];
                char d = dir[0];
                int length = Tools.ParseInt(dir.Substring(1));
                switch (d) {
                    case 'L': angle -= 90; break;
                    case 'R': angle += 90; break;
                }
                if (angle < 0) { angle += 360; }
                if (angle == 360) { angle = 0; }

                Line line = new Line() {
                    StartPosition = new Point() { X = x, Y = y },
                };

                switch (angle) {
                    case 0: y -= length; break;
                    case 90: x += length; break;
                    case 180: y += length; break;
                    case 270: x -= length; break;
                }
                line.EndPosition = new Point() { X = x, Y = y };

                for (int j = lines.Count - 2; j >= 0; j--) {
                    Line existing = lines[j];
                    Point? overlap = existing.Overlaps(line);
                    if (overlap.HasValue) {
                        return $"{overlap.Value.Distance()}";
                    }
                }
                lines.Add(line);
            }

            return string.Empty;
        }
    }
}