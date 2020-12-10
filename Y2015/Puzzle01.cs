using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle01 : ASolver {
        public Puzzle01(string input) : base(input) { Name = "Not Quite Lisp"; }

        [Description("To what floor do the instructions take Santa?")]
        public override string SolvePart1() {
            int floor = 0;
            for (int i = 0; i < Input.Length; i++) {
                switch (Input[i]) {
                    case '(': floor++; break;
                    case ')': floor--; break;
                }
            }

            return $"{floor}";
        }

        [Description("What is the position of the character that causes Santa to first enter the basement?")]
        public override string SolvePart2() {
            int floor = 0;
            for (int i = 0; i < Input.Length; i++) {
                switch (Input[i]) {
                    case '(': floor++; break;
                    case ')': floor--; break;
                }
                if (floor < 0) {
                    return $"{i + 1}";
                }
            }

            return string.Empty;
        }
    }
}