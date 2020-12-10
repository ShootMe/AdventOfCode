using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle09 : ASolver {
        private Dictionary<string, Dictionary<string, int>> distances;
        public Puzzle09(string input) : base(input) { Name = "All in a Single Night"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);

            distances = new Dictionary<string, Dictionary<string, int>>();
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int index1 = item.IndexOf(" to ");
                int index2 = item.IndexOf(" = ", index1);

                string city1 = item.Substring(0, index1);
                string city2 = item.Substring(index1 + 4, index2 - index1 - 4);
                int distance = Tools.ParseInt(item.Substring(index2 + 3));

                Dictionary<string, int> subDistances;
                if (!distances.TryGetValue(city1, out subDistances)) {
                    subDistances = new Dictionary<string, int>();
                    distances.Add(city1, subDistances);
                }
                subDistances.Add(city2, distance);

                if (!distances.TryGetValue(city2, out subDistances)) {
                    subDistances = new Dictionary<string, int>();
                    distances.Add(city2, subDistances);
                }
                subDistances.Add(city1, distance);
            }
        }

        [Description("What is the distance of the shortest route?")]
        public override string SolvePart1() {
            return $"{FindShortest()}";
        }

        [Description("What is the distance of the longest route?")]
        public override string SolvePart2() {
            return $"{FindLongest()}";
        }

        private int FindShortest() {
            int shortest = int.MaxValue;
            HashSet<string> traveledTo = new HashSet<string>();
            foreach (KeyValuePair<string, Dictionary<string, int>> pair in distances) {
                traveledTo.Clear();
                traveledTo.Add(pair.Key);
                int distance = FindShortestHelper(traveledTo, pair.Value, 0);
                if (distance < shortest) {
                    shortest = distance;
                }
            }
            return shortest;
        }
        private int FindShortestHelper(HashSet<string> traveledTo, Dictionary<string, int> cities, int traveled) {
            int shortest = int.MaxValue;
            foreach (KeyValuePair<string, int> pair in cities) {
                if (traveledTo.Contains(pair.Key)) { continue; }

                if (traveledTo.Count + 1 == distances.Count) {
                    return traveled + pair.Value;
                }

                traveledTo.Add(pair.Key);
                int distance = FindShortestHelper(traveledTo, distances[pair.Key], traveled + pair.Value);
                traveledTo.Remove(pair.Key);
                if (distance < shortest) {
                    shortest = distance;
                }
            }
            return shortest;
        }
        private int FindLongest() {
            int longest = int.MinValue;
            HashSet<string> traveledTo = new HashSet<string>();
            foreach (KeyValuePair<string, Dictionary<string, int>> pair in distances) {
                traveledTo.Clear();
                traveledTo.Add(pair.Key);
                int distance = FindLongestHelper(traveledTo, pair.Value, 0);
                if (distance > longest) {
                    longest = distance;
                }
            }
            return longest;
        }
        private int FindLongestHelper(HashSet<string> traveledTo, Dictionary<string, int> cities, int traveled) {
            int longest = int.MinValue;
            foreach (KeyValuePair<string, int> pair in cities) {
                if (traveledTo.Contains(pair.Key)) { continue; }

                if (traveledTo.Count + 1 == distances.Count) {
                    return traveled + pair.Value;
                }

                traveledTo.Add(pair.Key);
                int distance = FindLongestHelper(traveledTo, distances[pair.Key], traveled + pair.Value);
                traveledTo.Remove(pair.Key);
                if (distance > longest) {
                    longest = distance;
                }
            }
            return longest;
        }
    }
}