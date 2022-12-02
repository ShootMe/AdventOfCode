using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Bathroom Security")]
    public class Puzzle02 : ASolver {
        private List<string> items;

        public override void Setup() {
            items = Input.Lines();
        }

        [Description("What is the bathroom code?")]
        public override string SolvePart1() {
            int[] nextUp = { 0, 1, 2, 3, 1, 2, 3, 4, 5, 6 };
            int[] nextDown = { 0, 4, 5, 6, 7, 8, 9, 7, 8, 9 };
            int[] nextLeft = { 0, 1, 1, 2, 4, 4, 5, 7, 7, 8 };
            int[] nextRight = { 0, 2, 3, 3, 5, 6, 6, 8, 9, 9 };

            string solution = string.Empty;
            int pos = 5;
            for (int i = 0; i < items.Count; i++) {
                string code = items[i];
                for (int j = 0; j < code.Length; j++) {
                    char dir = code[j];
                    switch (dir) {
                        case 'U': pos = nextUp[pos]; break;
                        case 'D': pos = nextDown[pos]; break;
                        case 'L': pos = nextLeft[pos]; break;
                        case 'R': pos = nextRight[pos]; break;
                    }
                }

                solution += pos.ToString();
            }
            return $"{solution}";
        }

        [Description("What is the correct bathroom code?")]
        public override string SolvePart2() {
            int[] nextUp = { 0, 1, 2, 1, 4, 5, 2, 3, 4, 9, 6, 7, 8, 11 };
            int[] nextDown = { 0, 3, 6, 7, 8, 5, 10, 11, 12, 9, 10, 13, 12, 13 };
            int[] nextLeft = { 0, 1, 2, 2, 3, 5, 5, 6, 7, 8, 10, 10, 11, 13 };
            int[] nextRight = { 0, 1, 3, 4, 4, 6, 7, 8, 9, 9, 11, 12, 12, 13 };

            string solution = string.Empty;
            int pos = 5;
            for (int i = 0; i < items.Count; i++) {
                string code = items[i];
                for (int j = 0; j < code.Length; j++) {
                    char dir = code[j];
                    switch (dir) {
                        case 'U': pos = nextUp[pos]; break;
                        case 'D': pos = nextDown[pos]; break;
                        case 'L': pos = nextLeft[pos]; break;
                        case 'R': pos = nextRight[pos]; break;
                    }
                }

                solution += pos.ToString("X");
            }
            return $"{solution}";
        }
    }
}