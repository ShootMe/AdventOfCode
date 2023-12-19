using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2023 {
    [Description("Aplenty")]
    public class Puzzle19 : ASolver {
        private Dictionary<string, WorkFlow> flows = new();
        private List<Rating> partRatings = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            int i = 0;
            for (; i < lines.Length; i++) {
                string line = lines[i];
                if (string.IsNullOrEmpty(line)) { break; }

                WorkFlow flow = new WorkFlow(line);
                flows.Add(flow.Name, flow);
            }

            i++;
            for (; i < lines.Length; i++) {
                string line = lines[i];
                partRatings.Add(new Rating(line));
            }
        }

        [Description("What is the sum of the rating numbers for all of the parts that ultimately get accepted?")]
        public override string SolvePart1() {
            while (ProcessRatings(partRatings)) { }

            int total = 0;
            for (int i = 0; i < partRatings.Count; i++) {
                Rating rating = partRatings[i];
                if (rating.Result == "A") {
                    total += rating.Total;
                }
            }
            return $"{total}";
        }

        [Description("How many distinct combinations of ratings will be accepted by the Elves' workflows?")]
        public override string SolvePart2() {
            List<Rating> fullRatings = new();
            fullRatings.Add(new Rating());

            while (ProcessRatings(fullRatings)) { }

            long combinations = 0;
            for (int i = 0; i < fullRatings.Count; i++) {
                Rating rating = fullRatings[i];
                if (rating.Result == "A") {
                    combinations += rating.Combinations();
                }
            }
            return $"{combinations}";
        }

        private bool ProcessRatings(List<Rating> ratings) {
            for (int j = 0; j < ratings.Count; j++) {
                Rating rating = ratings[j];
                if (rating.Result == "A" || rating.Result == "R") { continue; }

                WorkFlow flow = flows[rating.Result];

                for (int i = 0; i < flow.Conditions.Count; i++) {
                    Condition condition = flow.Conditions[i];

                    if (condition.Operator == OpType.None) {
                        rating.Result = condition.Result;
                        break;
                    }

                    int amount = condition.Field switch {
                        'x' => rating.X,
                        'm' => rating.M,
                        'a' => rating.A,
                        _ => rating.S
                    };
                    int amountEnd = condition.Field switch {
                        'x' => rating.XE,
                        'm' => rating.ME,
                        'a' => rating.AE,
                        _ => rating.SE
                    };

                    if (condition.Operator == OpType.GreaterThan) {
                        if (amount > condition.Amount) {
                            rating.Result = condition.Result;
                            break;
                        } else if (amountEnd > condition.Amount) {
                            Rating newRating = new Rating(rating);
                            newRating.Result = condition.Result;
                            switch (condition.Field) {
                                case 'x':
                                    newRating.X = condition.Amount + 1;
                                    rating.XE = condition.Amount;
                                    break;
                                case 'm':
                                    newRating.M = condition.Amount + 1;
                                    rating.ME = condition.Amount;
                                    break;
                                case 'a':
                                    newRating.A = condition.Amount + 1;
                                    rating.AE = condition.Amount;
                                    break;
                                case 's':
                                    newRating.S = condition.Amount + 1;
                                    rating.SE = condition.Amount;
                                    break;
                            }
                            ratings.Add(newRating);
                        }
                    } else if (condition.Operator == OpType.LessThan) {
                        if (amountEnd < condition.Amount) {
                            rating.Result = condition.Result;
                            break;
                        } else if (amount < condition.Amount) {
                            Rating newRating = new Rating(rating);
                            newRating.Result = condition.Result;
                            switch (condition.Field) {
                                case 'x':
                                    newRating.XE = condition.Amount - 1;
                                    rating.X = condition.Amount;
                                    break;
                                case 'm':
                                    newRating.ME = condition.Amount - 1;
                                    rating.M = condition.Amount;
                                    break;
                                case 'a':
                                    newRating.AE = condition.Amount - 1;
                                    rating.A = condition.Amount;
                                    break;
                                case 's':
                                    newRating.SE = condition.Amount - 1;
                                    rating.S = condition.Amount;
                                    break;
                            }
                            ratings.Add(newRating);
                        }
                    }
                }
            }

            for (int j = 0; j < ratings.Count; j++) {
                Rating rating = ratings[j];
                if (rating.Result != "A" && rating.Result != "R") {
                    return true;
                }
            }
            return false;
        }

        private class Rating {
            public int X, XE, M, ME, A, AE, S, SE, Total;
            public string Result;
            public Rating() {
                X = 1; XE = 4000;
                M = 1; ME = 4000;
                A = 1; AE = 4000;
                S = 1; SE = 4000;
                Total = 0;
                Result = "in";
            }
            public Rating(string data) {
                string[] info = data.SplitOn("{x=", ",m=", ",a=", ",s=", "}");
                X = info[1].ToInt();
                XE = X;
                M = info[2].ToInt();
                ME = M;
                A = info[3].ToInt();
                AE = A;
                S = info[4].ToInt();
                SE = S;
                Total = X + M + A + S;
                Result = "in";
            }
            public Rating(Rating copy) {
                X = copy.X;
                XE = copy.XE;
                M = copy.M;
                ME = copy.ME;
                A = copy.A;
                AE = copy.AE;
                S = copy.S;
                SE = copy.SE;
                Total = copy.Total;
            }
            public long Combinations() {
                return (long)(XE - X + 1) * (long)(ME - M + 1) * (long)(AE - A + 1) * (long)(SE - S + 1);
            }
            public override string ToString() {
                return $"x={X}-{XE},m={M}-{ME},a={A}-{AE},s={S}-{SE},r={Result}";
            }
        }
        private class WorkFlow {
            public string Name;
            public List<Condition> Conditions = new();

            public WorkFlow(string line) {
                int index = line.IndexOf('{');
                Name = line.Substring(0, index);
                int lastIndex = index;
                while ((index = line.IndexOf(',', index + 1)) > 0) {
                    Conditions.Add(new Condition(line.Substring(lastIndex + 1, index - lastIndex - 1)));
                    lastIndex = index;
                }
                Conditions.Add(new Condition(line.Substring(lastIndex + 1, line.Length - lastIndex - 2)));
            }
            public override string ToString() {
                StringBuilder sb = new();
                sb.Append($"{Name}");
                for (int i = 0; i < Conditions.Count; i++) {
                    sb.Append($"({Conditions[i]})");
                }
                return sb.ToString();
            }
        }
        private class Condition {
            public char Field;
            public int Amount;
            public OpType Operator;
            public string Result;
            public Condition(string data) {
                Operator = OpType.None;
                Result = data;
                char op = data.Length > 1 ? data[1] : '\0';
                if (op == '>') {
                    Operator = OpType.GreaterThan;
                } else if (op == '<') {
                    Operator = OpType.LessThan;
                }

                if (Operator != OpType.None) {
                    Field = data[0];
                    int successIndex = data.IndexOf(':');
                    Amount = data.Substring(2, successIndex - 2).ToInt();
                    Result = data.Substring(successIndex + 1);
                }
            }
            public override string ToString() {
                if (Operator == OpType.None) {
                    return $"{Result}";
                } else if (Operator == OpType.GreaterThan) {
                    return $"{Field} > {Amount} = {Result}";
                }
                return $"{Field} < {Amount} = {Result}";
            }
        }
        private enum OpType {
            None,
            LessThan,
            GreaterThan
        }
    }
}