using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Like a GIF For Your Yard")]
    public class Puzzle18 : ASolver {
        private BitGrid lights, state;

        public override void Setup() {
            List<string> items = Input.Lines();
            lights = new BitGrid(items[0].Length + 2, items.Count + 2);

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                for (int j = 0; j < item.Length; j++) {
                    if (item[j] == '#') {
                        lights[j + 1, i + 1] = 1;
                    }
                }
            }

            state = new BitGrid(lights.Width, lights.Height);
        }

        [Description("How many lights are on after 100 steps?")]
        public override string SolvePart1() {
            for (int i = 0; i < 100; i++) {
                Step();
            }
            return $"{lights.Count()}";
        }

        [Description("How many lights are on after 100 steps when the four corners are always lit?")]
        public override string SolvePart2() {
            Setup();

            for (int i = 0; i < 100; i++) {
                lights[1, 1] = 1;
                lights[100, 1] = 1;
                lights[1, 100] = 1;
                lights[100, 100] = 1;

                Step();
            }

            lights[1, 1] = 1;
            lights[100, 1] = 1;
            lights[1, 100] = 1;
            lights[100, 100] = 1;

            return $"{lights.Count()}";
        }

        private void Step() {
            int width = lights.Width - 1;
            int height = lights.Height - 1;
            for (int i = 1; i < height; i++) {
                for (int j = 1; j < width; j++) {
                    byte isOn = lights[j, i];
                    int count = lights.Count(j - 1, i - 1, 3) + lights.Count(j - 1, i, 3) + lights.Count(j - 1, i + 1, 3);

                    if (isOn == 1 && (count < 3 || count > 4)) {
                        state[j, i] = 0;
                    } else if (isOn == 0 && count == 3) {
                        state[j, i] = 1;
                    } else {
                        state[j, i] = isOn;
                    }
                }
            }

            BitGrid temp = state;
            state = lights;
            lights = temp;
        }
    }
}