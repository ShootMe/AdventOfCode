using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Corporate Policy")]
    public class Puzzle11 : ASolver {
        [Description("What should his next password be?")]
        public override string SolvePart1() {
            char[] password = new char[Input.Length];
            for (int i = 0; i < Input.Length; i++) {
                password[i] = Input[i];
            }

            while (true) {
                AddOne(password);
                if (IsValid(password)) {
                    return new string(password);
                }
            }
        }

        [Description("Santa's password expired again. What's the next one?")]
        public override string SolvePart2() {
            char[] password = new char[Input.Length];
            for (int i = 0; i < Input.Length; i++) {
                password[i] = Input[i];
            }

            int count = 0;
            while (true) {
                AddOne(password);
                if (IsValid(password)) {
                    count++;
                    if (count == 2) {
                        return new string(password);
                    }
                }
            }
        }

        private void AddOne(char[] password) {
            int index = password.Length - 1;
            do {
                ref char c = ref password[index--];
                c++;
                if (c <= 'z') { break; }
                c = 'a';
            } while (index >= 0);
        }
        private bool IsValid(char[] password) {
            bool has3Increasing = false;
            bool has2Doubles = false;

            char lastDouble = '\0';
            int increasing = 1;
            char last = '\0';
            for (int i = password.Length - 1; i >= 0; i--) {
                char c = password[i];
                if (c == 'i' || c == 'o' || c == 'l') { return false; }

                if (last - c == 1) {
                    increasing++;
                    if (increasing == 3) {
                        has3Increasing = true;
                    }
                } else {
                    increasing = 1;
                }

                if (c == last) {
                    if (lastDouble == '\0') {
                        lastDouble = c;
                    } else if (c != lastDouble) {
                        has2Doubles = true;
                    }
                }

                last = c;
            }

            return has3Increasing && has2Doubles;
        }
    }
}