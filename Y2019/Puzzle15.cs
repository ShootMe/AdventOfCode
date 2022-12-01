using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Oxygen System")]
    public class Puzzle15 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Tools.GetLongs(Input, ','));
        }

        [Description("What is the fewest number of moves required to get to the location of the oxygen system?")]
        public override string SolvePart1() {
            return $"{RunMaze()}";
        }

        [Description("How many minutes will it take to fill with oxygen?")]
        public override string SolvePart2() {
            return $"{RunMaze(true)}";
        }

        private int RunMaze(bool exploreAll = false) {
            Dictionary<Point, int> maze = new Dictionary<Point, int>();
            List<Robot> robots = new List<Robot>();
            robots.Add(new Robot() { Program = program, Direction = 1, Position = new Point() { X = 0, Y = 0 }, Steps = 0 });
            program.Reset();

            maze.Add(robots[0].Position, 0);
            int[] availableDir = new int[4];
            bool hitGoal = false;
            int maxSteps = 0;
            while (robots.Count > 0) {
                for (int i = robots.Count - 1; i >= 0; i--) {
                    Robot robot = robots[i];
                    robot.Program.Run();

                    if (robot.Program.InputRequired) {
                        if (!robot.HasNextInput) {
                            int availableCount = 0;
                            if (!maze.ContainsKey(GetNextPoint(robot.Position, 1))) {
                                availableDir[availableCount++] = 1;
                            }
                            if (!maze.ContainsKey(GetNextPoint(robot.Position, 2))) {
                                availableDir[availableCount++] = 2;
                            }
                            if (!maze.ContainsKey(GetNextPoint(robot.Position, 3))) {
                                availableDir[availableCount++] = 3;
                            }
                            if (!maze.ContainsKey(GetNextPoint(robot.Position, 4))) {
                                availableDir[availableCount++] = 4;
                            }

                            if (availableCount == 0) {
                                robots.RemoveAt(i);
                                if (robots.Count == 0) {
                                    return maxSteps;
                                }
                                continue;
                            }

                            for (int j = 1; j < availableCount; j++) {
                                Robot robotToAdd = new Robot() { Position = robot.Position, Direction = availableDir[j], Program = robot.Program.Clone(), HasNextInput = true, Steps = robot.Steps };
                                robots.Add(robotToAdd);
                                maze.Add(GetNextPoint(robotToAdd.Position, robotToAdd.Direction), -1);
                            }

                            robot.Direction = availableDir[0];
                        }

                        robot.Steps++;
                        robot.Program.Run(robot.Direction);
                        robot.HasNextInput = false;
                    }

                    int out1 = (int)robot.Program.Output;
                    switch (out1) {
                        case 0:
                            Point wall = GetNextPoint(robot.Position, robot.Direction);
                            maze[wall] = 1;
                            robots.RemoveAt(i);
                            if (robots.Count == 0) {
                                return maxSteps;
                            }
                            continue;
                        case 1:
                        case 2: robot.Position = GetNextPoint(robot.Position, robot.Direction); break;
                    }

                    maze[robot.Position] = out1 == 2 ? 5 : 0;

                    if (robot.Steps > maxSteps) {
                        maxSteps = robot.Steps;
                        //Display(maze, robot.Position);
                    }

                    if (out1 == 2) {
                        if (!exploreAll) { return robot.Steps; }

                        robots.RemoveAt(i);
                        if (!hitGoal) {
                            maze.Clear();
                            robots.Clear();
                            robot.Steps = 0;
                            robots.Add(robot);
                            hitGoal = true;
                            maze.Add(robot.Position, 5);
                            maxSteps = 0;
                        }
                        break;
                    }
                }
            }

            return 0;
        }
        private Point GetNextPoint(Point point, int dir) {
            Point next = new Point() { X = point.X, Y = point.Y };
            switch (dir) {
                case 1: next.Y--; break;
                case 2: next.Y++; break;
                case 3: next.X--; break;
                case 4: next.X++; break;
            }
            return next;
        }
        private void Display(Dictionary<Point, int> maze, Point current) {
            Point min = new Point() { X = int.MaxValue, Y = int.MaxValue };
            Point max = new Point() { X = int.MinValue, Y = int.MinValue };
            foreach (KeyValuePair<Point, int> pair in maze) {
                Point point = pair.Key;
                if (point.X < min.X) { min.X = point.X; }
                if (point.Y < min.Y) { min.Y = point.Y; }
                if (point.X > max.X) { max.X = point.X; }
                if (point.Y > max.Y) { max.Y = point.Y; }
            }

            int width = max.X - min.X + 1;
            int height = max.Y - min.Y + 1;
            int[] grid = new int[width * height];
            Array.Fill(grid, -1);
            foreach (KeyValuePair<Point, int> pair in maze) {
                Point point = pair.Key;
                grid[(point.Y - min.Y) * width + point.X - min.X] = point.X == 0 && point.Y == 0 ? 4 : point.X == current.X && point.Y == current.Y ? 6 : pair.Value;
            }

            Console.WriteLine();
            for (int i = 0, k = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    int value = grid[k++];
                    Console.Write(value == -1 ? '?' : value == 0 ? ' ' : value == 1 ? '!' : value == 4 ? 'S' : value == 6 ? 'O' : 'X');
                }
                Console.WriteLine();
            }
        }

        private class Robot {
            public IntCode Program;
            public Point Position;
            public int Direction;
            public bool HasNextInput;
            public int Steps;

            public override string ToString() {
                return $"{Position} {Direction} {Steps} {HasNextInput}";
            }
        }
    }
}