using AdventOfCode.Core;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2015 {
    [Description("Elves Look, Elves Say")]
    public class Puzzle10 : ASolver {
        [Description("What is the length of the result?")]
        public override string SolvePart1() {
            string current = Input;
            for (int i = 0; i < 40; i++) {
                current = LookAndSay(current);
            }

            return $"{current.Length}";
        }

        [Description("What is the length of the new result?")]
        public override string SolvePart2() {
            string current = Input;
            for (int i = 0; i < 50; i++) {
                current = LookAndSay(current);
            }

            return $"{current.Length}";
        }

        private string LookAndSay(string value) {
            StringBuilder result = new StringBuilder((int)(value.Length * 1.4));
            char last = value[0];
            int count = 0;
            for (int i = 0; i < value.Length; i++) {
                char current = value[i];
                if (last != current) {
                    result.Append(count).Append(last);
                    count = 0;
                }
                count++;
                last = current;
            }
            result.Append(count).Append(last);
            return result.ToString();
        }
    }
}