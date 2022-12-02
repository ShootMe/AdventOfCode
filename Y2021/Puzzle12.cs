using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Passage Pathing")]
    public class Puzzle12 : ASolver {
        private Dictionary<int, HashSet<int>> links;
        private int startID, endID;
        public override void Setup() {
            List<string> items = Input.Lines();
            links = new Dictionary<int, HashSet<int>>();
            Dictionary<string, int> mappings = new Dictionary<string, int>();
            int mappingID = 1;
            for (int i = 0; i < items.Count; i++) {
                string[] split = items[i].Split('-');

                if (!mappings.TryGetValue(split[0], out int mappingOne)) {
                    SetStartEndID(split[0], mappingID);
                    mappingOne = mappingID | (char.IsLower(split[0][0]) ? 0 : 256);
                    mappings[split[0]] = mappingOne;
                    mappingID++;
                }
                if (!mappings.TryGetValue(split[1], out int mappingTwo)) {
                    SetStartEndID(split[1], mappingID);
                    mappingTwo = mappingID | (char.IsLower(split[1][0]) ? 0 : 256);
                    mappings[split[1]] = mappingTwo;
                    mappingID++;
                }

                if (mappingTwo != startID && mappingOne != endID) {
                    if (!links.TryGetValue(mappingOne, out HashSet<int> caveLinks)) {
                        caveLinks = new HashSet<int>();
                        links[mappingOne] = caveLinks;
                    }
                    caveLinks.Add(mappingTwo);
                }

                if (mappingOne != startID && mappingTwo != endID) {
                    if (!links.TryGetValue(mappingTwo, out HashSet<int> caveLinks)) {
                        caveLinks = new HashSet<int>();
                        links[mappingTwo] = caveLinks;
                    }
                    caveLinks.Add(mappingOne);
                }
            }
        }
        private void SetStartEndID(string cave, int mappingID) {
            if (cave == "start") {
                startID = mappingID;
            } else if (cave == "end") {
                endID = mappingID;
            }
        }

        [Description("How many paths through this cave system are there that visit small caves at most once?")]
        public override string SolvePart1() {
            return $"{FindPaths()}";
        }

        [Description("How many paths through this cave system are there?")]
        public override string SolvePart2() {
            return $"{FindPaths(true)}";
        }

        private int FindPaths(bool allowSmallTwice = false) {
            HashSet<int> usedIDs = new HashSet<int>();
            Stack<(int, int, bool)> ids = new Stack<(int, int, bool)>();
            ids.Push((startID, 1, allowSmallTwice));

            int pathCount = 0;
            while (ids.Count > 0) {
                (int id, int state, bool allowed) = ids.Pop();

                if (state == 1) {
                    ids.Push((id, 2, allowSmallTwice && usedIDs.Contains(id)));
                    usedIDs.Add(id);

                    HashSet<int> caveLinks = links[id];
                    foreach (int nextID in caveLinks) {
                        if (nextID == endID) { pathCount++; continue; }

                        if (allowed || IsLargeCave(nextID) || !usedIDs.Contains(nextID)) {
                            ids.Push((nextID, 1, allowed && (IsLargeCave(nextID) || !usedIDs.Contains(nextID))));
                        }
                    }
                } else if (!allowed) {
                    usedIDs.Remove(id);
                }
            }
            return pathCount;
        }
        private bool IsLargeCave(int id) {
            return (id & 256) == 256;
        }
    }
}