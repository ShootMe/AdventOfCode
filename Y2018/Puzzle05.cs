using AdventOfCode.Core;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2018 {
    [Description("Alchemical Reduction")]
    public class Puzzle05 : ASolver {
        [Description("How many units remain after fully reacting the polymer you scanned?")]
        public override string SolvePart1() {
            return $"{Reduce(Input)}";
        }

        [Description("What is the length of the shortest polymer you can find?")]
        public override string SolvePart2() {
            int minLength = int.MaxValue;
            for (int i = 0; i < 26; i++) {
                string chain = Input.Replace((char)(i + 0x41), (char)0).Replace((char)(i + 0x61), (char)0);
                int length = Reduce(chain);
                if (length < minLength) {
                    minLength = length;
                }
            }
            return $"{minLength}";
        }

        private int Reduce(string input) {
            int length = 0;
            StringBuilder chain = new StringBuilder(input);
            for (int j = chain.Length - 1; j >= 0; j--) {
                if (chain[j] != (char)0) {
                    length++;
                }
            }
            int i = 0;
            char last;
            int lastIndex = 0;
            while ((last = chain[i++]) == (char)0) {
                lastIndex++;
            }

            for (; i < chain.Length; i++) {
                char opposite = (char)((byte)last ^ 0x20);
                char current = chain[i];
                if (current == (char)0) { continue; }

                if (current != opposite) {
                    last = current;
                    lastIndex = i;
                } else {
                    length -= 2;
                    chain[lastIndex] = (char)0;
                    chain[i] = (char)0;
                    while (lastIndex > 0 && (last = chain[--lastIndex]) == (char)0) { }
                    while (lastIndex < chain.Length && (last = chain[lastIndex]) == (char)0) {
                        lastIndex++;
                    }
                    if (lastIndex > i) {
                        i = lastIndex;
                    }
                }
            }

            return length;
        }
    }
}