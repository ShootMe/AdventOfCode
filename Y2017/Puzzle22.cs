using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Sporifica Virus")]
    public class Puzzle22 : ASolver {
        private Dictionary<Point, State> grid;
        private int width, height;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);

            grid = new Dictionary<Point, State>();

            height = items.Count;
            width = items[0].Length;

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                for (int j = 0; j < item.Length; j++) {
                    grid[new Point() { X = j, Y = i }] = item[j] == '#' ? State.Infected : State.Clean;
                }
            }
        }

        [Description("After 10000 bursts, how many cause a node to become infected?")]
        public override string SolvePart1() {
            Dictionary<Point, State> copy = new Dictionary<Point, State>();
            foreach (KeyValuePair<Point, State> pair in grid) {
                copy[pair.Key] = pair.Value;
            }

            Point position = new Point() { X = width / 2, Y = height / 2 };
            int direction = 0;
            int count = 0;
            for (int i = 0; i < 10000; i++) {
                State current;
                copy.TryGetValue(position, out current);

                if (current == State.Infected) {
                    direction = (direction + 1) & 3;
                    copy[position] = State.Clean;
                } else {
                    count++;
                    direction = (direction - 1) & 3;
                    copy[position] = State.Infected;
                }

                switch (direction) {
                    case 0: position += Point.NORTH; break;
                    case 1: position += Point.EAST; break;
                    case 2: position += Point.SOUTH; break;
                    case 3: position += Point.WEST; break;
                }
            }

            return $"{count}";
        }

        [Description("After 10000000 bursts, how many cause a node to become infected?")]
        public override string SolvePart2() {
            Point position = new Point() { X = width / 2, Y = height / 2 };
            int direction = 0;
            int count = 0;
            for (int i = 0; i < 10000000; i++) {
                State current;
                grid.TryGetValue(position, out current);

                if (current == State.Clean) {
                    direction = (direction - 1) & 3;
                    grid[position] = State.Weakened;
                } else if (current == State.Weakened) {
                    grid[position] = State.Infected;
                    count++;
                } else if (current == State.Infected) {
                    direction = (direction + 1) & 3;
                    grid[position] = State.Flagged;
                } else {
                    direction = (direction + 2) & 3;
                    grid[position] = State.Clean;
                }

                switch (direction) {
                    case 0: position += Point.NORTH; break;
                    case 1: position += Point.EAST; break;
                    case 2: position += Point.SOUTH; break;
                    case 3: position += Point.WEST; break;
                }
            }

            return $"{count}";
        }

        private enum State : byte {
            Clean,
            Weakened,
            Infected,
            Flagged
        }
    }
}