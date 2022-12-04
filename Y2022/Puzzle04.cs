using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Camp Cleanup")]
    public class Puzzle04 : ASolver {
        private List<(int elf1Start, int elf1End, int elf2Start, int elf2End)> ranges = new();
        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                string[] splits = line.SplitOn("-", ",", "-");
                ranges.Add((splits[0].ToInt(), splits[1].ToInt(), splits[2].ToInt(), splits[3].ToInt()));
            }
        }

        [Description("In how many assignment pairs does one range fully contain the other?")]
        public override string SolvePart1() {
            int total = 0;
            foreach ((int elf1Start, int elf1End, int elf2Start, int elf2End) in ranges) {
                if ((elf1Start <= elf2Start && elf1End >= elf2End) || (elf2Start <= elf1Start && elf2End >= elf1End)) {
                    total++;
                }
            }
            return $"{total}";
        }

        [Description("In how many assignment pairs do the ranges overlap?")]
        public override string SolvePart2() {
            int total = 0;
            foreach ((int elf1Start, int elf1End, int elf2Start, int elf2End) in ranges) {
                if (elf1Start <= elf2End && elf1End >= elf2Start) {
                    total++;
                }
            }
            return $"{total}";
        }
    }
}