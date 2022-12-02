using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Shuttle Search")]
    public class Puzzle13 : ASolver {
        private int[] busIDs;
        private int timestamp;

        public override void Setup() {
            List<string> items = Input.Lines();
            timestamp = Tools.ParseInt(items[0]);
            string[] ids = items[1].Split(',');
            busIDs = new int[ids.Length];
            for (int i = 0; i < ids.Length; i++) {
                if (ids[i] != "x") {
                    busIDs[i] = Tools.ParseInt(ids[i]);
                } else {
                    busIDs[i] = 0;
                }
            }
        }

        [Description("What is the ID of the earliest bus you can take multiplied by the minutes you'll need to wait?")]
        public override string SolvePart1() {
            int minTime = int.MaxValue;
            int minID = 0;
            for (int i = 0; i < busIDs.Length; i++) {
                if (busIDs[i] == 0) { continue; }

                int id = busIDs[i] - (timestamp % busIDs[i]);
                if (id < minTime) {
                    minTime = id;
                    minID = busIDs[i];
                }
            }
            return $"{minID * minTime}";
        }

        [Description("What is the earliest timestamp where the listed bus IDs depart at their offsets in the list?")]
        public override string SolvePart2() {
            long step = busIDs[0];
            long time = 0;
            int busAmount = 1;
            while (busAmount < busIDs.Length) {
                if (busIDs[busAmount] == 0) {
                    busAmount++;
                    continue;
                } else if ((time + busAmount) % busIDs[busAmount] == 0) {
                    step *= busIDs[busAmount];
                    busAmount++;
                    continue;
                }

                time += step;
            }
            return $"{time}";
        }
    }
}