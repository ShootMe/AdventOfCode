using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Memory Reallocation")]
    public class Puzzle06 : ASolver {
        private int[] buckets;

        public override void Setup() {
            string[] items = Input.Split('\t');
            buckets = new int[items.Length];
            for (int i = 0; i < items.Length; i++) {
                buckets[i] = Tools.ParseInt(items[i]);
            }
        }

        [Description("How many redistribution cycles needed before a configuration has been seen before?")]
        public override string SolvePart1() {
            int[] state = new int[buckets.Length];
            Array.Copy(buckets, state, buckets.Length);

            int maxValue = 0;
            int maxIndex = 0;
            for (int i = 0; i < state.Length; i++) {
                int value = state[i];
                if (value > maxValue) {
                    maxValue = value;
                    maxIndex = i;
                }
            }

            HashSet<int[]> states = new HashSet<int[]>(ArrayComparer<int>.Comparer);
            int count = 0;
            while (states.Add(state)) {
                count++;

                int[] newState = new int[buckets.Length];
                Array.Copy(state, newState, buckets.Length);

                maxIndex = Redistribute(newState, maxIndex);
                state = newState;
            }

            return $"{count}";
        }

        [Description("How many cycles are in the infinite loop?")]
        public override string SolvePart2() {
            int[] state = new int[buckets.Length];
            Array.Copy(buckets, state, buckets.Length);

            int maxValue = 0;
            int maxIndex = 0;
            for (int i = 0; i < state.Length; i++) {
                int value = state[i];
                if (value > maxValue) {
                    maxValue = value;
                    maxIndex = i;
                }
            }

            Dictionary<int[], int> states = new Dictionary<int[], int>(ArrayComparer<int>.Comparer);
            int count = 0;
            while (!states.ContainsKey(state)) {
                states.Add(state, count);
                count++;

                int[] newState = new int[buckets.Length];
                Array.Copy(state, newState, buckets.Length);

                maxIndex = Redistribute(newState, maxIndex);
                state = newState;
            }

            int previousCount = states[state];

            return $"{count - previousCount}";
        }

        private int Redistribute(int[] values, int index) {
            int count = values[index];
            values[index++] = 0;
            int maxValue = 0;
            int maxIndex = 0;
            for (int i = 0; i < count; i++, index++) {
                if (index >= values.Length) { index = 0; }

                int value = ++values[index];
                if (value > maxValue || (value == maxValue && index < maxIndex)) {
                    maxValue = value;
                    maxIndex = index;
                }
            }

            while (count++ < values.Length) {
                if (index >= values.Length) { index = 0; }

                int value = values[index];
                if (value > maxValue || (value == maxValue && index < maxIndex)) {
                    maxValue = value;
                    maxIndex = index;
                }
                index++;
            }

            return maxIndex;
        }
    }
}