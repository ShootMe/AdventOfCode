using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Subterranean Sustainability")]
    public class Puzzle12 : ASolver {
        private const int Buffer = 300;
        private Dictionary<byte, bool> rules;
        private byte[] plants, state;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            string initialState = items[0].Substring(15);
            plants = new byte[initialState.Length + Buffer * 2];
            state = new byte[plants.Length];
            rules = new Dictionary<byte, bool>();

            for (int i = 0; i < initialState.Length; i++) {
                plants[i + Buffer] = initialState[i] == '#' ? (byte)1 : (byte)0;
            }

            for (int i = 2; i < items.Count; i++) {
                string item = items[i];
                byte rule = (byte)((item[0] == '#' ? 16 : 0) | (item[1] == '#' ? 8 : 0) | (item[2] == '#' ? 4 : 0) | (item[3] == '#' ? 2 : 0) | (item[4] == '#' ? 1 : 0));
                bool becomesPlant = item[9] == '#';
                rules.Add(rule, becomesPlant);
            }
        }

        [Description("After 20 generations, what is the sum of the numbers of all pots which contain a plant?")]
        public override string SolvePart1() {
            byte[] original = new byte[plants.Length];
            Array.Copy(plants, original, plants.Length);

            for (int i = 0; i < 20; i++) {
                Evolve();
            }

            int count = CountPlants();
            Array.Copy(original, plants, plants.Length);
            return $"{count}";
        }

        [Description("After 50-billion generations, what is the sum of the numbers of all pots which contain a plant?")]
        public override string SolvePart2() {
            byte[] original = new byte[plants.Length];
            Array.Copy(plants, original, plants.Length);

            int lastCount = CountPlants();
            int lastDiff = lastCount;
            for (int i = 0; i < 500; i++) {
                Evolve();
                int newCount = CountPlants();
                int newDiff = newCount - lastCount;
                if (newDiff == lastDiff && newDiff > 0) {
                    return $"{(long)newCount + (50000000000 - 1 - i) * (long)newDiff}";
                }
                lastCount = newCount;
                lastDiff = newDiff;
            }
            return string.Empty;
        }

        private void Evolve() {
            Array.Copy(plants, state, plants.Length);

            for (int i = 0; i < plants.Length; i++) {
                byte rule = (byte)((i > 1 ? plants[i - 2] << 4 : 0) | (i > 0 ? plants[i - 1] << 3 : 0) | (plants[i] << 2) | (i + 1 < plants.Length ? plants[i + 1] << 1 : 0) | (i + 2 < plants.Length ? plants[i + 2] : 0));
                bool outcome = rules[rule];
                state[i] = outcome ? (byte)1 : (byte)0;
            }

            Array.Copy(state, plants, plants.Length);
        }
        private int CountPlants() {
            int count = 0;
            for (int i = 0; i < plants.Length; i++) {
                if (plants[i] != 0) {
                    count += i - Buffer;
                }
            }

            return count;
        }
    }
}