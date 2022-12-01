using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("No Matter How You Slice It")]
    public class Puzzle03 : ASolver {
        private Claim[] claims;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            claims = new Claim[items.Count];
            for (int i = 0; i < claims.Length; i++) {
                claims[i] = items[i];
            }
        }

        [Description("How many square inches of fabric are within two or more claims?")]
        public override string SolvePart1() {
            int[] counts = new int[1000000];
            for (int i = 0; i < claims.Length; i++) {
                Claim claim = claims[i];

                for (int j = claim.X + claim.W - 1; j >= claim.X; j--) {
                    for (int k = claim.Y + claim.H - 1; k >= claim.Y; k--) {
                        counts[k * 1000 + j]++;
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < counts.Length; i++) {
                if (counts[i] > 1) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("What is the ID of the only claim that doesn't overlap?")]
        public override string SolvePart2() {
            for (int i = 0; i < claims.Length; i++) {
                Claim claim = claims[i];

                bool hasOverlap = false;
                for (int j = 0; j < claims.Length; j++) {
                    if (j == i) { continue; }

                    if (claims[j].Overlaps(claim) > 0) {
                        hasOverlap = true;
                        break;
                    }
                }

                if (!hasOverlap) {
                    return $"{claim.ID}";
                }
            }

            return string.Empty;
        }

        private class Claim {
            public int ID, X, Y, W, H;

            public static implicit operator Claim(string value) {
                Claim claim = new Claim();
                
                string[] splits = Tools.SplitOn(value, " @ ", ",", ": ", "x");
                claim.ID = Tools.ParseInt(splits[0], 1);
                claim.X = Tools.ParseInt(splits[1]);
                claim.Y = Tools.ParseInt(splits[2]);
                claim.W = Tools.ParseInt(splits[3]);
                claim.H = Tools.ParseInt(splits[4]);
                
                return claim;
            }
            public int Overlaps(Claim other) {
                bool overlap = X <= other.X + other.W && X + W >= other.X && Y <= other.Y + other.H && Y + H >= other.Y;
                if (!overlap) { return 0; }

                int x1 = Math.Max(X, other.X);
                int x2 = Math.Min(X + W, other.X + other.W);

                int y1 = Math.Max(Y, other.Y);
                int y2 = Math.Min(Y + H, other.Y + other.H);

                return (x2 - x1) * (y2 - y1);
            }
        }
    }
}