using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Signals and Noise")]
    public class Puzzle06 : ASolver {
        private List<string> items;
        private int[] counts;

        public override void Setup() {
            items = Input.Lines();

            counts = new int[26 * items[0].Length];
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                for (int j = 0; j < item.Length; j++) {
                    counts[j * 26 + (char.ToUpper(item[j]) - 0x41)]++;
                }
            }
        }

        [Description("What is the error-corrected version of the message being sent?")]
        public override string SolvePart1() {
            char[] output = new char[items[0].Length];
            for (int i = 0; i < output.Length; i++) {
                int maxCount = 0;
                char best = ' ';
                for (int j = 0; j < 26; j++) {
                    int count = counts[i * 26 + j];
                    if (count > maxCount) {
                        maxCount = count;
                        best = (char)(j + 0x41);
                    }
                }

                output[i] = best;
            }
            return new string(output);
        }

        [Description("What is the original message that Santa is trying to send?")]
        public override string SolvePart2() {
            char[] output = new char[items[0].Length];
            for (int i = 0; i < output.Length; i++) {
                int minCount = int.MaxValue;
                char best = ' ';
                for (int j = 0; j < 26; j++) {
                    int count = counts[i * 26 + j];
                    if (count < minCount) {
                        minCount = count;
                        best = (char)(j + 0x41);
                    }
                }

                output[i] = best;
            }
            return new string(output);
        }
    }
}