using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Rain Risk")]
    public class Puzzle12 : ASolver {
        private List<string> items;

        public override void Setup() {
            items = Input.Lines();
        }

        [Description("What is the Manhattan distance between that location and the ship's starting position?")]
        public override string SolvePart1() {
            int direction = 1;
            int x = 0;
            int y = 0;
            int xMove = 1;
            int yMove = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int amount = item.Substring(1).ToInt();
                switch (item[0]) {
                    case 'N': y -= amount; break;
                    case 'S': y += amount; break;
                    case 'E': x += amount; break;
                    case 'W': x -= amount; break;
                    case 'L':
                        direction -= amount / 90;
                        if (direction < 0) { direction += 4; }
                        break;
                    case 'R':
                        direction += amount / 90;
                        if (direction >= 4) { direction -= 4; }
                        break;
                    case 'F': x += xMove * amount; y += yMove * amount; break;
                }

                switch (direction) {
                    case 0: xMove = 0; yMove = -1; break;
                    case 1: xMove = 1; yMove = 0; break;
                    case 2: xMove = 0; yMove = 1; break;
                    case 3: xMove = -1; yMove = 0; break;
                }
            }

            return $"{Math.Abs(x) + Math.Abs(y)}";
        }

        [Description("What is the Manhattan distance between that location and the ship's starting position?")]
        public override string SolvePart2() {
            int direction = 1;
            int x = 0;
            int y = 0;
            int wx = 10;
            int wy = -1;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int amount = item.Substring(1).ToInt();
                int lastDirection = direction;
                switch (item[0]) {
                    case 'N': wy -= amount; break;
                    case 'S': wy += amount; break;
                    case 'E': wx += amount; break;
                    case 'W': wx -= amount; break;
                    case 'L':
                        direction -= amount / 90;
                        if (direction < 0) { direction += 4; }
                        break;
                    case 'R':
                        direction += amount / 90;
                        if (direction >= 4) { direction -= 4; }
                        break;
                    case 'F': x += wx * amount; y += wy * amount; break;
                }

                lastDirection = (lastDirection == 0 ? 4 : lastDirection) - direction;
                if (lastDirection < 0) { lastDirection += 4; }

                int temp;
                switch (lastDirection) {
                    case 1: temp = wx; wx = wy; wy = -temp; break;
                    case 2: wx = -wx; wy = -wy; break;
                    case 3: temp = wx; wx = -wy; wy = temp; break;
                }
            }

            return $"{Math.Abs(x) + Math.Abs(y)}";
        }
    }
}