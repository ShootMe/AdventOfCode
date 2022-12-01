using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Four-Dimensional Adventure")]
    public class Puzzle25 : ASolver {
        private List<SpacePoint> points;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            points = new List<SpacePoint>();
            for (int i = 0; i < items.Count; i++) {
                points.Add(new SpacePoint(items[i]) { ID = i });
            }

            for (int i = 0; i < points.Count; i++) {
                SpacePoint point = points[i];

                for (int j = i + 1; j < points.Count; j++) {
                    SpacePoint other = points[j];

                    if (point.Parent.ID != other.Parent.ID && point.Connects(other)) {
                        if (point.Parent.ID < other.Parent.ID) {
                            SetParent(other.Parent, point.Parent);
                        } else {
                            SetParent(point.Parent, other.Parent);
                        }
                    }
                }
            }
        }
        private void SetParent(SpacePoint replace, SpacePoint parent) {
            for (int i = 0; i < points.Count; i++) {
                SpacePoint point = points[i];
                if (point.Parent.ID == replace.ID) {
                    point.Parent = parent;
                }
            }
        }

        [Description("How many constellations are formed by the fixed points in spacetime?")]
        public override string SolvePart1() {
            HashSet<int> distinctGroups = new HashSet<int>();
            for (int i = 0; i < points.Count; i++) {
                SpacePoint p = points[i];
                distinctGroups.Add(p.Parent.ID);
            }
            return $"{distinctGroups.Count}";
        }

        private class SpacePoint {
            public int X, Y, Z, T, ID;
            public SpacePoint Parent;

            public SpacePoint(string input) {
                int index = input.IndexOf(',');
                X = Tools.ParseInt(input, 0, index);
                int endIndex = input.IndexOf(',', index + 1);
                Y = Tools.ParseInt(input, index + 1, endIndex - index - 1);
                index = input.IndexOf(',', endIndex + 1);
                Z = Tools.ParseInt(input, endIndex + 1, index - endIndex - 1);
                T = Tools.ParseInt(input, index + 1);
                Parent = this;
            }
            public bool Connects(SpacePoint p) {
                return Math.Abs(p.X - X) + Math.Abs(p.Y - Y) + Math.Abs(p.Z - Z) + Math.Abs(p.T - T) <= 3;
            }
            public override string ToString() {
                return $"ID: {ID} Pos: {X},{Y},{Z},{T} Parent: {Parent?.ID}";
            }
        }
    }
}