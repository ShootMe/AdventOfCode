using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle14 : ASolver {
        private Reindeer[] reindeers;
        public Puzzle14(string input) : base(input) { Name = "Reindeer Olympics"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            reindeers = new Reindeer[items.Count];

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int velocity = Tools.ParseInt(item, " can fly ", " km/s ");
                int length = Tools.ParseInt(item, " km/s for ", " seconds");
                int rest = Tools.ParseInt(item, " rest for ", " seconds");

                Reindeer reindeer = new Reindeer() { Velocity = velocity, RestLength = rest, VelocityLength = length };
                reindeers[i] = reindeer;
            }
        }

        [Description("What distance has the winning reindeer traveled?")]
        public override string SolvePart1() {
            for (int j = 0; j < 2503; j++) {
                for (int i = 0; i < reindeers.Length; i++) {
                    reindeers[i].Advance();
                }
                AwardPoints();
            }

            return $"{AwardPoints()}";
        }

        [Description("How many points does the winning reindeer have?")]
        public override string SolvePart2() {
            int max = 0;
            for (int i = 0; i < reindeers.Length; i++) {
                if (reindeers[i].Points > max) {
                    max = reindeers[i].Points;
                }
            }
            return $"{max}";
        }

        private int AwardPoints() {
            int max = 0;
            for (int i = 0; i < reindeers.Length; i++) {
                if (reindeers[i].DistanceTraveled > max) {
                    max = reindeers[i].DistanceTraveled;
                }
            }
            for (int i = 0; i < reindeers.Length; i++) {
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