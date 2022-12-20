using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Not Enough Minerals")]
    public class Puzzle19 : ASolver {
        private List<Blueprint> blueprints = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                blueprints.Add(line);
            }
        }

        [Description("What do you get if you add up the quality level of all of the blueprints in your list?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < blueprints.Count; i++) {
                total += blueprints[i].Value(24) * (i + 1);
            }
            return $"{total}";
        }

        [Description("What do you get if you multiply these numbers together?")]
        public override string SolvePart2() {
            int total = 1;
            for (int i = 0; i < blueprints.Count && i < 3; i++) {
                total *= blueprints[i].Value(32);
            }
            return $"{total}";
        }

        private class Blueprint {
            public int ID;
            public List<Robot> Robots = new();

            public int Value(int minutes) {
                Queue<Simulation> open = new();
                Dictionary<Simulation, (byte minutes, Resources rocks)> closed = new();
                open.Enqueue(new Simulation() { Minutes = 0, OreBots = 1 });
                int bestValue = -1;
                int maxOre = 0; int clayOre = 0;
                int obsidianOre = 0; int obsidianClay = 0;
                int geodeOre = 0; int geodeObsidian = 0;
                for (int i = 0; i < Robots.Count; i++) {
                    Robot robot = Robots[i];
                    if (robot.Ore > maxOre) { maxOre = robot.Ore; }
                    if (robot.RockType == RockType.Geode) {
                        geodeOre = robot.Ore;
                        geodeObsidian = robot.Obsidian;
                    } else if (robot.RockType == RockType.Obsidian) {
                        obsidianOre = robot.Ore;
                        obsidianClay = robot.Clay;
                    } else if (robot.RockType == RockType.Clay) {
                        clayOre = robot.Ore;
                    }
                }

                while (open.Count > 0) {
                    Simulation current = open.Dequeue();

                    if (current.Minutes >= minutes) {
                        int collected = current.Rocks.Geode;
                        if (collected > bestValue) { bestValue = collected; }
                        continue;
                    }

                    int minutesLeft = minutes - current.Minutes - 1;

                    int obsidianBotsToMake = geodeObsidian - current.ObsidianBots;
                    obsidianBotsToMake = obsidianBotsToMake > minutesLeft ? minutesLeft : obsidianBotsToMake;

                    int clayBotsToMake = obsidianClay - current.ClayBots;
                    clayBotsToMake = clayBotsToMake > minutesLeft ? minutesLeft : clayBotsToMake;

                    bool obsidianNotNeeded = current.Rocks.Obsidian + (current.ObsidianBots - geodeObsidian) * minutesLeft >= 0;
                    bool clayNotNeeded = current.Rocks.Clay + (current.ClayBots - obsidianClay) * obsidianBotsToMake >= 0;
                    bool oreNotNeeded = current.Rocks.Ore + current.OreBots * minutesLeft - geodeOre * minutesLeft - obsidianOre * obsidianBotsToMake - clayOre * clayBotsToMake >= 0;

                    bool skipOre = current.OreBots >= maxOre || oreNotNeeded;
                    bool skipObsidian = obsidianNotNeeded;
                    bool skipClay = obsidianNotNeeded || clayNotNeeded;

                    for (int i = 0; i < Robots.Count; i++) {
                        Robot robot = Robots[i];

                        if (robot.RockType == RockType.Ore && skipOre) { continue; }
                        if (robot.RockType == RockType.Clay && skipClay) { continue; }
                        if (robot.RockType == RockType.Obsidian && skipObsidian) { continue; }

                        if (current.Rocks.Ore >= robot.Ore && current.Rocks.Clay >= robot.Clay && current.Rocks.Obsidian >= robot.Obsidian) {
                            Simulation next = new Simulation(current);
                            next.AddCollected();
                            next.Build(robot);
                            if (TryAdd(closed, next)) {
                                closed[next] = (next.Minutes, next.Rocks);
                                open.Enqueue(next);
                            }
                        }
                    }

                    current.Minutes++;
                    current.AddCollected();
                    open.Enqueue(current);
                }
                //Console.WriteLine($"{minutes}={bestValue}");
                return bestValue;
            }
            private bool TryAdd(Dictionary<Simulation, (byte minutes, Resources rocks)> closed, Simulation simulation) {
                return !closed.TryGetValue(simulation, out var value) || value.minutes > simulation.Minutes || (value.minutes == simulation.Minutes && value.rocks != simulation.Rocks);
            }
            public static implicit operator Blueprint(string line) {
                Blueprint blueprint = new Blueprint();
                int idIndex = line.IndexOf(':');
                blueprint.ID = line[10..idIndex].ToInt();
                string[] robots = line.Split(" Each ");
                for (int i = 1; i < robots.Length; i++) {
                    Robot robot = robots[i];
                    blueprint.Robots.Add(robot);
                }
                return blueprint;
            }
        }
        private struct Simulation : IEquatable<Simulation> {
            public byte OreBots, ClayBots, ObsidianBots, GeodeBots;
            public Resources Rocks;
            public byte Minutes;
            public Simulation() { }
            public Simulation(Simulation copy) {
                OreBots = copy.OreBots;
                ClayBots = copy.ClayBots;
                ObsidianBots = copy.ObsidianBots;
                GeodeBots = copy.GeodeBots;
                Rocks = copy.Rocks;
                Minutes = (byte)(copy.Minutes + 1);
            }
            public void Build(Robot robot) {
                Rocks.Ore -= robot.Ore;
                Rocks.Clay -= robot.Clay;
                Rocks.Obsidian -= robot.Obsidian;
                switch (robot.RockType) {
                    case RockType.Ore: OreBots++; break;
                    case RockType.Clay: ClayBots++; break;
                    case RockType.Obsidian: ObsidianBots++; break;
                    case RockType.Geode: GeodeBots++; break;
                }
            }
            public void AddCollected() {
                Rocks.Ore += OreBots;
                Rocks.Clay += ClayBots;
                Rocks.Obsidian += ObsidianBots;
                Rocks.Geode += GeodeBots;
            }
            public bool Equals(Simulation other) {
                return OreBots == other.OreBots && ClayBots == other.ClayBots && ObsidianBots == other.ObsidianBots && GeodeBots == other.GeodeBots;
            }
            public override int GetHashCode() {
                return OreBots + (ClayBots << 8) + (ObsidianBots << 16) + (GeodeBots << 24);
            }
            public override string ToString() {
                return $"{Minutes,2}=[{OreBots,2},{ClayBots,2},{ObsidianBots,2},{GeodeBots,2}]={Rocks}";
            }
        }
        private struct Resources {
            public short Ore, Clay, Obsidian, Geode;
            public static bool operator ==(Resources left, Resources right) {
                return left.Ore == right.Ore && left.Clay == right.Clay && left.Obsidian == right.Obsidian && left.Geode == right.Geode;
            }
            public static bool operator !=(Resources left, Resources right) {
                return left.Ore != right.Ore || left.Clay != right.Clay || left.Obsidian != right.Obsidian || left.Geode != right.Geode;
            }
            public override string ToString() {
                return $"[{Ore,3},{Clay,3},{Obsidian,3},{Geode,3}]";
            }
        }
        private class Robot {
            public RockType RockType;
            public short Ore, Clay, Obsidian;

            public static implicit operator Robot(string line) {
                Robot robot = new Robot();
                string[] splits = line.SplitOn(" robot costs ");
                robot.RockType = (RockType)Enum.Parse(typeof(RockType), splits[0], true);
                int amountIndex = splits[1].IndexOf(' ');
                int typeIndex = splits[1].IndexOf(" and ");
                if (typeIndex < 0) { typeIndex = splits[1].IndexOf('.'); }
                robot.AddCost(splits[1][(amountIndex + 1)..typeIndex], splits[1][..amountIndex].ToInt());
                if (typeIndex < splits[1].Length - 1) {
                    amountIndex = splits[1].IndexOf(' ', typeIndex + 5);
                    robot.AddCost(splits[1][(amountIndex + 1)..^1], splits[1][(typeIndex + 5)..amountIndex].ToInt());
                }
                return robot;
            }
            private void AddCost(string type, int amount) {
                switch (type) {
                    case "ore": Ore += (short)amount; break;
                    case "clay": Clay += (short)amount; break;
                    case "obsidian": Obsidian += (short)amount; break;
                }
            }
            public override string ToString() {
                return $"R({RockType}) C({Ore},{Clay},{Obsidian})";
            }
        }
        private enum RockType : int {
            Ore,
            Clay,
            Obsidian,
            Geode
        }
    }
}