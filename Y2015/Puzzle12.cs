using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle12 : ASolver {
        private JsonObject root;
        public Puzzle12(string input) : base(input) { Name = "JSAbacusFramework.io"; }

        public override void Setup() {
            root = JsonReader.Read(Input);
        }

        [Description("What is the sum of all numbers in the document?")]
        public override string SolvePart1() {
            return $"{SumJson(root)}";
        }

        [Description("What is the sum of all numbers in the document that don't belong to red objects?")]
        public override string SolvePart2() {
            return $"{SumJsonMinusRed(root)}";
        }

        private int SumJsonMinusRed(JsonObject json) {
            int sum = 0;
            if (json is JsonArray jsonArray) {
                for (int i = 0; i < jsonArray.Objects.Count; i++) {
                    sum += SumJsonMinusRed(jsonArray.Objects[i]);
                }
            } else if (json is JsonClass jsonClass) {
                bool hasRed = false;
                for (int i = 0; i < jsonClass.Values.Count; i++) {
                    if (jsonClass.Values[i] is JsonItem item && item.Item.Equals("red", StringComparison.OrdinalIgnoreCase)) {
                        hasRed = true;
                        break;
                    }
                }
                if (!hasRed) {
                    for (int i = 0; i < jsonClass.Values.Count; i++) {
                        sum += SumJsonMinusRed(jsonClass.Values[i]);
                    }
                }
            } else if (json is JsonItem jsonItem) {
                sum += SumJson(jsonItem);
            }
            return sum;
        }
        private int SumJson(JsonObject json) {
            int sum = 0;
            foreach (JsonObject obj in json) {
                int value;
                if (int.TryParse(obj.Value(), out value)) {
                    sum += value;
                }
            }
            return sum;
        }
    }
}