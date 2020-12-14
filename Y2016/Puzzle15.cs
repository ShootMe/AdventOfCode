using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    public class Puzzle15 : ASolver {
        private Disc[] discs;
        public Puzzle15(string input) : base(input) { Name = "Timing is Everything"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            discs = new Disc[items.Count + 1];
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int positions = Tools.ParseInt(item, " has ", " positions;");
                int current = Tools.ParseInt(item, " at position ");
                discs[i] = new Disc() { ID = i + 1, CurrentPosition = current, TotalPositions = positions };
            }
            discs[^1] = new Disc() { ID = discs.Length, CurrentPosition = 0, TotalPositions = 11 };
        }

        [Description("What is the first time you can press the button to get a capsule?")]
        public override string SolvePart1() {
            int step = 1;
            long time = 0;
            int current = 0;
            while (current < discs.Length - 1) {
                if ((time + 1 + discs[current].CurrentPosition) % discs[current].TotalPositions == 0) {
                    step *= discs[current].TotalPositions;
                    time++;
                    current++;
                    continue;
                }

                time += step;
            }
            return $"{time - discs.Length + 1}";
        }

        [Description("What is the first time you can press the button to get another capsule?")]
        public override string SolvePart2() {
            int step = 1;
            long time = 0;
            int current = 0;
            while (current < discs.Length) {
                if ((time + 1 + discs[current].CurrentPosition) % discs[current].TotalPositions == 0) {
                    step *= discs[current].TotalPositions;
                    time++;
                    current++;
                    continue;
                }

                time += step;
            }
            return $"{time - discs.Length}";
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