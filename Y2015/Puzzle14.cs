using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Reindeer Olympics")]
    public class Puzzle14 : ASolver {
        private List<Reindeer> reindeers = new();

        public override void Setup() {
            foreach (string item in Input.Split('\n')) {
                string[] splits = item.SplitOn(" can fly ", " km/s for ", " seconds", " rest for ", " seconds");
                int velocity = splits[1].ToInt();
                int length = splits[2].ToInt();
                int rest = splits[4].ToInt();

                reindeers.Add(new Reindeer() { Velocity = velocity, RestLength = rest, VelocityLength = length });
            }
        }

        [Description("What distance has the winning reindeer traveled?")]
        public override string SolvePart1() {
            for (int j = 0; j < 2503; j++) {
                for (int i = 0; i < reindeers.Count; i++) {
                    reindeers[i].Advance();
                }
                AwardPoints();
            }

            return $"{AwardPoints()}";
        }

        [Description("How many points does the winning reindeer have?")]
        public override string SolvePart2() {
            int max = 0;
            for (int i = 0; i < reindeers.Count; i++) {
                if (reindeers[i].Points > max) {
                    max = reindeers[i].Points;
                }
            }
            return $"{max}";
        }

        private int AwardPoints() {
            int max = 0;
            for (int i = 0; i < reindeers.Count; i++) {
                if (reindeers[i].DistanceTraveled > max) {
                    max = reindeers[i].DistanceTraveled;
                }
            }
            for (int i = 0; i < reindeers.Count; i++) {
                if (reindeers[i].DistanceTraveled == max) {
                    reindeers[i].Points++;
                }
            }
            return max;
        }

        private class Reindeer {
            public int Velocity;
            public int VelocityLength;
            public int RestLength;
            public int DistanceTraveled;
            public int VelocityTime;
            public int RestTime;
            public int Points;

            public void Clear() {
                DistanceTraveled = 0;
                VelocityTime = 0;
                RestTime = 0;
                Points = 0;
            }
            public void Advance() {
                if (VelocityTime < VelocityLength) {
                    VelocityTime++;
                    DistanceTraveled += Velocity;
                } else if (RestTime < RestLength) {
                    RestTime++;
                    if (RestTime == RestLength) {
                        RestTime = 0;
                        VelocityTime = 0;
                    }
                }
            }
            public override string ToString() {
                return $"{Velocity} {VelocityLength} {RestLength} = {DistanceTraveled} {Points}";
            }
        }
    }
}