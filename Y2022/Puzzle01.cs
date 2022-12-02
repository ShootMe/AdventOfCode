using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Calorie Counting")]
    public class Puzzle01 : ASolver {
        private List<int> elves = new List<int>();

        public override void Setup() {
            foreach (string value in Input.Sections()) {
                int total = 0;
                foreach (string calories in value.Split('\n')) {
                    total += calories.ToInt();
                }
                elves.Add(total);
            }

            elves.Sort((left, right) => { return right.CompareTo(left); });
        }

        [Description("How many total Calories is that Elf carrying?")]
        public override string SolvePart1() {
            return $"{elves[0]}";
        }

        [Description("How many Calories are those Elves carrying in total?")]
        public override string SolvePart2() {
            return $"{elves[0] + elves[1] + elves[2]}";
        }
    }
}