using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2017 {
    [Description("Electromagnetic Moat")]
    public class Puzzle24 : ASolver {
        private List<Component> components;

        public override void Setup() {
            List<string> items = Input.Lines();
            components = new List<Component>();
            for (int i = 0; i < items.Count; i++) {
                components.Add(new Component(items[i]));
            }
            components.Sort();
        }

        [Description("What is the strength of the strongest bridge you can make with the components available?")]
        public override string SolvePart1() {
            Stack<SolveState> states = new Stack<SolveState>();

            for (int i = 0; i < components.Count; i++) {
                Component component = components[i];
                if (component.Port1 == 0 || component.Port2 == 0) {
                    SolveState state = new SolveState() { Components = new List<Component>(components), Bridge = new List<Component>() };
                    state.Bridge.Add(component);
                    state.Components.RemoveAt(i);
                    state.PortAvailable = component.Port1 == 0 ? component.Port2 : component.Port1;
                    states.Push(state);
                }
            }

            int maxStrength = 0;
            while (states.Count > 0) {
                SolveState state = states.Pop();
                int strength = state.Strength();
                if (strength > maxStrength) {
                    maxStrength = strength;
                }

                for (int i = state.Components.Count - 1; i >= 0; i--) {
                    Component component = state.Components[i];
                    if (component.Port1 == state.PortAvailable || component.Port2 == state.PortAvailable) {
                        SolveState next = new SolveState() { Components = new List<Component>(state.Components), Bridge = new List<Component>(state.Bridge) };
                        next.Bridge.Add(component);
                        next.Components.RemoveAt(i);
                        next.PortAvailable = component.Port1 == state.PortAvailable ? component.Port2 : component.Port1;
                        states.Push(next);
                    }
                }
            }
            return $"{maxStrength}";
        }

        [Description("What is the strength of the longest bridge you can make?")]
        public override string SolvePart2() {
            Stack<SolveState> states = new Stack<SolveState>();

            for (int i = 0; i < components.Count; i++) {
                Component component = components[i];
                if (component.Port1 == 0 || component.Port2 == 0) {
                    SolveState state = new SolveState() { Components = new List<Component>(components), Bridge = new List<Component>() };
                    state.Bridge.Add(component);
                    state.Components.RemoveAt(i);
                    state.PortAvailable = component.Port1 == 0 ? component.Port2 : component.Port1;
                    states.Push(state);
                }
            }

            int maxStrength = 0;
            int maxLength = 0;
            while (states.Count > 0) {
                SolveState state = states.Pop();
                int strength = state.Strength();
                if (strength > maxStrength && state.Bridge.Count >= maxLength) {
                    maxStrength = strength;
                }
                if (state.Bridge.Count > maxLength) {
                    maxStrength = strength;
                    maxLength = state.Bridge.Count;
                }

                for (int i = state.Components.Count - 1; i >= 0; i--) {
                    Component component = state.Components[i];
                    if (component.Port1 == state.PortAvailable || component.Port2 == state.PortAvailable) {
                        SolveState next = new SolveState() { Components = new List<Component>(state.Components), Bridge = new List<Component>(state.Bridge) };
                        next.Bridge.Add(component);
                        next.Components.RemoveAt(i);
                        next.PortAvailable = component.Port1 == state.PortAvailable ? component.Port2 : component.Port1;
                        states.Push(next);
                    }
                }
            }
            return $"{maxStrength}";
        }

        private struct SolveState {
            public List<Component> Components, Bridge;
            public int PortAvailable;
            public int Strength() {
                int strength = 0;
                for (int i = 0; i < Bridge.Count; i++) {
                    strength += Bridge[i].Strength();
                }
                return strength;
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Bridge.Count; i++) {
                    sb.Append($"{Bridge[i]} ->");
                }
                if (sb.Length > 0) { sb.Length -= 3; }
                return sb.ToString();
            }
        }
        private struct Component : IEquatable<Component>, IComparable<Component> {
            public int Port1, Port2;
            public Component(string value) {
                int index = value.IndexOf('/');
                Port1 = value.Substring(0, index).ToInt();
                Port2 = value.Substring(index + 1).ToInt();
            }

            public int Strength() {
                return Port1 + Port2;
            }
            public int CompareTo(Component other) {
                int compare = Port1.CompareTo(other.Port1);
                if (compare != 0) { return compare; }
                return Port2.CompareTo(other.Port2);
            }
            public bool Equals(Component other) {
                return Port1 == other.Port1 && Port2 == other.Port2;
            }
            public override bool Equals(object obj) {
                return obj is Component component && Equals(component);
            }
            public override int GetHashCode() {
                return (Port1 << 16) | Port2;
            }
            public override string ToString() {
                return $"{Port1} | {Port2}";
            }
        }
    }
}