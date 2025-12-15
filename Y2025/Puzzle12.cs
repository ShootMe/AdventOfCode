using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Christmas Tree Farm")]
    public class Puzzle12 : ASolver {
        private List<Present> presents = new();
        private List<Tree> trees = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            Present present = null;
            int presentIndex = 0;
            foreach (string line in lines) {
                if (string.IsNullOrEmpty(line)) { continue; }

                int colIndex = line.IndexOf(':');
                int xIndex = line.IndexOf('x');
                if (xIndex > 0) {
                    string[] splits = line.SplitOn("x", ": ");
                    Tree t = new Tree();
                    t.Width = splits[0].ToInt();
                    t.Height = splits[1].ToInt();
                    splits = splits[2].Split(' ');
                    t.PresentCounts = new int[splits.Length];
                    for (int i = 0; i < splits.Length; i++) {
                        t.PresentCounts[i] = splits[i].ToInt();
                    }
                    trees.Add(t);
                } else if (colIndex > 0) {
                    present?.SetArea();
                    present = new Present(line.ToInt());
                    presentIndex = 0;
                    presents.Add(present);
                } else {
                    for (int i = 0; i < 3; i++) {
                        present.Layout[presentIndex++] = line[i] == '#';
                    }
                }
            }
            present?.SetArea();
        }

        [Description("How many of the regions can fit all of the presents listed?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < trees.Count; i++) {
                Tree t = trees[i];
                int area = 0;
                for (int j = 0; j < t.PresentCounts.Length; j++) {
                    int count = t.PresentCounts[j];
                    if (count > 0) {
                        area += presents[j].Area * count;
                    }
                }
                if (t.Width * t.Height >= area) {
                    total++;
                }
            }
            return $"{total}";
        }

        private class Present {
            public int ID;
            public int Area;
            public bool[] Layout = new bool[9];
            public Present(int id) { ID = id; Area = 0; }
            public void SetArea() {
                for (int i = 0; i < Layout.Length; i++) {
                    if (Layout[i]) { Area++; }
                }
            }
        }
        private class Tree {
            public int Width, Height;
            public int[] PresentCounts;
        }
    }
}