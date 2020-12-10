using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    public class Puzzle04 : ASolver {
        private List<string> phrases;
        public Puzzle04(string input) : base(input) { Name = "High-Entropy Passphrases"; }

        public override void Setup() {
            phrases = Tools.GetLines(Input);
        }

        [Description("How many passphrases are valid?")]
        public override string SolvePart1() {
            HashSet<string> distinct = new HashSet<string>();
            int total = 0;
            for (int i = 0; i < phrases.Count; i++) {
                string[] items = phrases[i].Split(' ');
                distinct.Clear();

                bool valid = true;
                for (int j = 0; j < items.Length; j++) {
                    string item = items[j];
                    if (!distinct.Add(item)) {
                        valid = false;
                        break;
                    }
                }

                if (valid) {
                    total++;
                }
            }
            return $"{total}";
        }

        [Description("How many passphrases are valid?")]
        public override string SolvePart2() {
            HashSet<string> distinct = new HashSet<string>();
            int total = 0;
            for (int i = 0; i < phrases.Count; i++) {
                string[] items = phrases[i].Split(' ');
                distinct.Clear();

                bool valid = true;
                for (int j = 0; j < items.Length; j++) {
                    string item = SortString(items[j]);
                    if (!distinct.Add(item)) {
                        valid = false;
                        break;
                    }
                }

                if (valid) {
                    total++;
                }
            }
            return $"{total}";
        }

        private string SortString(string input) {
            char[] characters = input.ToCharArray();
            Array.Sort(characters);
            return new string(characters);
        }
    }
}