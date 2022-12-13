using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2022 {
    [Description("Distress Signal")]
    public class Puzzle13 : ASolver {
        private List<Packet> packets = new();

        public override void Setup() {
            foreach (string signal in Input.Split("\n\n")) {
                string[] pair = signal.Split('\n');
                packets.Add(Packet.ReadPacket(pair[0]));
                packets.Add(Packet.ReadPacket(pair[1]));
            }
        }

        [Description("What is the sum of the indices of those pairs?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < packets.Count; i++) {
                Packet left = packets[i++];
                Packet right = packets[i];
                if (left.CompareTo(right) < 0) {
                    total += i / 2 + 1;
                }
            }
            return $"{total}";
        }

        [Description("What is the decoder key for the distress signal?")]
        public override string SolvePart2() {
            Packet div2 = Packet.ReadPacket("[[2]]");
            Packet div6 = Packet.ReadPacket("[[6]]");
            packets.Add(div2);
            packets.Add(div6);
            packets.Sort();
            return $"{(packets.IndexOf(div2) + 1) * (packets.IndexOf(div6) + 1)}";
        }

        private class Packet : IComparable<Packet> {
            public List<Packet> Packets;

            public static Packet ReadPacket(string packet) {
                int start = 1;
                int index = 1;
                return ReadPacket(packet, ref start, ref index);
            }
            private static Packet ReadPacket(string packet, ref int start, ref int index) {
                Packet newPacket = new Packet() { Packets = new() };
                while (index < packet.Length) {
                    char current = packet[index++];

                    switch (current) {
                        case '[':
                            start = index;
                            newPacket.Packets.Add(ReadPacket(packet, ref start, ref index));
                            break;
                        case ']':
                            if (start < index - 1) {
                                newPacket.Packets.Add(new PacketValue() { Value = packet[start..(index - 1)].ToInt() });
                            }
                            start = index;
                            return newPacket;
                        case ',':
                            if (start < index - 1) {
                                newPacket.Packets.Add(new PacketValue() { Value = packet[start..(index - 1)].ToInt() });
                            }
                            start = index;
                            break;
                    }
                }
                return newPacket;
            }
            public int CompareTo(Packet other) {
                for (int i = 0; i < Packets.Count; i++) {
                    Packet sub = Packets[i];
                    if (i == other.Packets.Count) { return 1; }
                    Packet otherSub = other.Packets[i];

                    PacketValue subValue = sub as PacketValue;
                    PacketValue otherValue = otherSub as PacketValue;
                    if (subValue != null && otherValue != null) {
                        int valueComp = subValue.Value.CompareTo(otherValue.Value);
                        if (valueComp != 0) { return valueComp; }
                        continue;
                    } else if (subValue != null) {
                        sub = new Packet() { Packets = new() };
                        sub.Packets.Add(subValue);
                    } else if (otherValue != null) {
                        otherSub = new Packet() { Packets = new() };
                        otherSub.Packets.Add(otherValue);
                    }

                    int comp = sub.CompareTo(otherSub);
                    if (comp != 0) { return comp; }
                }
                return Packets.Count.CompareTo(other.Packets.Count);
            }
            public override bool Equals(object obj) {
                return obj is Packet packet && Equals(packet);
            }
            public bool Equals(Packet packet) {
                return packet.CompareTo(this) == 0;
            }
            public override int GetHashCode() {
                return Packets.Count;
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder("[");
                for (int i = 0; i < Packets.Count; i++) {
                    sb.Append(Packets[i].ToString()).Append(',');
                }
                if (sb.Length > 1) { sb.Length--; }
                return sb.Append(']').ToString();
            }
        }
        private class PacketValue : Packet {
            public int Value;
            public override string ToString() {
                return $"{Value}";
            }
        }
    }
}