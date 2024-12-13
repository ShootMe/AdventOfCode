using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Claw Contraption")]
    public class Puzzle13 : ASolver {
        [Description("What is the fewest tokens you would have to spend to win all possible prizes?")]
        public override string SolvePart1() {
            return $"{FindTotalTokens()}";
        }

        [Description("What is the fewest tokens you would have to spend to win all possible prizes?")]
        public override string SolvePart2() {
            return $"{FindTotalTokens(10000000000000)}";
        }
        private long FindTotalTokens(long additional = 0) {
            string[] lines = Input.Split('\n');
            long total = 0;
            for (int i = 0; i < lines.Length; i++) {
                string[] buttonA = lines[i++].SplitOn("X+", ", Y+");
                string[] buttonB = lines[i++].SplitOn("X+", ", Y+");
                string[] prize = lines[i++].SplitOn("X=", ", Y=");
                long xA = buttonA[1].ToLong(); long yA = buttonA[2].ToLong();
                long xB = buttonB[1].ToLong(); long yB = buttonB[2].ToLong();
                long pX = additional + prize[1].ToLong(); long pY = additional + prize[2].ToLong();

                long denominator = xA * yB - yA * xB;
                if (denominator != 0) {
                    long bB = (xA * pY - yA * pX) / denominator;
                    long bA = (yB * pX - xB * pY) / denominator;
                    if (xA * bA + xB * bB == pX) {
                        total += bA * 3 + bB;
                    }
                }
            }
            return total;
        }
    }
}