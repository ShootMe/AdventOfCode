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

            string timeStr = string.Empty;
            string distanceStr = string.Empty;
            for (int i = 0; i < boatTimes.Length; i++) {
                times.Add(boatTimes[i].ToInt());
                distances.Add(boatDistances[i].ToInt());
                timeStr += boatTimes[i];
                distanceStr += boatDistances[i];
            }

            timeAll = timeStr.ToLong();
            distanceAll = distanceStr.ToLong();
        }

        [Description("Determine the number of ways you could beat the record in each race. What do you get if you multiply these numbers together?")]
        public override string SolvePart1() {
            int total = 1;
            for (int i = 0; i < times.Count; i++) {
                int time = times[i];
                int distance = distances[i];
                int timesBeat = 0;
                for (int j = 1; j < time; j++) {
                    if ((time - j) * j > distance) {
                        timesBeat++;
                    }
                }
                total *= timesBeat;
            }
            return $"{total}";
        }

        [Description("How many ways can you beat the record in this one much longer race?")]
        public override string SolvePart2() {
            int div = (int)(distanceAll / timeAll);
            int startTime = 0;
            for (int i = div; i < timeAll; i++) {
                if ((timeAll - i) * i > distanceAll) {
                    startTime = i;
                    break;
                }
            }

            int endTime = 0;
            for (int i = (int)timeAll - div; i > startTime; i--) {
                if ((timeAll - i) * i > distanceAll) {
                    endTime = i;
                    break;
                }
            }

            return $"{endTime - startTime + 1}";
        }
    }
}