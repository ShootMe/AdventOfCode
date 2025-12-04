using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Printing Department")]
    public class Puzzle04 : ASolver {
        private bool[,] floor;
        private int width, height;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            width = lines[0].Length;
            height = lines.Length;
            floor = new bool[height + 2, width + 2];
            for (int y = 1; y <= height; y++) {
                string line = lines[y - 1];
                for (int x = 1; x <= width; x++) {
                    floor[y, x] = line[x - 1] == '@';
                }
            }
        }

        [Description("How many rolls of paper can be accessed by a forklift?")]
        public override string SolvePart1() {
            return $"{RemoveRolls(false)}";
        }

        [Description("How many rolls of paper in total can be removed by the Elves and their forklifts?")]
        public override string SolvePart2() {
            int total = 0;
            while (true) {
                int rolls = RemoveRolls(true);
                total += rolls;
                if (rolls == 0) { break; }
            }
            return $"{total}";
        }

        private int RemoveRolls(bool remove) {
            int rolls = 0;
            List<(int, int)> toRemove = new();
            for (int y = 1; y <= height; y++) {
                for (int x = 1; x <= width; x++) {
                    if (!floor[y, x]) { continue; }

                    int count = 0;
                    count += floor[y - 1, x - 1] ? 1 : 0;
                    count += floor[y - 1, x] ? 1 : 0;
                    count += floor[y - 1, x + 1] ? 1 : 0;
                    count += floor[y, x - 1] ? 1 : 0;
                    count += floor[y, x + 1] ? 1 : 0;
                    count += floor[y + 1, x - 1] ? 1 : 0;
                    count += floor[y + 1, x] ? 1 : 0;
                    count += floor[y + 1, x + 1] ? 1 : 0;
                    if (count < 4) {
                        rolls++;
                        if (remove) { toRemove.Add((y, x)); }
                    }
                }
            }
            for (int i = 0; i < toRemove.Count; i++) {
                (int y, int x) = toRemove[i];
                floor[y, x] = false;
            }
            return rolls;
        }
    }
}