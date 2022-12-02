using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Rock Paper Scissors")]
    public class Puzzle02 : ASolver {
        [Description("What would your total score be if everything goes exactly according to your strategy guide?")]
        public override string SolvePart1() {
            Dictionary<string, int> score = new() {
                { "A X", 1 + 3 }, { "A Y", 2 + 6 }, { "A Z", 3 + 0 },
                { "B X", 1 + 0 }, { "B Y", 2 + 3 }, { "B Z", 3 + 6 },
                { "C X", 1 + 6 }, { "C Y", 2 + 0 }, { "C Z", 3 + 3 }
            };

            int total = 0;
            Input.Slice('\n', line => {
                total += score[line];
            });

            return $"{total}";
        }

        [Description("What would your total score be if everything goes exactly according to your strategy guide?")]
        public override string SolvePart2() {
            Dictionary<string, int> score = new() {
                { "A X", 3 + 0 }, { "A Y", 1 + 3 }, { "A Z", 2 + 6 },
                { "B X", 1 + 0 }, { "B Y", 2 + 3 }, { "B Z", 3 + 6 },
                { "C X", 2 + 0 }, { "C Y", 3 + 3 }, { "C Z", 1 + 6 }
            };

            int total = 0;
            Input.Slice('\n', line => {
                total += score[line];
            });

            return $"{total}";
        }
    }
}