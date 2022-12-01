using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Experimental Emergency Teleportation")]
    public class Puzzle23 : ASolver {
        private List<Nanobot> bots;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            bots = new List<Nanobot>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                bots.Add(new Nanobot(item));
            }
            bots.Sort();
        }

        [Description("How many nanobots are in range of its signals?")]
        public override string SolvePart1() {
            return $"{CountInRange(bots[0])}";
        }
        private int CountInRange(Nanobot bot) {
            int total = 0;
            for (int i = 0; i < bots.Count; i++) {
                if (bots[i].Distance(bot) <= bot.R) {
                    total++;
                }
            }
            return total;
        }

        [Description("What is the shortest manhattan distance between any of those points?")]
        public override string SolvePart2() {
            int minX = int.MaxValue; int maxX = int.MinValue;
            int minY = int.MaxValue; int maxY = int.MinValue;
            int minZ = int.MaxValue; int maxZ = int.MinValue;

            for (int i = 0; i < bots.Count; i++) {
                Nanobot bot = bots[i];
                if (bot.X > maxX) { maxX = bot.X; }
                if (bot.Y > maxY) { maxY = bot.Y; }
                if (bot.Z > maxZ) { maxZ = bot.Z; }
                if (bot.X < minX) { minX = bot.X; }
                if (bot.Y < minY) { minY = bot.Y; }
                if (bot.Z < minZ) { minZ = bot.Z; }
            }

            int xRange = maxX - minX, yRange = maxY - minY, zRange = maxZ - minZ;
            int increaseX = xRange >> 2; int increaseY = yRange >> 2; int increaseZ = zRange >> 2;
            int bestX = 0; int bestY = 0; int bestZ = 0;

            while (true) {
                int maxInRange = 0;
                int bestSum = int.MaxValue;
                Nanobot testBot = new Nanobot();

                for (int x = minX; x <= maxX; x += increaseX) {
                    int absX = Math.Abs(x);
                    testBot.X = x;
                    for (int y = minY; y <= maxY; y += increaseY) {
                        int absY = Math.Abs(y);
                        testBot.Y = y;
                        for (int z = minZ; z <= maxZ; z += increaseZ) {
                            int absZ = Math.Abs(z);
                            testBot.Z = z;

                            int inRange = 0;
                            for (int i = 0; i < bots.Count; i++) {
                                Nanobot bot = bots[i];
                                if (testBot.Distance(bot) <= bot.R) {
                                    inRange++;
                                }
                            }

                            if (inRange > maxInRange || (inRange == maxInRange && absX + absY + absZ < bestSum)) {
                                maxInRange = inRange;
                                bestX = x; bestY = y; bestZ = z;
                                bestSum = absX + absY + absZ;
                            }
                        }
                    }
                }

                //Console.WriteLine($"Inc {increaseX} Location: {bestX},{bestY},{bestZ} InRange: {maxInRange} Sum: {bestSum}");
                minX = bestX - increaseX; minY = bestY - increaseY; minZ = bestZ - increaseZ;
                maxX = bestX + increaseX; maxY = bestY + increaseY; maxZ = bestZ + increaseZ;

                if (increaseX + increaseY + increaseZ == 3) { return $"{bestSum}"; }
                if (increaseX > 1) { increaseX >>= 1; }
                if (increaseY > 1) { increaseY >>= 1; }
                if (increaseZ > 1) { increaseZ >>= 1; }
            }
        }

        private struct Nanobot : IComparable<Nanobot> {
            public int X, Y, Z;
            public int R;

            public Nanobot(string value) {
                int index1 = value.IndexOf(',');
                X = Tools.ParseInt(value, 5, index1 - 5);

                int index2 = value.IndexOf(',', index1 + 1);
                Y = Tools.ParseInt(value, index1 + 1, index2 - index1 - 1);

                index1 = value.IndexOf('>', index2);
                Z = Tools.ParseInt(value, index2 + 1, index1 - index2 - 1);

                index2 = value.IndexOf('=', index1 + 3);
                R = Tools.ParseInt(value, index2 + 1);
            }

            public int CompareTo(Nanobot other) {
                return other.R.CompareTo(R);
            }
            public int Distance(Nanobot particle) {
                return Math.Abs(X - particle.X) + Math.Abs(Y - particle.Y) + Math.Abs(Z - particle.Z);
            }
            public override string ToString() {
                return $"({X},{Y},{Z})({R})";
            }
        }
    }
}