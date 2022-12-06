using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Tuning Trouble")]
    public class Puzzle06 : ASolver {
        [Description("How many characters need to be processed before the first start-of-packet marker is detected?")]
        public override string SolvePart1() {
            return CheckForMarker(Input, 4);
        }

        [Description("How many characters need to be processed before the first start-of-message marker is detected?")]
        public override string SolvePart2() {
            return CheckForMarker(Input, 14);
        }

        private static string CheckForMarker(string data, int length) {
            byte[] hits = new byte[26];
            int count = 0;
            length--;
            for (int i = 0; i < length; i++) {
                if ((hits[data[i] - 'a'] ^= 1) == 1) {
                    count++;
                } else {
                    count--;
                }
            }

            for (int i = length++; i < data.Length;) {
                if ((hits[data[i] - 'a'] ^= 1) == 1) {
                    count++;
                } else {
                    count--;
                }

                if (count == length) {
                    return $"{i + 1}";
                }

                if ((hits[data[++i - length] - 'a'] ^= 1) == 1) {
                    count++;
                } else {
                    count--;
                }
            }
            return string.Empty;
        }
    }
}