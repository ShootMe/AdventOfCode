using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle05 : ASolver {
        private List<string> items;
        public Puzzle05(string input) : base(input) { Name = "Doesn't He Have Intern-Elves For This?"; }

        public override void Setup() {
            items = Tools.GetLines(Input);
        }

        [Description("How many strings are nice?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                bool has3Vowels = Has3Vowels(item);
                bool hasDouble = HasDouble(item);
                bool hasInvalid = HasInvalid(item);
                if (has3Vowels && hasDouble && !hasInvalid) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("How many strings are nice under these new rules?")]
        public override string SolvePart2() {
            int count = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                bool haspair = HasPair(item);
                bool hasRepeat = HasRepeat(item);
                if (haspair && hasRepeat) {
                    count++;
                }
            }
            return $"{count}";
        }

        public bool Has3Vowels(string item) {
            int count = 0;
            for (int i = 0; i < item.Length; i++) {
                switch (char.ToLower(item[i])) {
                    case 'a':
                    case 'e':
                    case 'i':
                    case 'o':
                    case 'u':
                        count++;
                        break;
                }
            }
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