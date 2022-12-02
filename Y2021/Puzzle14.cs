using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Extended Polymerization")]
    public class Puzzle14 : ASolver {
        private Dictionary<(char, char), char> rules;
        private Dictionary<(char, char), long> pairs;
        private string template;
        public override void Setup() {
            List<string> items = Input.Lines();

            pairs = new Dictionary<(char, char), long>();
            template = items[0];
            for (int i = 1; i < template.Length; i++) {
                (char a, char b) pair = (template[i - 1], template[i]);
                if (pairs.TryGetValue(pair, out long count)) {
                    pairs[pair]++;
                } else {
                    pairs[pair] = 1;
                }
            }

            rules = new Dictionary<(char, char), char>();
            for (int i = 2; i < items.Count; i++) {
                string item = items[i];
                rules.Add((item[0], item[1]), item[6]);
            }
        }

        [Description("What do you get if you take the most common element and subtract the least common?")]
        public override string SolvePart1() {
            return $"{RunFor(10)}";
        }

        [Description("What do you get if you take the most common element and subtract the least common?")]
        public override string SolvePart2() {
            return $"{RunFor(30)}";
        }

        private long RunFor(int steps) {
            Dictionary<(char, char), long> newPairs = new Dictionary<(char, char), long>();
            for (int i = 0; i < steps; i++) {
                (char, char)[] currentPairs = new (char, char)[pairs.Count];
                pairs.Keys.CopyTo(currentPairs, 0);

                newPairs.Clear();
                for (int j = 0; j < currentPairs.Length; j++) {
                    (char a, char b) current = currentPairs[j];
                    char rule = rules[current];
                    long value = pairs[current];
                    (char a, char b) left = (current.a, rule);
                    if (newPairs.TryGetValue(left, out long count)) {
                        newPairs[left] += value;
                    } else {
                        newPairs[left] = value;
                    }

                    (char a, char b) right = (rule, current.b);
                    if (newPairs.TryGetValue(right, out count)) {
                        newPairs[right] += value;
                    } else {
                        newPairs[right] = value;
                    }
                }

                Dictionary<(char, char), long> temp = pairs;
                pairs = newPairs;
                newPairs = temp;
            }

            return GetMaxMinusMin();
        }
        private long GetMaxMinusMin() {
            long[] counts = new long[26];
            counts[template[0] - 'A']++;
            counts[template[template.Length - 1] - 'A']++;
            foreach ((char a, char b) pair in pairs.Keys) {
                long value = pairs[pair];
                counts[pair.a - 'A'] += value;
                counts[pair.b - 'A'] += value;
            }

            long maxCount = 0;
            long minCount = long.MaxValue;
            for (int i = 0; i < 26; i++) {
                long count = counts[i];
                if (count == 0) { continue; }

                if (count < minCount) { minCount = count; }
                if (count > maxCount) { maxCount = count; }
            }

            return (maxCount - minCount) / 2L;
        }
    }
}