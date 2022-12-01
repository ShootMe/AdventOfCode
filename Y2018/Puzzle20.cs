using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("A Regular Map")]
    public class Puzzle20 : ASolver {
        private Dictionary<Point, MapType> map;

        public override void Setup() {
            map = new Dictionary<Point, MapType>();
            map[Point.ZERO] = MapType.Room;
            Traverse();
        }
        private void Traverse() {
            Stack<Point> points = new Stack<Point>();
            Point pos = new Point();
            int index = 0;
            while (++index < Input.Length) {
                char path = Input[index];
                switch (path) {
                    case '(': points.Push(pos); break;
                    case ')': pos = points.Pop(); break;
                    case '|': pos = points.Peek(); break;
                    case 'N': MoveInMap(ref pos, Point.NORTH); break;
                    case 'S': MoveInMap(ref pos, Point.SOUTH); break;
                    case 'E': MoveInMap(ref pos, Point.EAST); break;
                    case 'W': MoveInMap(ref pos, Point.WEST); break;
                }
            }
        }
        private void MoveInMap(ref Point point, Point direction) {
            point += direction;
            map[point] = MapType.Door;
            point += direction;
            map[point] = MapType.Room;
        }

        [Description("What is the largest number of doors you would be required to pass through to reach a room?")]
        public override string SolvePart1() {
            return $"{Solve().farthest}";
        }

        [Description("How many rooms have a path from your current location that pass through 1000 doors?")]
        public override string SolvePart2() {
            return $"{Solve().roomsPast1000}";
        }
        private (int farthest, int roomsPast1000) Solve() {
            Point[] dirs = new Point[] { Point.NORTH, Point.SOUTH, Point.EAST, Point.WEST };
            HashSet<Point> closed = new HashSet<Point>();
            Queue<(Point, int)> open = new Queue<(Point, int)>();
            Point pos = new Point();
            closed.Add(pos);
            open.Enqueue((pos, 0));

            int farthest = 0;
            int roomsPast1000 = 0;
            while (open.Count > 0) {
                (pos, int steps) = open.Dequeue();

                if (steps > farthest) {
                    farthest = steps;
                }
                if (steps >= 1000) {
                    roomsPast1000++;
                }

                for (int i = 0; i < dirs.Length; i++) {
                    Point dir = dirs[i];
                    pos += dir;
                    if (map.TryGetValue(pos, out MapType type) && type == MapType.Door) {
                        pos += dir;
                        if (closed.Add(pos)) {
                            open.Enqueue((pos, steps + 1));
                        }
                        pos -= dir;
                    }
                    pos -= dir;
                }
            }

            return (farthest, roomsPast1000);
        }
        private enum MapType : byte {
            Room,
            Door
        }
    }
}