using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Lens Library")]
    public class Puzzle15 : ASolver {
        private Label[] sequences;

        public override void Setup() {
            string[] sequenceValues = Input.Split(',');
            sequences = new Label[sequenceValues.Length];
            for (int i = 0; i < sequenceValues.Length; i++) {
                string value = sequenceValues[i];
                sequences[i] = new Label(value);
            }
        }

        [Description("What is the sum of the results?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < sequences.Length; i++) {
                Label value = sequences[i];
                total += value.FullHash;
            }
            return $"{total}";
        }


        [Description("What is the focusing power of the resulting lens configuration?")]
        public override string SolvePart2() {
            List<Label>[] maps = new List<Label>[256];
            for (int i = 0; i < 256; i++) {
                maps[i] = new List<Label>();
            }

            for (int i = 0; i < sequences.Length; i++) {
                Label value = sequences[i];
                List<Label> map = maps[value.Hash];
                if (value.Add) {
                    int index = map.IndexOf(value);
                    if (index >= 0) {
                        map[index] = value;
                    } else {
                        map.Add(value);
                    }
                } else {
                    map.Remove(value);
                }
            }

            int total = 0;
            for (int i = 0; i < 256; i++) {
                List<Label> map = maps[i];
                for (int j = 0; j < map.Count; j++) {
                    Label value = map[j];
                    total += (i + 1) * (j + 1) * value.Lens;
                }
            }
            return $"{total}";
        }

        private class Label : IEquatable<Label> {
            public string Name;
            public int Hash;
            public int FullHash;
            public int Lens;
            public bool Add;
            public Label(string sequence) {
                int index = sequence.IndexOfAny(['-', '=']);
                Add = sequence.IndexOf('-') < 0;
                if (Add) {
                    Lens = sequence.Substring(index + 1).ToInt();
                }
                Name = sequence.Substring(0, index);
                Hash = CalculateHash(Name);
                FullHash = CalculateHash(sequence);
            }
            public bool Equals(Label other) {
                return Name == other.Name;
            }
            public override string ToString() {
                return $"{Name}({Lens}) = {Hash} ";
            }
            private int CalculateHash(string value) {
                int result = 0;
                for (int i = 0; i < value.Length; i++) {
                    result += value[i];
                    result *= 17;
                    result &= 0xff;
                }
                return result;
            }
        }
    }
}