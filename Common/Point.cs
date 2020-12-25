using System;
using AdventOfCode.Core;
namespace AdventOfCode.Common {
    public struct Point : IEquatable<Point> {
        public static Point ZERO { get; } = default;
        public static Point NORTH { get; } = new Point() { X = 0, Y = -1 };
        public static Point SOUTH { get; } = new Point() { X = 0, Y = 1 };
        public static Point EAST { get; } = new Point() { X = 1, Y = 0 };
        public static Point WEST { get; } = new Point() { X = -1, Y = 0 };
        public int X;
        public int Y;

        public int Distance() {
            return Math.Abs(X) + Math.Abs(Y);
        }
        public int Distance(Point right) {
            return Math.Abs(X - right.X) + Math.Abs(Y - right.Y);
        }
        public override string ToString() {
            return $"({X}, {Y})";
        }
        public bool Equals(Point point) {
            return point.X == X && point.Y == Y;
        }
        public override bool Equals(object obj) {
            return obj is Point point && point.X == X && point.Y == Y;
        }
        public override int GetHashCode() {
            return X ^ Y;
        }
        public static bool operator ==(Point left, Point right) {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator !=(Point left, Point right) {
            return left.X != right.X || left.Y != right.Y;
        }
        public static Point operator -(Point left, Point right) {
            return new Point() { X = left.X - right.X, Y = left.Y - right.Y };
        }
        public static Point operator +(Point left, Point right) {
            return new Point() { X = left.X + right.X, Y = left.Y + right.Y };
        }
        public static Point operator *(Point left, Point right) {
            return new Point() { X = left.X * right.X, Y = left.Y * right.Y };
        }
        public static Point operator *(Point left, int right) {
            return new Point() { X = left.X * right, Y = left.Y * right };
        }
        public static Point operator *(int left, Point right) {
            return new Point() { X = left * right.X, Y = left * right.X };
        }
        public static Point operator /(Point left, Point right) {
            return new Point() { X = left.X / right.X, Y = left.Y / right.Y };
        }
        public static Point operator /(Point left, int right) {
            return new Point() { X = left.X / right, Y = left.Y / right };
        }
        public static Point operator /(int left, Point right) {
            return new Point() { X = left / right.X, Y = left / right.X };
        }
        public static bool operator >(Point left, Point right) {
            return GreaterThan(left, right);
        }
        public static bool operator <(Point left, Point right) {
            return GreaterThan(right, left);
        }
        public Point Normalize() {
            int gcd = Tools.GCD(X, Y);
            return new Point() { X = X / gcd, Y = Y / gcd };
        }
        private static bool GreaterThan(Point left, Point right) {
            if (left.X == 0 && left.Y == 0) { return true; }
            if (right.X == 0 && right.Y == 0) { return true; }

            bool leftXPos = left.X > 0;
            bool leftYNeg = left.Y < 0;
            int leftQuad = (leftXPos || left.X == 0) && leftYNeg ? 1 : leftXPos && !leftYNeg ? 2 : !leftXPos && !(leftYNeg || left.Y == 0) ? 3 : 4;

            bool rightXPos = right.X > 0;
            bool rightYNeg = right.Y < 0;
            int rightQuad = (rightXPos || right.X == 0) && rightYNeg ? 1 : rightXPos && !rightYNeg ? 2 : !rightXPos && !(rightYNeg || right.Y == 0) ? 3 : 4;
            if (leftQuad != rightQuad) { return leftQuad > rightQuad; }

            int gcd = Tools.GCD(left.Y, right.Y);
            int leftNum = left.X * right.Y / gcd;
            int rightNum = right.X * left.Y / gcd;
            return leftNum < rightNum;
        }
    }
}