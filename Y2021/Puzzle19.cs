using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Beacon Scanner")]
    public class Puzzle19 : ASolver {
        private List<Scanner> scanners;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            scanners = new List<Scanner>();
            for (int i = 0; i < items.Count;) {
                string line = items[i++];
                string[] splits = Tools.SplitOn(line, "--- scanner ", " ---");
                Scanner scanner = new Scanner(Tools.ParseInt(splits[1]));

                while (i < items.Count && !string.IsNullOrEmpty(line = items[i++])) {
                    splits = Tools.SplitOn(line, ",", ",");
                    scanner.Add(Tools.ParseInt(splits[0]), Tools.ParseInt(splits[1]), Tools.ParseInt(splits[2]));
                }
                scanners.Add(scanner);
                scanner.SetDistances();
            }
        }

        [Description("How many beacons are there?")]
        public override string SolvePart1() {
            Queue<Scanner> toTry = new Queue<Scanner>();
            toTry.Enqueue(scanners[0]);
            scanners[0].Aligned = new HashSet<Beacon>(scanners[0].Beacons[0]);

            while (toTry.Count > 0) {
                Scanner scanner = toTry.Dequeue();

                int index;
                while ((index = Align(scanner)) >= 0) {
                    toTry.Enqueue(scanners[index]);
                }
            }

            HashSet<Beacon> beacons = new HashSet<Beacon>();
            for (int j = 0; j < scanners.Count; j++) {
                Scanner scanner = scanners[j];
                beacons.UnionWith(scanner.Aligned);
            }
            return $"{beacons.Count}";
        }

        [Description("What is the largest Manhattan distance between any two scanners?")]
        public override string SolvePart2() {
            int maxDist = 0;
            for (int i = 0; i < scanners.Count; i++) {
                Scanner scanner1 = scanners[i];

                for (int j = i + 1; j < scanners.Count; j++) {
                    Scanner scanner2 = scanners[j];

                    int dist = scanner1.Robot - scanner2.Robot;
                    if (dist > maxDist) {
                        maxDist = dist;
                    }
                }
            }
            return $"{maxDist}";
        }

        private int Align(Scanner scanner) {
            for (int j = 0; j < scanners.Count; j++) {
                Scanner scanner2 = scanners[j];
                if (scanner2.ID == scanner.ID || scanner2.Aligned != null) { continue; }

                if (scanner.Compare(scanner2)) {
                    return j;
                }
            }
            return -1;
        }

        private class Scanner {
            public readonly List<Beacon>[] Beacons = new List<Beacon>[24];
            public readonly HashSet<int> Distances = new HashSet<int>();
            public HashSet<Beacon> Aligned;
            public Beacon Robot = new Beacon(0, 0, 0);
            public int ID;

            public Scanner(int id) {
                ID = id;
                for (int i = 0; i < Beacons.Length; i++) {
                    Beacons[i] = new List<Beacon>();
                }
            }

            public void Add(int x, int y, int z) {
                Beacon beacon = new Beacon(x, y, z);
                for (int i = 0; i < 24; i++) {
                    if (i == 12 || i == 16) {
                        beacon.Negate(0);
                        beacon.Negate(2);
                    }

                    Beacons[i].Add(beacon.Copy());

                    //0 0 0 1 0 0 0 1 ...
                    beacon.Rotate(((i & 3) + 1) >> 2);
                }
            }
            public void SetDistances() {
                List<Beacon> beacons = Beacons[0];
                for (int i = 0; i < beacons.Count; i++) {
                    Beacon beacon1 = beacons[i];
                    for (int j = i + 1; j < beacons.Count; j++) {
                        Beacon beacon2 = beacons[j];

                        Distances.Add(beacon1 - beacon2);
                    }
                }
            }
            public bool Compare(Scanner other) {
                HashSet<int> distancesClone = new HashSet<int>(Distances);
                distancesClone.IntersectWith(other.Distances);
                if (distancesClone.Count < 66) { return false; }

                Beacon tester = new Beacon(0, 0, 0);
                foreach (Beacon beacon1Ref in Aligned) {
                    for (int i = 0; i < 24; i++) {
                        List<Beacon> otherBeacons = other.Beacons[i];

                        for (int j = 11; j < otherBeacons.Count; j++) {
                            Beacon beacon2Ref = otherBeacons[j];
                            int count = 0;
                            int index = 0;
                            int x = beacon1Ref.X - beacon2Ref.X;
                            int y = beacon1Ref.Y - beacon2Ref.Y;
                            int z = beacon1Ref.Z - beacon2Ref.Z;

                            foreach (Beacon beacon in otherBeacons) {
                                tester.X = beacon.X + x;
                                tester.Y = beacon.Y + y;
                                tester.Z = beacon.Z + z;

                                if (Aligned.Contains(tester)) { count++; }

                                index++;
                                if (count >= 12 || 12 - count > otherBeacons.Count - index) { break; }
                            }

                            if (count >= 12) {
                                foreach (Beacon beacon in otherBeacons) {
                                    beacon.X += x;
                                    beacon.Y += y;
                                    beacon.Z += z;
                                }
                                other.Aligned = new HashSet<Beacon>(otherBeacons);
                                other.Robot.X += x;
                                other.Robot.Y += y;
                                other.Robot.Z += z;

                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            public override string ToString() {
                return $"{ID}={Aligned.Count}";
            }
        }
        private class Beacon : IEquatable<Beacon> {
            public int X, Y, Z;
            public Beacon(int x, int y, int z) {
                X = x;
                Y = y;
                Z = z;
            }
            public Beacon Copy() {
                return new Beacon(X, Y, Z);
            }
            public void Rotate(int xyz) {
                switch (xyz) {
                    case 0: (Y, Z) = (-Z, Y); break;
                    case 1: (X, Z) = (Z, -X); break;
                    case 2: (X, Y) = (-Y, X); break;
                }
            }
            public void Negate(int xyz) {
                switch (xyz) {
                    case 0: X = -X; break;
                    case 1: Y = -Y; break;
                    case 2: Z = -Z; break;
                }
            }
            public static int operator -(Beacon a, Beacon b) {
                return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
            }
            public bool Equals(Beacon other) {
                return X == other.X && Y == other.Y && Z == other.Z;
            }
            public override int GetHashCode() {
                return ((X * 31) + Y) * 31 + Z;
            }
            public override string ToString() {
                return $"({X,5},{Y,5},{Z,5})";
            }
        }
    }
}