using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Wait For It")]
    public class Puzzle06 : ASolver {
        private List<int> times = new();
        private List<int> distances = new();
        private long timeAll;
        private long distanceAll;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            string[] boatTimes = lines[0].Substring(5).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] boatDistances = lines[1].Substring(9).Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < boatTimes.Length; i++) {
                times.Add(boatTimes[i].ToInt());
                distances.Add(boatDistances[i].ToInt());
            }

            timeAll = lines[0].ToLong();
            distanceAll = lines[1].ToLong();
        }

        [Description("What do you get if you multiply these numbers together?")]
        public override string SolvePart1() {
            int total = 1;
            for (int i = 0; i < times.Count; i++) {
                total *= CalculateWins(times[i], distances[i]);
            }
            return $"{total}";
        }

        [Description("How many ways can you beat the record in this one much longer race?")]
        public override string SolvePart2() {
            return $"{CalculateWins(timeAll, distanceAll)}";
        }

        public int CalculateWins(long time, long distance) {
            int maxVal = (int)Math.Ceiling((time + Math.Sqrt(time * time - 4 * distance)) / 2d);
            int minVal = (int)((time - Math.Sqrt(time * time - 4 * distance)) / 2d);
            return maxVal - minVal - 1;
        }
    }
}