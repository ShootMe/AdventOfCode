using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Restroom Redoubt")]
    public class Puzzle14 : ASolver {
        private List<Robot> robots = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                robots.Add(new Robot(line));
            }
        }

        [Description("What will the safety factor be after exactly 100 seconds have elapsed?")]
        public override string SolvePart1() {
            int maxX = robots.Count < 20 ? 11 : 101, maxY = robots.Count < 20 ? 7 : 103;
            for (int j = 0; j < 100; j++) {
                for (int i = 0; i < robots.Count; i++) {
                    Robot robot = robots[i];
                    robot.Update(maxX, maxY);
                }
            }

            int total1 = 0, total2 = 0, total3 = 0, total4 = 0;
            for (int i = 0; i < robots.Count; i++) {
                Robot robot = robots[i];
                if (robot.X > maxX / 2 && robot.Y < maxY / 2) { total1++; }
                if (robot.X > maxX / 2 && robot.Y > maxY / 2) { total2++; }
                if (robot.X < maxX / 2 && robot.Y < maxY / 2) { total4++; }
                if (robot.X < maxX / 2 && robot.Y > maxY / 2) { total3++; }
            }
            return $"{total1 * total2 * total3 * total4}";
        }

        [Description("What is the fewest number of seconds that must elapse for the robots to display the Easter egg?")]
        public override string SolvePart2() {
            int maxX = robots.Count < 20 ? 11 : 101, maxY = robots.Count < 20 ? 7 : 103;

            //void Print() {
            //    Console.WriteLine();
            //    for (int y = 0; y < maxY; y++) {
            //        for (int x = 0; x < maxX; x++) {
            //            bool printed = false;
            //            for (int i = 0; i < robots.Count; i++) {
            //                Robot robot = robots[i];
            //                if (robot.X == x && robot.Y == y) {
            //                    Console.Write('#');
            //                    printed = true;
            //                    break;
            //                }
            //            }
            //            if (!printed) {
            //                Console.Write(' ');
            //            }
            //        }
            //        Console.WriteLine();
            //    }
            //}

            HashSet<(int, int)> occ = new();
            for (int j = 0; j < 100000; j++) {
                occ.Clear();
                for (int i = 0; i < robots.Count; i++) {
                    Robot robot = robots[i];
                    robot.Update(maxX, maxY);
                    occ.Add((robot.X, robot.Y));
                }

                bool possible = occ.Count == robots.Count;
                if (possible) {
                    //Print();
                    return $"{j + 101}";
                }
            }
            return "";
        }
        private class Robot {
            public int X, Y, VX, VY;
            public Robot(string data) {
                string[] splits = data.SplitOn("p=", ",", " v=", ",");
                X = splits[1].ToInt(); Y = splits[2].ToInt();
                VX = splits[3].ToInt(); VY = splits[4].ToInt();
            }
            public void Update(int maxX, int maxY) {
                X += VX; Y += VY;
                if (X < 0) { X += maxX; } else if (X >= maxX) { X -= maxX; }
                if (Y < 0) { Y += maxY; } else if (Y >= maxY) { Y -= maxY; }
            }
        }
    }
}