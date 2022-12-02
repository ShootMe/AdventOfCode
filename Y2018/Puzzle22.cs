using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Mode Maze")]
    public class Puzzle22 : ASolver {
        private readonly Dictionary<(int, int), int> mapValues = new Dictionary<(int, int), int>();
        private int depth, targetX, targetY;

        public override void Setup() {
            int indexDepth = Input.IndexOf("depth: ");
            int indexTarget = Input.IndexOf("target: ");
            depth = Input.Substring(indexDepth + 7, indexTarget - indexDepth - 7).ToInt();

            int indexY = Input.IndexOf(',', indexTarget);
            targetX = Input.Substring(indexTarget + 8, indexY - indexTarget - 8).ToInt();
            targetY = Input.Substring(indexY + 1).ToInt();
        }

        private int ErosionLevel(int x, int y) {
            if (mapValues.TryGetValue((x, y), out int val)) {
                return val;
            }

            if (x == targetX && y == targetY) {
                val = 0;
            } else if (y == 0) {
                val = x * 16807;
            } else if (x == 0) {
                val = y * 48271;
            } else {
                val = ErosionLevel(x - 1, y) * ErosionLevel(x, y - 1);
            }

            val = (val + depth) % 20183;
            mapValues.Add((x, y), val);
            return val;
        }

        [Description("What is the total risk level for the smallest rectangle?")]
        public override string SolvePart1() {
            int total = 0;
            for (int y = 0; y <= targetY; y++) {
                for (int x = 0; x <= targetX; x++) {
                    total += ErosionLevel(x, y) % 3;
                }
            }
            return $"{total}";
        }

        [Description("What is the fewest number of minutes you can take to reach the target?")]
        public override string SolvePart2() {
            (int, int)[] dirs = { (0, 1), (1, 0), (-1, 0), (0, -1) };
            Queue<(int, int, Tool, int, int)> queue = new Queue<(int, int, Tool, int, int)>();
            HashSet<(Tool, int, int)> seen = new HashSet<(Tool, int, int)>();
            queue.Enqueue((0, 0, Tool.Torch, 0, 0));
            seen.Add((Tool.Torch, 0, 0));

            while (queue.Count > 0) {
                (int X, int Y, Tool Tool, int Time, int ChangeTime) current = queue.Dequeue();

                if (current.ChangeTime > 0) {
                    if (current.ChangeTime != 1 || seen.Add((current.Tool, current.X, current.Y))) {
                        current.Time++;
                        current.ChangeTime--;
                        queue.Enqueue(current);
                    }
                    continue;
                }

                if (current.Tool == Tool.Torch && current.X == targetX && current.Y == targetY) {
                    return $"{current.Time}";
                }

                foreach ((int x, int y) in dirs) {
                    int tX = current.X + x; int tY = current.Y + y;
                    if (tX < 0 || tY < 0) { continue; }

                    int region = ErosionLevel(tX, tY) % 3;
                    bool allowed = ((region == 0 || region == 2) && current.Tool == Tool.Torch) ||
                        ((region == 0 || region == 1) && current.Tool == Tool.Gear) ||
                        ((region == 1 || region == 2) && current.Tool == Tool.None);

                    if (allowed && seen.Add((current.Tool, tX, tY))) {
                        queue.Enqueue((tX, tY, current.Tool, current.Time + 1, 0));
                    }
                }

                current.Time++;
                current.ChangeTime = 6;
                int thisRegion = ErosionLevel(current.X, current.Y) % 3;
                switch (thisRegion) {
                    case 0:
                        current.Tool = current.Tool == Tool.Gear ? Tool.Torch : Tool.Gear;
                        queue.Enqueue(current);
                        break;
                    case 1:
                        current.Tool = current.Tool == Tool.Gear ? Tool.None : Tool.Gear;
                        queue.Enqueue(current);
                        break;
                    default:
                        current.Tool = current.Tool == Tool.Torch ? Tool.None : Tool.Torch;
                        queue.Enqueue(current);
                        break;
                }
            }

            return string.Empty;
        }
        private enum Tool : byte {
            None,
            Torch,
            Gear
        }
    }
}