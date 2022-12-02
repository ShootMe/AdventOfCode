using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Timing is Everything")]
    public class Puzzle15 : ASolver {
        private List<Disc> discs = new();

        public override void Setup() {
            int id = 1;
            foreach (string item in Input.Split('\n')) {
                string[] splits = item.SplitOn(" has ", " positions;", " at position ");
                int positions = splits[1].ToInt();
                int current = splits[3].ToInt();
                discs.Add(new Disc() { ID = id++, CurrentPosition = current, TotalPositions = positions });
            }
            discs.Add(new Disc() { ID = id, CurrentPosition = 0, TotalPositions = 11 });
        }

        [Description("What is the first time you can press the button to get a capsule?")]
        public override string SolvePart1() {
            int step = 1;
            long time = 0;
            int current = 0;
            while (current < discs.Count - 1) {
                if ((time + 1 + discs[current].CurrentPosition) % discs[current].TotalPositions == 0) {
                    step *= discs[current].TotalPositions;
                    time++;
                    current++;
                    continue;
                }

                time += step;
            }
            return $"{time - discs.Count + 1}";
        }

        [Description("What is the first time you can press the button to get another capsule?")]
        public override string SolvePart2() {
            int step = 1;
            long time = 0;
            int current = 0;
            while (current < discs.Count) {
                if ((time + 1 + discs[current].CurrentPosition) % discs[current].TotalPositions == 0) {
                    step *= discs[current].TotalPositions;
                    time++;
                    current++;
                    continue;
                }

                time += step;
            }
            return $"{time - discs.Count}";
        }

        private class Disc {
            public int ID;
            public int CurrentPosition;
            public int TotalPositions;

            public override string ToString() {
                return $"{ID} {TotalPositions} {CurrentPosition}";
            }
        }
    }
}