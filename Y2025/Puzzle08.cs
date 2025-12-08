using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Playground")]
    public class Puzzle08 : ASolver {
        private List<Box> boxes = new();
        private Heap<BoxPair> distances = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                string[] splits = line.Split(',');
                boxes.Add(new Box(splits[0].ToInt(), splits[1].ToInt(), splits[2].ToInt(), boxes.Count));
            }

            Heap<BoxPair> heap = new();
            for (int i = 0; i < boxes.Count; i++) {
                Box box1 = boxes[i];

                for (int j = i + 1; j < boxes.Count; j++) {
                    Box box2 = boxes[j];

                    long dist = box1.Dist(box2);
                    distances.Enqueue(new BoxPair(box1, box2, dist));
                }
            }
        }

        [Description("What do you get if you multiply together the sizes of the three largest circuits?")]
        public override string SolvePart1() {
            int[] counts = new int[boxes.Count];
            Array.Fill(counts, 1);
            int amountToConnect = boxes.Count < 40 ? 10 : 1000;
            for (int i = 0; i < amountToConnect; i++) {
                BoxPair pair = distances.Dequeue();
                Box box1 = pair.Box1; Box box2 = pair.Box2;
                if (box1.Circuit == box2.Circuit) { continue; }

                int cMove = box2.Circuit;
                counts[box1.Circuit] += counts[cMove];
                counts[cMove] = 0;
                for (int j = 0; j < boxes.Count; j++) {
                    Box box = boxes[j];
                    if (box.Circuit == cMove) {
                        box.Circuit = box1.Circuit;
                    }
                }
            }

            Array.Sort(counts);
            int total = 1;
            for (int i = 0; i < 3; i++) {
                int amt = counts[boxes.Count - 1 - i];
                total *= amt;
            }
            return $"{total}";
        }

        [Description("What do you get if you multiply together the X coordinates of the last two junction boxes you need to connect?")]
        public override string SolvePart2() {
            int[] counts = new int[boxes.Count];
            for (int i = 0; i < boxes.Count; i++) {
                counts[boxes[i].Circuit]++;
            }

            while (true) {
                BoxPair pair = distances.Dequeue();
                Box box1 = pair.Box1; Box box2 = pair.Box2;
                if (box1.Circuit == box2.Circuit) { continue; }

                int cMove = box2.Circuit;
                counts[box1.Circuit] += counts[box2.Circuit];
                counts[box2.Circuit] = 0;
                for (int j = 0; j < boxes.Count; j++) {
                    Box box = boxes[j];
                    if (box.Circuit == cMove) {
                        box.Circuit = box1.Circuit;
                    }
                }
                if (counts[box1.Circuit] == boxes.Count) {
                    return $"{(long)box1.X * box2.X}";
                }
            }
        }
        private struct BoxPair : IComparable<BoxPair> {
            public Box Box1, Box2;
            public long Dist;
            public BoxPair(Box box1, Box box2, long dist) {
                Box1 = box1; Box2 = box2; Dist = dist;
            }
            public int CompareTo(BoxPair other) {
                return Dist.CompareTo(other.Dist);
            }
        }
        private class Box {
            public int X, Y, Z, Circuit;
            public Box(int x, int y, int z, int c) {
                X = x; Y = y; Z = z; Circuit = c;
            }
            public long Dist(Box box) {
                return (long)(X - box.X) * (X - box.X) + (long)(Y - box.Y) * (Y - box.Y) + (long)(Z - box.Z) * (Z - box.Z);
            }
            public override string ToString() {
                return $"({X,5},{Y,5},{Z,5})={Circuit,3}";
            }
        }
    }
}