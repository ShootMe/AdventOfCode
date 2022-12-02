using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Dive!")]
    public class Puzzle02 : ASolver {
        private List<(string, int)> commands;

        public override void Setup() {
            List<string> items = Input.Lines();
            commands = new List<(string, int)>();
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index = item.IndexOf(' ');
                commands.Add((item.Substring(0, index), Tools.ParseInt(item, index + 1)));
            }
        }

        [Description("What do you get if you multiply your final horizontal position by your final depth?")]
        public override string SolvePart1() {
            int horizontal = 0;
            int depth = 0;
            for (int i = 0; i < commands.Count; i++) {
                (string type, int amount) = commands[i];
                switch (type) {
                    case "forward": horizontal += amount; break;
                    case "down": depth += amount; break;
                    case "up": depth -= amount; break;
                }
            }
            return $"{horizontal * depth}";
        }

        [Description("What do you get if you multiply your final horizontal position by your final depth?")]
        public override string SolvePart2() {
            int horizontal = 0;
            int depth = 0;
            int aim = 0;
            for (int i = 0; i < commands.Count; i++) {
                (string type, int amount) = commands[i];
                switch (type) {
                    case "forward":
                        horizontal += amount;
                        depth += aim * amount;
                        break;
                    case "down": aim += amount; break;
                    case "up": aim -= amount; break;
                }
            }
            return $"{horizontal * depth}";
        }
    }
}