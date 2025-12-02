using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Secret Entrance")]
    public class Puzzle01 : ASolver {
        private List<int> rotations = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                int num = line.ToInt();
                if (line[0] == 'L') { num = -num; }
                rotations.Add(num);
            }
        }

        [Description("What's the actual password to open the door?")]
        public override string SolvePart1() {
            int position = 50;
            int password = 0;
            for (int i = 0; i < rotations.Count; i++) {
                position += rotations[i] % 100;
                if (position < 0) { position += 100; }
                if (position >= 100) { position -= 100; }
                if (position == 0) { password++; }
            }
            return $"{password}";
        }

        [Description("Using password method 0x434C49434B, what is the password to open the door?")]
        public override string SolvePart2() {
            int position = 50;
            int password = 0;
            for (int i = 0; i < rotations.Count; i++) {
                int num = rotations[i];
                int div = num / 100;
                num %= 100;
                password += div < 0 ? -div : div;

                int startingPosition = position;
                position += num;
                if (position >= 100) {
                    position -= 100;
                    password++;
                } else if (position < 0) {
                    position += 100;
                    if (startingPosition != 0) { password++; }
                } else if (position == 0) {
                    password++;
                }
            }
            return $"{password}";
        }
    }
}