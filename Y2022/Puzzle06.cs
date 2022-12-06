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
            char[] list = new char[length--];
            for (int i = 0; i < length; i++) {
                list[i] = data[i];
            }

            int index = length;
            for (int i = length; i < data.Length; i++) {
                list[index++] = data[i];
                if (index > length) { index = 0; }

                bool good = true;
                for (int j = 0; j <= length; j++) {
                    for (int k = j + 1; k <= length; k++) {
                        if (list[j] == list[k]) {
                            good = false;
                            break;
                        }
                    }
                }

                if (good) {
                    return $"{i + 1}";
                }
            }
            return string.Empty;
        }
    }
}