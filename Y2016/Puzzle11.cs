using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2016 {
    public class Puzzle11 : ASolver {
        private FloorContents[] floors;
        private Dictionary<string, byte> elementToIndex;
        private Dictionary<byte, string> indexToElement;
        private byte allElements;
        public Puzzle11(string input) : base(input) { Name = "Radioisotope Thermoelectric Generators"; }

        public override void Setup() {
            List<string> floorItems = Tools.GetLines(Input);
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
            HashSet<FloorState> closed = new HashSet<FloorState>(initialClosed);
            Heap<FloorState> open = new Heap<FloorState>(maxOpen);

            int bestSteps = int.MaxValue;

            FloorState currentState = new FloorState() { Floors = floors, Floor = 0, Steps = 0 };
            closed.Add(currentState);
            open.Enqueue(currentState);

            while (open.Count > 0) {
                FloorState state = open.Dequeue();
                if (state.Steps >= bestSteps) { continue; }

                //Console.WriteLine(state);
                FloorContents last = state.Floors[state.Floors.Length - 1];
                bool completed = last.Generators == allElements && last.Microchips == allElements;
                if (completed) {
                    bestSteps = state.Steps;
                    //Console.WriteLine(state);
                    continue;
                }

                if (state.Floor + 1 < state.Floors.Length) {
                    EnumerateMoves(closed, open, state, (byte)(state.Floor + 1));
                }

                if (state.Floor > 0) {
                    EnumerateMoves(closed, open, state, (byte)(state.Floor - 1));
                }
            }

            return bestSteps;
        }
        private void EnumerateMoves(HashSet<FloorState> closed, Heap<FloorState> open, FloorState state, byte nextFloor) {
            FloorContents contents = state.Floors[state.Floor];
            FloorContents nextContents = state.Floors[nextFloor];
            FloorState temp = new FloorState() { Floor = nextFloor };

            foreach (FloorContents move in nextContents.Moves(contents)) {
                FloorContents newContents = contents & move;

                FloorContents[] newFloors = new FloorContents[state.Floors.Length];
                Array.Copy(state.Floors, newFloors, state.Floors.Length);
                newFloors[state.Floor] = newContents;
                newFloors[nextFloor] = move;
                temp.Floors = newFloors;

                if (!closed.Contains(temp)) {
                    FloorState newState = new FloorState() { Floors = newFloors, Floor = nextFloor, Steps = (byte)(state.Steps + 1), Previous = state };
                    closed.Add(newState);
                    open.Enqueue(newState);
                }
            }
        }

        private class FloorState : IComparable<FloorState>, IEquatable<FloorState> {
            public FloorContents[] Floors;
            public byte Steps;
            public byte Floor;
            public FloorState Previous;

            public int CompareTo(FloorState other) {
                int compare = Steps.CompareTo(other.Steps);
                if (compare != 0) { return compare; }

                return other.Floor.CompareTo(Floor);
            }
            public bool Equals(FloorState other) {
                ReadOnlySpan<FloorContents> left = new ReadOnlySpan<FloorContents>(Floors);
                ReadOnlySpan<FloorContents> right = new ReadOnlySpan<FloorContents>(other.Floors);
                bool floorsEqual = left.SequenceEqual(right);
                return floorsEqual && Floor == other.Floor;
            }
            public override bool Equals(object obj) {
                return obj is FloorState state && Equals(state);
            }
            public override int GetHashCode() {
                int hash = 17;
                for (int i = Floors.Length - 1; i >= 0; i--) {
                    hash = (hash * 31) + Floors[i].GetHashCode();
                }
                return hash * 31 + Floor;
            }
            public override string ToString() {
                StringBuilder state = new StringBuilder();
                state.Append(State());
                FloorState previous = Previous;
                while (previous != null) {
                    state.AppendLine();
                    state.Append(previous.State());
                    previous = previous.Previous;
                }
                return state.ToString();
            }
            public string State() {
                StringBuilder state = new StringBuilder();
                for (int i = Floors.Length - 1; i >= 0; i--) {
                    string elevator = i == Floor ? "E" : " ";
                    state.AppendLine($"F{i + 1} {elevator} {Floors[i]}");
                }
                return state.ToString();
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