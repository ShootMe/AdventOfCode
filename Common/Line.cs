using System;
namespace AdventOfCode.Common {
    public class Line {
        public Point StartPosition;
        public Point EndPosition;

        public Point? Overlaps(Line newLine) {
            int a1 = StartPosition.Y - EndPosition.Y;
            int b1 = StartPosition.X - EndPosition.X;
            int a2 = newLine.StartPosition.Y - newLine.EndPosition.Y;
            int b2 = newLine.StartPosition.X - newLine.EndPosition.X;

            int det = a1 * b2 - a2 * b1;
            if (det == 0) {
                if (StartPosition.Y == newLine.StartPosition.Y) {
                    if (StartPosition.Y <= newLine.EndPosition.Y && EndPosition.Y >= newLine.StartPosition.Y) {
                        return null;
                    }
                } else if (StartPosition.X == newLine.StartPosition.X) {
                    if (StartPosition.X <= newLine.EndPosition.X && EndPosition.X >= newLine.StartPosition.X) {
                        return null;
                    }
                }
                return null;
            } else if (a1 == 0) {
                a1 = Math.Min(StartPosition.X, EndPosition.X);
                a2 = Math.Max(StartPosition.X, EndPosition.X);
                b1 = newLine.StartPosition.X;
                if (a1 <= b1 && a2 >= b1) {
                    a1 = StartPosition.Y;
                    b1 = Math.Min(newLine.StartPosition.Y, newLine.EndPosition.Y);
                    b2 = Math.Max(newLine.StartPosition.Y, newLine.EndPosition.Y);
                    if (a1 <= b2 && a1 >= b1) {
                        return new Point() { X = newLine.StartPosition.X, Y = a1 };
                    }
                }
            } else {
                a1 = Math.Min(StartPosition.Y, EndPosition.Y);
                a2 = Math.Max(StartPosition.Y, EndPosition.Y);
                b1 = newLine.StartPosition.Y;
                if (a1 <= b1 && a2 >= b1) {
                    a1 = StartPosition.X;
                    b1 = Math.Min(newLine.StartPosition.X, newLine.EndPosition.X);
                    b2 = Math.Max(newLine.StartPosition.X, newLine.EndPosition.X);
                    if (a1 <= b2 && a1 >= b1) {
                        return new Point() { X = a1, Y = newLine.StartPosition.Y };
                    }
                }
            }
            return null;
        }

        public int Length() {
            return Math.Abs(StartPosition.X - EndPosition.X) + Math.Abs(StartPosition.Y - EndPosition.Y);
        }
        public override string ToString() {
            return $"{StartPosition} {EndPosition}";
        }
    }
}