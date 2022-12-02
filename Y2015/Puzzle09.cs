using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("All in a Single Night")]
    public class Puzzle09 : ASolver {
        private Dictionary<string, Dictionary<string, int>> distances = new();

        public override void Setup() {
            foreach (string item in Input.Split('\n')) {
                string[] splits = item.SplitOn(" to ", " = ");
                string city1 = splits[0];
                string city2 = splits[1];
                int distance = splits[2].ToInt();

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