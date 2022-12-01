using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("JSAbacusFramework.io")]
    public class Puzzle12 : ASolver {
        private JsonObject root;

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
                for (int i = 0; i < jsonArray.Count; i++) {
                    sum += SumJsonMinusRed(jsonArray[i]);
                }
            } else if (json is JsonClass jsonClass) {
                bool hasRed = false;
                foreach (JsonObject obj in jsonClass) {
                    if (obj is JsonItem jsonItem && jsonItem.Item == "red") {
                        hasRed = true;
                        break;
                    }
                }

                if (!hasRed) {
                    foreach (JsonObject obj in jsonClass) {
                        sum += SumJsonMinusRed(obj);
                    }
                }
            } else {
                sum += json.AsInt();
            }

            return sum;
        }
        private int SumJson(JsonObject json) {
            int sum = 0;
            if (json is JsonArray jsonArray) {
                for (int i = 0; i < jsonArray.Count; i++) {
                    sum += SumJson(jsonArray[i]);
                }
            } else if (json is JsonClass jsonClass) {
                foreach (JsonObject obj in jsonClass) {
                    sum += SumJson(obj);
                }
            } else {
                sum += json.AsInt();
            }

            return sum;
        }
    }
}