using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    public class Puzzle15 : ASolver {
        public Puzzle15(string input) : base(input) { Name = "Rambunctious Recitation"; }

        [Description("What will be the 2020th number spoken?")]
        public override string SolvePart1() {
            return $"{GetLastSpoken(2020)}";
        }

        [Description("What will be the 30000000th number spoken?")]
        public override string SolvePart2() {
            return $"{GetLastSpoken(30000000)}";
        }

        private int GetLastSpoken(int amount) {
            int[] said = new int[amount];
            Array.Fill(said, -1);

            string[] numbers = Input.Split(',');

            int last = 0;
            for (int i = 0; i < numbers.Length; i++) {
                last = Tools.ParseInt(numbers[i]);
                if (i + 1 < numbers.Length) {
                    said[last] = i + 1;
                }
            }

            int turn = numbers.Length;
            while (turn < amount) {
                int lastUsed = said[last];
                said[last] = turn;
                last = lastUsed < 0 ? 0 : turn - lastUsed;
                turn++;
            }
            return last;
        }
    }
}