using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Custom Customs")]
    public class Puzzle06 : ASolver {
        [Description("What is the sum of those counts?")]
        public override string SolvePart1() {
            int total = 0;
            HashSet<char> questions = new HashSet<char>();

            foreach (string item in Input.Sections()) {
                questions.Clear();

                for (int j = 0; j < item.Length; j++) {
                    char answer = item[j];
                    if (answer != '\n') {
                        questions.Add(answer);
                    }
                }

                total += questions.Count;
            }

            return $"{total}";
        }

        [Description("What is the sum of those counts?")]
        public override string SolvePart2() {
            int total = 0;
            Dictionary<char, int> questions = new Dictionary<char, int>();

            foreach (string section in Input.Sections()) {
                string[] people = section.Split('\n');
                questions.Clear();

                for (int j = 0; j < people.Length; j++) {
                    string answers = people[j];

                    for (int k = 0; k < answers.Length; k++) {
                        int count;
                        if (questions.TryGetValue(answers[k], out count)) {
                            questions[answers[k]] = ++count;

                            if (count == people.Length) {
                                total++;
                            }
                        } else {
                            questions.Add(answers[k], 1);
                            if (1 == people.Length) {
                                total++;
                            }
                        }
                    }
                }
            }

            return $"{total}";
        }
    }
}