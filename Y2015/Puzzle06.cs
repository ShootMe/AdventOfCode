using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle06 : ASolver {
        private LightAction[] actions;
        public Puzzle06(string input) : base(input) { Name = "Probably a Fire Hazard"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            actions = new LightAction[items.Count];
            for (int i = 0; i < actions.Length; i++) {
                actions[i] = items[i];
            }
        }

        [Description("How many lights are lit?")]
        public override string SolvePart1() {
            bool[] lights = new bool[1000000];
            for (int i = 0; i < actions.Length; i++) {
                LightAction action = actions[i];
                DoAction(lights, action);
            }

            int count = 0;
            for (int i = 0; i < lights.Length; i++) {
                if (lights[i]) {
                    count++;
                }
            }

            return $"{count}";
        }

        [Description("What is the total brightness of all lights combined?")]
        public override string SolvePart2() {
            int[] lights = new int[1000000];
            for (int i = 0; i < actions.Length; i++) {
                LightAction action = actions[i];
                DoBrightness(lights, action);
            }

            int count = 0;
            for (int i = 0; i < lights.Length; i++) {
                count += lights[i];
            }

            return $"{count}";
        }

        private void DoAction(bool[] lights, LightAction action) {
            for (int i = action.X + action.W; i >= action.X; i--) {
                for (int j = action.Y + action.H; j >= action.Y; j--) {
                    lights[j * 1000 + i] = action.Action == 0 ? false : action.Action == 1 ? true : !lights[j * 1000 + i];
                }
            }
        }
        private void DoBrightness(int[] lights, LightAction action) {
            for (int i = action.X + action.W; i >= action.X; i--) {
                for (int j = action.Y + action.H; j >= action.Y; j--) {
                    ref int current = ref lights[j * 1000 + i];
                    if (action.Action == 0 && current > 0) {
                        current--;
                    } else if (action.Action == 1) {
                        current++;
                    } else if (action.Action == -1) {
                        current += 2;
                    }
                }
            }
        }

        private class LightAction {
            public int X, Y, W, H;
            public int Action;

            public static implicit operator LightAction(string item) {
                int action;
                int index1;
                if (item.StartsWith("turn on", StringComparison.OrdinalIgnoreCase)) {
                    action = 1;
                    index1 = 8;
                } else if (item.StartsWith("turn off", StringComparison.OrdinalIgnoreCase)) {
                    action = 0;
                    index1 = 9;
                } else {
                    action = -1;
                    index1 = 7;
                }

                int index2 = item.IndexOf(',', index1);
                int x1 = Tools.ParseInt(item, index1, index2 - index1);

                int index3 = item.IndexOf(' ', index2);
                int y1 = Tools.ParseInt(item, index2 + 1, index3 - index2 - 1);

                int index4 = item.IndexOf(',', index3);
                int x2 = Tools.ParseInt(item, index3 + 9, index4 - index3 - 9);

                int y2 = Tools.ParseInt(item, index4 + 1);

                int xs = Math.Min(x1, x2);
                int ys = Math.Min(y1, y2);
                int xe = Math.Max(x1, x2);
                int ye = Math.Max(y1, y2);

                return new LightAction() {
                    X = xs,
                    Y = ys,
                    W = xe - xs,
                    H = ye - ys,
                    Action = action
                };
            }
        }
    }
}