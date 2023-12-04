using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Gear Ratios")]
    public class Puzzle03 : ASolver {
        private string[] grid;
        private int width, height;
        private List<Part> parts = new List<Part>();
        public override void Setup() {
            grid = Input.Split('\n');
            width = grid[0].Length;
            height = grid.Length;
            Dictionary<(int, int), Gear> gears = new Dictionary<(int, int), Gear>();

            for (int i = 0; i < grid.Length; i++) {
                string line = grid[i];
                for (int j = 0; j < line.Length; j++) {
                    char c = line[j];
                    if (char.IsDigit(c)) {
                        int endIndex = j + 1;
                        while (endIndex < line.Length) {
                            char n = line[endIndex];
                            if (!char.IsDigit(n)) {
                                break;
                            }
                            endIndex++;
                        }

                        Part part = new Part();
                        part.Value = line.Substring(j, endIndex - j).ToInt();
                        if (HasSymbol(j - 1, endIndex, i - 1) || HasSymbol(j - 1, endIndex, i) || HasSymbol(j - 1, endIndex, i + 1)) {
                            part.Symbols = true;
                        }
                        UpdateGear(j - 1, endIndex, i - 1, part, gears);
                        UpdateGear(j - 1, endIndex, i, part, gears);
                        UpdateGear(j - 1, endIndex, i + 1, part, gears);
                        parts.Add(part);

                        j = endIndex;
                    }
                }
            }
        }

        [Description("What is the sum of all of the part numbers in the engine schematic?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < parts.Count; i++) {
                Part part = parts[i];
                if (part.Symbols) {
                    total += part.Value;
                }
            }
            return $"{total}";
        }

        [Description("What is the sum of all of the gear ratios in your engine schematic?")]
        public override string SolvePart2() {
            int total = 0;
            HashSet<(int, int)> gearsSeen = new HashSet<(int, int)>();
            for (int i = 0; i < parts.Count; i++) {
                Part part = parts[i];
                if (part.Gears.Count > 0) {
                    for (int j = 0; j < part.Gears.Count; j++) {
                        Gear gear = part.Gears[j];
                        if (gear.Parts.Count == 2 && gearsSeen.Add((gear.X,gear.Y))) {
                            total += gear.Parts[0].Value * gear.Parts[1].Value;
                        }
                    }
                }
            }
            return $"{total}";
        }

        private bool HasSymbol(int startX, int endX, int y) {
            if (y < 0 || y >= height) { return false; }

            string line = grid[y];
            for (int i = startX >= 0 ? startX : 0; i < width && i <= endX; i++) {
                if (line[i] != '.' && !char.IsDigit(line[i])) {
                    return true;
                }
            }
            return false;
        }
        private void UpdateGear(int startX, int endX, int y, Part part, Dictionary<(int, int), Gear> gears) {
            if (y < 0 || y >= height) { return; }

            string line = grid[y];
            for (int i = startX >= 0 ? startX : 0; i < width && i <= endX; i++) {
                if (line[i] == '*') {
                    if (gears.TryGetValue((i, y), out Gear value)) {
                        value.Parts.Add(part);
                        part.Gears.Add(value);
                    } else {
                        Gear gear = new Gear() { X = i, Y = y };
                        gear.Parts.Add(part);
                        part.Gears.Add(gear);
                        gears.Add((i, y), gear);
                    }
                }
            }
        }
        private class Part {
            public int Value;
            public List<Gear> Gears = new List<Gear>();
            public bool Symbols = false;
            public override string ToString() {
                return $"{Value}={Symbols},{Gears.Count}";
            }
        }
        private class Gear {
            public int X, Y;
            public List<Part> Parts = new List<Part>();
            public override string ToString() {
                return $"{X},{Y}";
            }
        }
    }
}