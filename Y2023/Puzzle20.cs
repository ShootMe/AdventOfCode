using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Pulse Propagation")]
    public class Puzzle20 : ASolver {
        private Dictionary<string, Module> modules = new();
        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                ParseLine(line);
            }
            foreach (string line in lines) {
                ParseLine(line);
            }
        }
        private void ParseLine(string line) {
            int type = 0;
            if (line[0] == '%') {
                type = 1;
            } else if (line[0] == '&') {
                type = 2;
            }

            int start = type > 0 ? 1 : 0;
            int index = line.IndexOf(" -> ");
            string name = line.Substring(start, index - start);

            if (modules.TryGetValue(name, out Module module)) {
                string outputs = line.Substring(index + 4);
                start = 0;
                while ((index = outputs.IndexOf(", ", start)) > 0) {
                    name = outputs.Substring(start, index - start);
                    AddOutput(module, name);
                    start = index + 2;
                }
                name = outputs.Substring(start, outputs.Length - start);
                AddOutput(module, name);
            } else {
                if (type == 0) {
                    module = new Module(name);
                } else if (type == 1) {
                    module = new FlipFlop(name);
                } else {
                    module = new Conjunction(name);
                }
                modules.Add(name, module);
            }
        }
        private void AddOutput(Module module, string name) {
            if (!modules.TryGetValue(name, out Module output)) {
                output = new Module(name);
                modules.Add(name, output);
            }
            output.Inputs.Add(module);
            module.Outputs.Add(output);
        }

        [Description("What do you get if you multiply the total number of low pulses sent by the total number of high pulses sent?")]
        public override string SolvePart1() {
            foreach (Module module in modules.Values) {
                module.Reset();
            }

            int totalHigh = 0; int totalLow = 0;
            Module broadcaster = modules["broadcaster"];
            Queue<(Module, Module)> pulses = new();
            for (int i = 0; i < 1000; i++) {
                pulses.Enqueue((broadcaster, broadcaster));
                while (pulses.Count > 0) {
                    (Module sender, Module current) = pulses.Dequeue();
                    if (sender.LastOutput) { totalHigh++; } else { totalLow++; }
                    current.Pulse(pulses, sender);
                }
            }
            return $"{totalHigh * totalLow}";
        }

        [Description("What is the fewest number of button presses required to deliver a single low pulse to the module named rx?")]
        public override string SolvePart2() {
            foreach (Module module in modules.Values) {
                module.Reset();
            }

            Queue<(Module, Module)> pulses = new();
            int presses = 0;
            Module rxIns = modules["rx"].Inputs[0];
            Dictionary<Module, int> states = new();
            for (int i = 0; i < rxIns.Inputs.Count; i++) {
                states[rxIns.Inputs[i]] = 0;
            }

            Module broadcaster = modules["broadcaster"];
            while (true) {
                presses++;
                pulses.Enqueue((broadcaster, broadcaster));
                while (pulses.Count > 0) {
                    (Module sender, Module current) = pulses.Dequeue();

                    if (sender.LastOutput && states.TryGetValue(sender, out int lastPress) && presses > lastPress) {
                        states[sender] = presses - lastPress;

                        long total = 1;
                        foreach (int value in states.Values) {
                            total *= value / Extensions.GCD(value, total);
                        }

                        if (total > 0) { return $"{total}"; }
                    }
                    current.Pulse(pulses, sender);
                }
            }
        }

        private class Module : IEquatable<Module> {
            private static int idCounter = 0;
            public int ID;
            public string Name;
            public bool LastOutput;
            public List<Module> Inputs = new();
            public List<Module> Outputs = new();
            public Module(string name) { Name = name; ID = ++idCounter; }
            public virtual void Pulse(Queue<(Module, Module)> pulses, Module sender) {
                for (int i = 0; i < Outputs.Count; i++) {
                    pulses.Enqueue((this, Outputs[i]));
                }
            }
            public virtual void Reset() { LastOutput = false; }
            public bool Equals(Module other) { return ID == other.ID; }
            public override int GetHashCode() { return ID; }
            public override string ToString() { return $"{Name}"; }
        }
        private class FlipFlop : Module {
            public FlipFlop(string name) : base(name) { }
            public override void Pulse(Queue<(Module, Module)> pulses, Module sender) {
                if (sender.LastOutput) { return; }

                for (int i = 0; i < Outputs.Count; i++) {
                    pulses.Enqueue((this, Outputs[i]));
                }
                LastOutput = !LastOutput;
            }
            public override string ToString() { return $"%{Name}({LastOutput})"; }
        }
        private class Conjunction : Module {
            public Conjunction(string name) : base(name) { }
            public override void Pulse(Queue<(Module, Module)> pulses, Module sender) {
                bool state = true;
                for (int i = 0; i < Inputs.Count && state; i++) {
                    state &= Inputs[i].LastOutput;
                }

                for (int i = 0; i < Outputs.Count; i++) {
                    pulses.Enqueue((this, Outputs[i]));
                }
                LastOutput = !state;
            }
            public override string ToString() {
                return $"&{Name}({LastOutput})";
            }
        }
    }
}