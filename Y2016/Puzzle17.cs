using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2016 {
    [Description("Two Steps Forward")]
    public class Puzzle17 : ASolver {
        [Description("What is the shortest path to reach the vault?")]
        public override string SolvePart1() {
            Heap<VaultMove> open = new Heap<VaultMove>();
            open.Enqueue(new VaultMove() { Code = Input, Position = new Point() { X = 0, Y = 0 } });

            while (open.Count > 0) {
                VaultMove move = open.Dequeue();

                if (move.Position.X == 3 && move.Position.Y == 3) {
                    return move.Code.Substring(Input.Length);
                }

                byte[] md5 = MD5.Compute(Encoding.ASCII.GetBytes(move.Code));
                bool up = (md5[0] >> 4) > 10;
                bool down = (md5[0] & 0xf) > 10;
                bool left = (md5[1] >> 4) > 10;
                bool right = (md5[1] & 0xf) > 10;
                if (up && move.Position.Y > 0) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}U", Position = new Point() { X = move.Position.X, Y = move.Position.Y - 1 } });
                }
                if (down && move.Position.Y < 3) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}D", Position = new Point() { X = move.Position.X, Y = move.Position.Y + 1 } });
                }
                if (left && move.Position.X > 0) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}L", Position = new Point() { X = move.Position.X - 1, Y = move.Position.Y } });
                }
                if (right && move.Position.X < 3) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}R", Position = new Point() { X = move.Position.X + 1, Y = move.Position.Y } });
                }
            }
            return string.Empty;
        }

        [Description("What is the length of the longest path that reaches the vault?")]
        public override string SolvePart2() {
            Heap<VaultMove> open = new Heap<VaultMove>();
            open.Enqueue(new VaultMove() { Code = Input, Position = new Point() { X = 0, Y = 0 } });

            int maxLength = 0;
            while (open.Count > 0) {
                VaultMove move = open.Dequeue();

                if (move.Position.X == 3 && move.Position.Y == 3) {
                    if (move.Code.Length > maxLength) {
                        maxLength = move.Code.Length;
                    }
                    continue;
                }

                byte[] md5 = MD5.Compute(Encoding.ASCII.GetBytes(move.Code));
                bool up = (md5[0] >> 4) > 10;
                bool down = (md5[0] & 0xf) > 10;
                bool left = (md5[1] >> 4) > 10;
                bool right = (md5[1] & 0xf) > 10;
                if (up && move.Position.Y > 0) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}U", Position = new Point() { X = move.Position.X, Y = move.Position.Y - 1 } });
                }
                if (down && move.Position.Y < 3) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}D", Position = new Point() { X = move.Position.X, Y = move.Position.Y + 1 } });
                }
                if (left && move.Position.X > 0) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}L", Position = new Point() { X = move.Position.X - 1, Y = move.Position.Y } });
                }
                if (right && move.Position.X < 3) {
                    open.Enqueue(new VaultMove() { Code = $"{move.Code}R", Position = new Point() { X = move.Position.X + 1, Y = move.Position.Y } });
                }
            }
            return $"{maxLength - Input.Length}";
        }

        private class VaultMove : IComparable<VaultMove> {
            public Point Position;
            public string Code;

            public int CompareTo(VaultMove other) {
                return Code.Length.CompareTo(other.Code.Length);
            }
            public override string ToString() {
                return $"{Position} {Code}";
            }
        }
    }
}