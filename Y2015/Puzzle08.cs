using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle08 : ASolver {
        private List<string> items;
        public Puzzle08(string input) : base(input) { Name = "Matchsticks"; }

        public override void Setup() {
            items = Tools.GetLines(Input);
        }

        [Description("What is the number of characters of code minus the number of characters in memory?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                count += item.Length + 2;
                int j = 0;
                while (ParseString(item, ref j)) {
                    count--;
                }
            }

            return $"{count}";
        }

        [Description("What is the number of characters to represent the newly encoded strings minus the characters of code?")]
        public override string SolvePart2() {
            int count = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                count += EncodedLength(item) - item.Length;
            }

            return $"{count}";
        }

        private int EncodedLength(string item) {
            int count = 2 + item.Length;
            for (int i = 0; i < item.Length; i++) {
                char c = item[i];
                switch (c) {
                    case '\\':
                    case '\"': count++; break;
                }
            }
            return count;
        }
        private bool ParseString(string item, ref int index) {
            if (index >= item.Length) { return false; }

            char c = item[index++];
            switch (c) {
                case '\\':
                    EscapeString(item, ref index);
                    break;
            }
            return true;
        }
        private void EscapeString(string item, ref int index) {
            char c = item[index];
            switch (c) {
                case 'x':
                    index += 3;
                    break;
                case '\\':
                case '\"':
                    index++;
                    break;
            }
        }
    }
}