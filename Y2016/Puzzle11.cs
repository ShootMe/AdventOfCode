using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2016 {
    [Description("Radioisotope Thermoelectric Generators")]
    public class Puzzle11 : ASolver {
        private FloorContents[] floors;
        private Dictionary<string, byte> elementToIndex;
        private Dictionary<byte, string> indexToElement;
        private byte allElements;

        public override void Setup() {
            List<string> floorItems = Input.Lines();
            floors = new FloorContents[floorItems.Count];
            elementToIndex = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
            indexToElement = new Dictionary<byte, string>();
            allElements = 1;

            for (int i = 0; i < floorItems.Count; i++) {
                string list = floorItems[i];
                byte microchips = 0;
                byte generators = 0;

                if (list.IndexOf("nothing relevant", StringComparison.OrdinalIgnoreCase) < 0) {
                    string[] items = list.Split(" a ");
                    for (int j = 1; j < items.Length; j++) {
                        string item = items[j];
                        int index = item.IndexOf('-');
                        bool isMicrochip = index > 0;

                        if (!isMicrochip) {
                            index = item.IndexOf(' ');
                        }

                        byte bit;
                        string element = item.Substring(0, index);
                        if (!elementToIndex.TryGetValue(element, out bit)) {
                            bit = allElements;
                            elementToIndex.Add(element, allElements);
                            indexToElement.Add(allElements, element);
                            allElements <<= 1;
                        }

                        if (isMicrochip) {
                            microchips |= bit;
                        } else {
                            generators |= bit;
                        }
                    }
                }

                floors[i] = new FloorContents() { Microchips = microchips, Generators = generators };
            }

            allElements--;
        }

        [Description("What is the minimum steps required to bring all of the objects to the fourth floor?")]
        public override string SolvePart1() {
            return $"{Solve(170000, 30000)}";
        }

        [Description("What is the minimum steps required, including these four new ones, to the fourth floor?")]
        public override string SolvePart2() {
            allElements++;
            elementToIndex.Add("elerium", allElements);
            indexToElement.Add(allElements, "elerium");
            floors[0].Generators |= allElements;
            floors[0].Microchips |= allElements;
            allElements <<= 1;

            elementToIndex.Add("dilithium", allElements);
            indexToElement.Add(allElements, "dilithium");
            floors[0].Generators |= allElements;
            floors[0].Microchips |= allElements;
            allElements <<= 1;
            allElements--;

            return $"{Solve(610000, 400000)}";
        }

        private int Solve(int initialClosed, int maxOpen) {
            HashSet<(FloorContents, FloorContents, FloorContents, FloorContents, int)> closed = new HashSet<(FloorContents, FloorContents, FloorContents, FloorContents, int)>(initialClosed);
            Queue<(FloorContents, FloorContents, FloorContents, FloorContents, int, int)> open = new Queue<(FloorContents, FloorContents, FloorContents, FloorContents, int, int)>(maxOpen);

            int bestSteps = int.MaxValue;

            (FloorContents, FloorContents, FloorContents, FloorContents, int, int) currentState = (floors[0], floors[1], floors[2], floors[3], 0, 0);
            closed.Add((floors[0], floors[1], floors[2], floors[3], 0));
            open.Enqueue(currentState);

            while (open.Count > 0) {
                (FloorContents floor1, FloorContents floor2, FloorContents floor3, FloorContents floor4, int floor, int steps) state = open.Dequeue();
                if (state.steps >= bestSteps) { continue; }

                //Console.WriteLine(state);
                FloorContents last = state.floor4;
                bool completed = last.Generators == allElements && last.Microchips == allElements;
                if (completed) {
                    bestSteps = state.steps;
                    //Console.WriteLine(state);
                    continue;
                }

                FloorContents floorContents = GetFloorContents(state, state.floor);
                if (state.floor < 3) {
                    EnumerateMoves(closed, open, state, floorContents, GetFloorContents(state, state.floor + 1), state.floor + 1);
                }

                if (state.floor > 0) {
                    EnumerateMoves(closed, open, state, floorContents, GetFloorContents(state, state.floor - 1), state.floor - 1);
                }
            }

            return bestSteps;
        }
        private FloorContents GetFloorContents((FloorContents floor1, FloorContents floor2, FloorContents floor3, FloorContents floor4, int floor, int steps) state, int floor) {
            switch (floor) {
                case 1: return state.floor2;
                case 2: return state.floor3;
                case 3: return state.floor4;
                default: return state.floor1;
            }
        }
        private void EnumerateMoves(HashSet<(FloorContents, FloorContents, FloorContents, FloorContents, int)> closed, Queue<(FloorContents, FloorContents, FloorContents, FloorContents, int, int)> open, (FloorContents floor1, FloorContents floor2, FloorContents floor3, FloorContents floor4, int floor, int steps) state, FloorContents currentFloor, FloorContents nextFloor, int nextFloorNumber) {
            foreach (FloorContents move in nextFloor.Moves(currentFloor)) {
                FloorContents newContents = currentFloor & move;

                (FloorContents floor1, FloorContents floor2, FloorContents floor3, FloorContents floor4, int floor) temp =
                    (state.floor == 0 ? newContents : nextFloorNumber == 0 ? move : state.floor1,
                    state.floor == 1 ? newContents : nextFloorNumber == 1 ? move : state.floor2,
                    state.floor == 2 ? newContents : nextFloorNumber == 2 ? move : state.floor3,
                    state.floor == 3 ? newContents : nextFloorNumber == 3 ? move : state.floor4,
                    nextFloorNumber);

                if (!closed.Contains(temp)) {
                    closed.Add(temp);
                    open.Enqueue((temp.floor1, temp.floor2, temp.floor3, temp.floor4, temp.floor, state.steps + 1));
                }
            }
        }

        private struct FloorContents : IEquatable<FloorContents> {
            public byte Microchips;
            public byte Generators;

            public bool IsValid() {
                return (Generators == 0 || (Generators & Microchips) == Microchips);
            }
            public IEnumerable<FloorContents> Moves(FloorContents floorFrom) {
                byte bit = 1;
                while (bit != 0) {
                    byte generator = (byte)(floorFrom.Generators & bit);
                    byte microchip = (byte)(floorFrom.Microchips & bit);
                    if (generator != 0 || microchip != 0) {
                        //Just generator
                        FloorContents newFrom = new FloorContents() { Microchips = floorFrom.Microchips, Generators = (byte)(floorFrom.Generators & ~generator) };
                        byte newGen = (byte)(Generators | generator);
                        FloorContents newContents = new FloorContents() { Microchips = Microchips, Generators = newGen };
                        if (generator != 0 && newFrom.IsValid() && newContents.IsValid()) {
                            yield return newContents;
                        }

                        //Just microchip
                        byte newMicro = (byte)(Microchips | microchip);
                        newContents = new FloorContents() { Microchips = newMicro, Generators = Generators };
                        if (microchip != 0 && newContents.IsValid()) {
                            yield return newContents;
                        }

                        //Both
                        newContents = new FloorContents() { Microchips = newMicro, Generators = newGen };
                        if (microchip != 0 && generator != 0 && newContents.IsValid()) {
                            yield return newContents;
                        }

                        //Any two generators
                        if (generator != 0) {
                            byte bitOther = (byte)(bit << 1);
                            while (bitOther != 0) {
                                byte otherGenerator = (byte)(floorFrom.Generators & bitOther);
                                if (otherGenerator != 0) {
                                    otherGenerator |= generator;
                                    newFrom = new FloorContents() { Microchips = floorFrom.Microchips, Generators = (byte)(floorFrom.Generators & ~otherGenerator) };
                                    newContents = new FloorContents() { Microchips = Microchips, Generators = (byte)(Generators | otherGenerator) };
                                    if (newFrom.IsValid() && newContents.IsValid()) {
                                        yield return newContents;
                                    }
                                }
                                bitOther <<= 1;
                            }
                        }

                        //Any two microchips
                        if (microchip != 0) {
                            byte bitOther = (byte)(bit << 1);
                            while (bitOther != 0) {
                                byte otherMicrochip = (byte)(floorFrom.Microchips & bitOther);
                                if (otherMicrochip != 0) {
                                    otherMicrochip |= microchip;
                                    newContents = new FloorContents() { Microchips = (byte)(Microchips | otherMicrochip), Generators = Generators };
                                    if (newContents.IsValid()) {
                                        yield return newContents;
                                    }
                                }
                                bitOther <<= 1;
                            }
                        }
                    }

                    bit <<= 1;
                }
            }
            public static FloorContents operator &(FloorContents left, FloorContents right) {
                return new FloorContents() { Microchips = (byte)(left.Microchips & ~right.Microchips), Generators = (byte)(left.Generators & ~right.Generators) };
            }
            public string ToString(Dictionary<ushort, string> indexToElement) {
                StringBuilder items = new StringBuilder("[");
                ushort bit = 1;
                while (bit != 0) {
                    if ((Microchips & bit) != 0) {
                        string element = indexToElement[bit];
                        items.Append($"{element} micro, ");
                    }

                    if ((Generators & bit) != 0) {
                        string element = indexToElement[bit];
                        items.Append($"{element} gen, ");
                    }

                    bit <<= 1;
                }
                if (items.Length > 1) {
                    items.Length -= 2;
                }
                items.Append(']');
                return items.ToString();
            }
            public override string ToString() {
                return $"{Convert.ToString(Microchips, 2).PadLeft(8, '0')} {Convert.ToString(Generators, 2).PadLeft(8, '0')}";
            }
            public override bool Equals(object obj) {
                return obj is FloorContents item && item.Microchips == Microchips && item.Generators == Generators;
            }
            public bool Equals(FloorContents other) {
                return other.Microchips == Microchips && other.Generators == Generators;
            }
            public override int GetHashCode() {
                return Microchips | ((int)Generators << 8);
            }
        }
    }
}