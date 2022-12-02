using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Permutation Promenade")]
    public class Puzzle16 : ASolver {
        private string[] actions;

        public override void Setup() {
            actions = Input.Split(',');
        }

        [Description("In what order are the programs standing after their dance?")]
        public override string SolvePart1() {
            char[] arrangement = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p' };
            return Dance(arrangement);
        }

        [Description("In what order are the programs standing after their billion dances?")]
        public override string SolvePart2() {
            Dictionary<string, int> dances = new Dictionary<string, int>();
            char[] arrangement = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p' };
            int count = 0;
            while (true) {
                string dance = Dance(arrangement);
                if (dances.ContainsKey(dance)) {
                    int danceNumber = 1000000000 % count;
                    foreach (KeyValuePair<string, int> pair in dances) {
                        if (pair.Value == danceNumber) {
                            return pair.Key;
                        }
                    }
                    return string.Empty;
                }

                dances.Add(dance, ++count);
            }
        }

        private string Dance(char[] arrangement) {
            for (int i = 0; i < actions.Length; i++) {
                string dance = actions[i];
                if (dance[0] == 's') {
                    int value = dance.Substring(1).ToInt();
                    Spin(arrangement, value);
                } else if (dance[0] == 'x') {
                    int index = dance.IndexOf('/');
                    int value1 = dance.Substring(1, index - 1).ToInt();
                    int value2 = dance.Substring(index + 1).ToInt();
                    Exchange(arrangement, value1, value2);
                } else {
                    Partner(arrangement, dance[1], dance[3]);
                }
            }
            return new string(arrangement);
        }
        private void Spin(char[] arrangement, int value) {
            if (value >= arrangement.Length) { return; }

            char[] temp = new char[arrangement.Length - value];
            Array.Copy(arrangement, 0, temp, 0, temp.Length);
            Array.Copy(arrangement, temp.Length, arrangement, 0, value);
            Array.Copy(temp, 0, arrangement, value, temp.Length);
        }
        private void Exchange(char[] arrangement, int index1, int index2) {
            char temp = arrangement[index1];
            arrangement[index1] = arrangement[index2];
            arrangement[index2] = temp;
        }
        private void Partner(char[] arrangement, char p1, char p2) {
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < arrangement.Length; i++) {
                char c = arrangement[i];
                if (c == p1) {
                    index1 = i;
                }
                if (c == p2) {
                    index2 = i;
                }
            }
            Exchange(arrangement, index1, index2);
        }
    }
}