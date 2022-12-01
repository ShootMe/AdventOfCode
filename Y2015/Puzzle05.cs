using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace AdventOfCode.Y2015 {
    [Description("Doesn't He Have Intern-Elves For This?")]
    public class Puzzle05 : ASolver {
        [Description("How many strings are nice?")]
        public override string SolvePart1() {
            int count = Input.Slice('\n').Count(item => Has3Vowels(item) && HasDouble(item) && !HasInvalid(item));
            return $"{count}";
        }

        [Description("How many strings are nice under these new rules?")]
        public override string SolvePart2() {
            int count = Input.Slice('\n').Count(item => HasPair(item) && HasRepeat(item));
            return $"{count}";
        }

        public bool Has3Vowels(string item) {
            int count = item.Count(c => c switch { 'a' or 'e' or 'i' or 'o' or 'u' => true, _ => false });
            return count >= 3;
        }
        public bool HasDouble(string item) {
            char lastChar = char.ToLower(item[0]);
            for (int i = 1; i < item.Length; i++) {
                char c = char.ToLower(item[i]);
                if (lastChar == c) {
                    return true;
                }

                lastChar = c;
            }
            return false;
        }
        public bool HasInvalid(string item) {
            char lastChar = char.ToLower(item[0]);
            for (int i = 1; i < item.Length; i++) {
                char c = char.ToLower(item[i]);
                switch (lastChar) {
                    case 'a': if (c == 'b') { return true; } break;
                    case 'c': if (c == 'd') { return true; } break;
                    case 'p': if (c == 'q') { return true; } break;
                    case 'x': if (c == 'y') { return true; } break;
                }

                lastChar = c;
            }
            return false;
        }
        public bool HasPair(string item) {
            List<LetterPair> pairs = new List<LetterPair>();
            char lastChar = char.ToLower(item[0]);
            for (int i = 1; i < item.Length; i++) {
                char c = char.ToLower(item[i]);

                LetterPair pair = new LetterPair() { Index = i, Pair = $"{lastChar}{c}" };
                bool shouldAdd = true;
                for (int j = 0; j < pairs.Count; j++) {
                    LetterPair temp = pairs[j];
                    if (temp.Pair == pair.Pair) {
                        shouldAdd = false;
                        if (temp.Index + 1 != pair.Index) {
                            return true;
                        }
                    }
                }

                if (shouldAdd) {
                    pairs.Add(pair);
                }

                lastChar = c;
            }

            return false;
        }
        public bool HasRepeat(string item) {
            char lastChar = char.ToLower(item[0]);
            for (int i = 2; i < item.Length; i++) {
                char c = char.ToLower(item[i]);

                if (c == lastChar) {
                    return true;
                }
                lastChar = item[i - 1];
            }

            return false;
        }

        private class LetterPair {
            public int Index;
            public string Pair;
        }
    }
}