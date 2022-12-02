using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Secure Container")]
    public class Puzzle04 : ASolver {
        [Description("How many different passwords in the range given meet these criteria?")]
        public override string SolvePart1() {
            int start = Input.Substring(0, 6).ToInt();
            int end = Input.Substring(7, 6).ToInt();
            int count = 0;

            for (int i = start; i <= end; i++) {
                if (MatchesCritera(i, false)) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("How many different passwords in the range meet all of the criteria?")]
        public override string SolvePart2() {
            int start = Input.Substring(0, 6).ToInt();
            int end = Input.Substring(7, 6).ToInt();
            int count = 0;

            for (int i = start; i <= end; i++) {
                if (MatchesCritera(i, true)) {
                    count++;
                }
            }
            return $"{count}";
        }

        private bool MatchesCritera(int value, bool extra) {
            int mod = value % 10;
            value /= 10;
            bool isDouble = false;
            bool wasDouble = false;
            bool hasDouble = false;
            bool anyDouble = false;
            while (value > 0) {
                int next = value % 10;
                if (next > mod) {
                    return false;
                } else if (next == mod) {
                    isDouble = !wasDouble;
                    wasDouble = true;
                    anyDouble = true;
                } else {
                    if (isDouble) {
                        hasDouble = true;
                    }
                    wasDouble = false;
                }
                mod = next;
                value /= 10;
            }
            if (isDouble) {
                hasDouble = true;
            }

            return extra ? hasDouble : anyDouble;
        }
    }
}