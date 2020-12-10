using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    public class Puzzle06 : ASolver {
        private List<string> items;
        public Puzzle06(string input) : base(input) { Name = "Custom Customs"; }

        public override void Setup() {
            items = Tools.GetSections(Input);
        }

        [Description("What is the sum of those counts?")]
        public override string SolvePart1() {
            int total = 0;
            HashSet<char> questions = new HashSet<char>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
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

            for (int i = 0; i < items.Count; i++) {
                string[] people = items[i].Split('\n');
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