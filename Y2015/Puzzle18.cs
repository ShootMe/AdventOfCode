using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle18 : ASolver {
        private byte[] lights, state;
        public Puzzle18(string input) : base(input) { Name = "Like a GIF For Your Yard"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            lights = new byte[100 * 100];
            int index = 0;

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                for (int j = 0; j < item.Length; j++) {
                    lights[index++] = item[j] == '#' ? (byte)1 : (byte)0;
                }
            }

            state = new byte[lights.Length];
            Array.Copy(lights, state, lights.Length);
        }

        [Description("How many lights are on after 100 steps?")]
        public override string SolvePart1() {
            byte[] original = new byte[lights.Length];
            Array.Copy(lights, original, lights.Length);

            for (int i = 0; i < 100; i++) {
                Step();
            }

            int count = 0;
            for (int i = 0; i < lights.Length; i++) {
                count += lights[i];
            }

            Array.Copy(original, lights, lights.Length);
            return $"{count}";
        }

        [Description("How many lights are on after 100 steps when the four corners are always lit?")]
        public override string SolvePart2() {
            for (int i = 0; i < 100; i++) {
                lights[0] = 1;
                lights[lights.Length - 1] = 1;
                lights[99] = 1;
                lights[9900] = 1;
                Step();
            }

            lights[0] = 1;
            lights[lights.Length - 1] = 1;
            lights[99] = 1;
            lights[9900] = 1;
            int count = 0;
            for (int i = 0; i < lights.Length; i++) {
                count += lights[i];
            }

            return $"{count}";
        }

        private void Step() {
            int x = 0;
            int y = 0;
            for (int i = 0; i < lights.Length; i++) {
                byte isOn = lights[i];

                byte count = IsLightOn(x - 1, y - 1);
                count += IsLightOn(x - 1, y);
                count += IsLightOn(x - 1, y + 1);
                count += IsLightOn(x, y - 1);
                count += IsLightOn(x, y + 1);
                count += IsLightOn(x + 1, y - 1);
                count += IsLightOn(x + 1, y);
                count += IsLightOn(x + 1, y + 1);

                if (isOn == 1 && (count < 2 || count > 3)) {
                    state[i] = 0;
                } else if (isOn == 0 && count == 3) {
                    state[i] = 1;
                } else {
                    state[i] = isOn;
                }

                x++;
                if (x == 100) {
                    x = 0;
                    y++;
                }
            }

            byte[] temp = state;
            state = lights;
            lights = temp;
        }
        private byte IsLightOn(int x, int y) {
            if (x < 0 || y < 0 || x >= 100 || y >= 100) { return 0; }
            return lights[y * 100 + x];
        }
    }
}