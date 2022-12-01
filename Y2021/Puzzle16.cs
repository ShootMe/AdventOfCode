using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2021 {
    [Description("Packet Decoder")]
    public class Puzzle16 : ASolver {
        private Packet packet;

        public override void Setup() {
            packet = Packet.ReadPacket(Input);
        }

        [Description("What do you get if you add up the version numbers in all packets?")]
        public override string SolvePart1() {
            return $"{packet.VersionSum()}";
        }

        [Description("What do you get if you evaluate the expression represented by your BITS transmission?")]
        public override string SolvePart2() {
            return $"{packet.Evaluate()}";
        }

        private class Packet {
            public int Version, Bits;
            public PacketType TypeID;
            public long Value;
            public List<Packet> SubPackets = new List<Packet>();
            public enum PacketType : byte {
                Sum,
                Product,
                Minimum,
                Maximum,
                Literal,
                GreaterThan,
                LessThan,
                EqualTo
            }
            public static Packet ReadPacket(string input) {
                BitArray bits = new BitArray(input);
                return ReadPacket(bits);
            }
            private static Packet ReadPacket(BitArray bits) {
                int startIndex = bits.Position;
                Packet packet = new Packet() {
                    Version = bits.ReadBits(3),
                    TypeID = (PacketType)bits.ReadBits(3)
                };

                if (packet.TypeID == PacketType.Literal) {
                    int sub;
                    do {
                        sub = bits.ReadBits(5);
                        packet.Value <<= 4;
                        packet.Value |= (long)sub & 0xf;
                    } while ((sub & 16) != 0);
                } else if (bits.ReadBits(1) == 1) {
                    packet.ReadSubPackets(bits, int.MaxValue, bits.ReadBits(11));
                } else {
                    packet.ReadSubPackets(bits, bits.ReadBits(15), int.MaxValue);
                }

                packet.Bits = bits.Position - startIndex;
                return packet;
            }
            private void ReadSubPackets(BitArray bits, int bitsLeft, int packetsLeft) {
                while (bits.HasMore && bitsLeft > 0 && packetsLeft > 0) {
                    Packet packet = ReadPacket(bits);
                    bitsLeft -= packet.Bits;
                    packetsLeft--;
                    SubPackets.Add(packet);
                }
            }
            public int VersionSum() {
                int total = Version;
                for (int i = 0; i < SubPackets.Count; i++) {
                    total += SubPackets[i].VersionSum();
                }
                return total;
            }
            public long Evaluate() {
                long total = 0;
                switch (TypeID) {
                    case PacketType.Sum: Evaluate(value => total += value); break;
                    case PacketType.Product: total = 1; Evaluate(value => total *= value); break;
                    case PacketType.Minimum: total = long.MaxValue; Evaluate(value => { if (value < total) total = value; }); break;
                    case PacketType.Maximum: total = long.MinValue; Evaluate(value => { if (value > total) total = value; }); break;
                    case PacketType.Literal: return Value;
                    case PacketType.GreaterThan: return SubPackets[0].Evaluate() > SubPackets[1].Evaluate() ? 1 : 0;
                    case PacketType.LessThan: return SubPackets[0].Evaluate() < SubPackets[1].Evaluate() ? 1 : 0;
                    case PacketType.EqualTo: return SubPackets[0].Evaluate() == SubPackets[1].Evaluate() ? 1 : 0;
                }
                return total;
            }
            private void Evaluate(Action<long> action) {
                for (int i = 0; i < SubPackets.Count; i++) {
                    action.Invoke(SubPackets[i].Evaluate());
                }
            }
            public string Expression() {
                StringBuilder sb = new StringBuilder();
                switch (TypeID) {
                    case PacketType.Sum:
                    case PacketType.Product:
                    case PacketType.Minimum:
                    case PacketType.Maximum: Expression(sb); break;
                    case PacketType.Literal: return $"{Value}";
                    case PacketType.GreaterThan: sb.Append($"({SubPackets[0].Expression()} >? {SubPackets[1].Expression()})"); break;
                    case PacketType.LessThan: sb.Append($"({SubPackets[0].Expression()} <? {SubPackets[1].Expression()})"); break;
                    case PacketType.EqualTo: sb.Append($"({SubPackets[0].Expression()} == {SubPackets[1].Expression()})"); break;
                }
                return sb.ToString();
            }
            private void Expression(StringBuilder sb) {
                if (SubPackets.Count > 1) { sb.Append('('); }
                for (int i = 0; i < SubPackets.Count; i++) {
                    string exp = SubPackets[i].Expression();
                    switch (TypeID) {
                        case PacketType.Sum: sb.Append($"{exp} + "); break;
                        case PacketType.Product: sb.Append($"{exp} * "); break;
                        case PacketType.Minimum: sb.Append($"{exp} v? "); break;
                        case PacketType.Maximum: sb.Append($"{exp} ^? "); break;
                    }
                }
                switch (TypeID) {
                    case PacketType.Sum:
                    case PacketType.Product: sb.Length -= 3; break;
                    case PacketType.Minimum:
                    case PacketType.Maximum: sb.Length -= 4; break;
                }
                if (SubPackets.Count > 1) { sb.Append(')'); }
            }
            public override string ToString() {
                return $"Version={Version} TypeID={TypeID} Value={Evaluate()} Subs={SubPackets.Count}";
            }
        }
        private class BitArray {
            private static readonly byte[] masks = new byte[] { 0, 1, 3, 7, 15 };
            private readonly string data;
            private int bit, index;
            private byte current;
            public bool HasMore { get { return index < data.Length; } }
            public int Position { get; set; }
            public BitArray(string input) {
                data = input;
            }
            private void Advance() {
                current = ConvertToBinary(data[index++]);
                bit = 4;
            }
            private byte ConvertToBinary(char value) {
                byte bit64 = (byte)(value & 64);
                return (byte)(((bit64 >> 3) | (value & 15)) + (bit64 >> 6));
            }
            public int ReadBits(int bits) {
                Position += bits;
                int value = 0;
                while (bits > bit) {
                    bits -= bit;
                    value |= (current & masks[bit]) << bits;
                    Advance();
                }

                bit -= bits;
                value |= (byte)((current >> bit) & masks[bits]);

                return value;
            }
        }
    }
}