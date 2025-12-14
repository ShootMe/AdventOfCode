using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Factory")]
    public class Puzzle10 : ASolver {
        private List<Machine> machines = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                machines.Add(new Machine(line));
            }
        }

        [Description("What is the fewest presses required to configure the indicator lights on all of the machines?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < machines.Count; i++) {
                Machine machine = machines[i];
                total += machine.FewestButtonInitial();
            }
            return $"{total}";
        }

        [Description("What is the fewest presses required to configure the joltage level counters on all of the machines?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < machines.Count; i++) {
                Machine machine = machines[i];
                total += machine.FewestButtonJoltage();
            }
            return $"{total}";
        }

        private class Machine {
            public Light[] Lights;
            public byte[][] Buttons;
            public Machine(string init) {
                string[] splits = init.SplitOn("] ", "{");
                string lights = splits[0];
                string[] joltages = splits[2].Split(',');
                Lights = new Light[lights.Length - 1];
                for (int i = 0; i < Lights.Length; i++) {
                    Lights[i] = new Light(lights[i + 1] == '#', (byte)joltages[i].ToInt());
                }
                string[] buttons = splits[1].Split(") (");
                Buttons = new byte[buttons.Length][];
                for (int i = 0; i < buttons.Length; i++) {
                    splits = buttons[i].Split(',');
                    byte[] data = new byte[splits.Length];
                    Buttons[i] = data;
                    for (int j = 0; j < splits.Length; j++) {
                        data[j] = (byte)splits[j].ToInt();
                    }
                }
            }
            public int FewestButtonInitial() {
                HashSet<int> closed = new();
                Queue<(int, int)> open = new();
                int endValue = 0;
                for (int i = 0; i < Lights.Length; i++) {
                    if (Lights[i].Value) { endValue |= 1 << i; }
                }
                open.Enqueue((0, 0));
                closed.Add(0);
                while (open.Count > 0) {
                    (int current, int steps) = open.Dequeue();
                    if (current == endValue) { return steps; }

                    for (int i = 0; i < Buttons.Length; i++) {
                        byte[] data = Buttons[i];
                        int next = current;
                        for (int j = 0; j < data.Length; j++) {
                            next ^= 1 << data[j];
                        }
                        if (closed.Add(next)) { open.Enqueue((next, steps + 1)); }
                    }
                }
                return -1;
            }
            public int FewestButtonJoltage() {
                //To Finish
                return -1;
            }
        }
        private struct Light {
            public bool Value;
            public byte Joltage;
            public Light(bool value, byte joltage) {
                Value = value;
                Joltage = joltage;
            }
        }
    }
}