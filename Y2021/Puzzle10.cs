using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Syntax Scoring")]
    public class Puzzle10 : ASolver {
        private List<string> lines;
        private readonly Dictionary<char, int> errorScores = new Dictionary<char, int>() { { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 } };
        private readonly Dictionary<char, int> completionScores = new Dictionary<char, int>() { { '(', 1 }, { '[', 2 }, { '{', 3 }, { '<', 4 } };
        private readonly Dictionary<char, char> mapping = new Dictionary<char, char>() { { ')', '(' }, { ']', '[' }, { '}', '{' }, { '>', '<' } };

        public override void Setup() {
            lines = Tools.GetLines(Input);
        }

        [Description("What is the total syntax error score for those errors?")]
        public override string SolvePart1() {
            long totalScore = 0;
            for (int i = 0; i < lines.Count; i++) {
                string line = lines[i];
                totalScore += GetScore(line);
            }
            return $"{totalScore}";
        }

        [Description("What is the middle score?")]
        public override string SolvePart2() {
            List<long> scores = new List<long>();
            for (int i = 0; i < lines.Count; i++) {
                long score = GetScore(lines[i], false);
                if (score == 0) { continue; }
                scores.Add(score);
            }
            scores.Sort();
            return $"{scores[scores.Count / 2]}";
        }

        private long GetScore(string line, bool getCorrupted = true) {
            Stack<char> values = new Stack<char>();

            for (int i = 0; i < line.Length; i++) {
                char c = line[i];
                switch (c) {
                    case '{':
                    case '[':
                    case '<':
                    case '(': values.Push(c); break;
                    case '}':
                    case ']':
                    case '>':
                    case ')':
                        if (values.Pop() != mapping[c]) {
                            return getCorrupted ? errorScores[c] : 0;
                        }
                        break;
                }
            }

            if (getCorrupted) { return 0; }

            long score = 0;
            while (values.Count > 0) {
                score = score * 5 + completionScores[values.Pop()];
            }
            return score;
        }
    }
}