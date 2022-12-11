using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
namespace AdventOfCode.Y2022 {
    [Description("Monkey in the Middle")]
    public class Puzzle11 : ASolver {
        private List<Monkey> monkeys;

        public override void Setup() {
            monkeys = new();
            string[] lines = Input.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                Monkey monkey = new Monkey();

                monkey.ID = lines[i++][7..].ToInt();
                string[] items = lines[i++][18..].Split(' ');
                for (int j = 0; j < items.Length; j++) {
                    monkey.Items.Add(items[j].ToInt());
                }

                DynamicMethod method = new DynamicMethod("Operation", typeof(long), new Type[] { typeof(long) });
                ILGenerator il = method.GetILGenerator();
                string operation = lines[i++][19..];

                string[] splits = operation.Split(' ');
                if (splits[0] == "old") {
                    il.Emit(OpCodes.Ldarg_0);
                } else {
                    il.Emit(OpCodes.Ldc_I8, splits[0].ToLong());
                }
                if (splits[2] == "old") {
                    il.Emit(OpCodes.Ldarg_0);
                } else {
                    il.Emit(OpCodes.Ldc_I8, splits[2].ToLong());
                }

                il.Emit(splits[1][0] switch { '+' => OpCodes.Add, '-' => OpCodes.Sub, '*' => OpCodes.Mul, '/' => OpCodes.Div, _ => OpCodes.Rem });
                il.Emit(OpCodes.Ret);

                monkey.Operation = (Monkey.CalculateOperation)method.CreateDelegate(typeof(Monkey.CalculateOperation));
                monkey.Test = lines[i++][19..].ToInt();
                monkey.True = lines[i++][29..].ToInt();
                monkey.False = lines[i++][30..].ToInt();
                monkeys.Add(monkey);
            }
        }

        [Description("What is the level of monkey business after 20 rounds of stuff-slinging simian shenanigans?")]
        public override string SolvePart1() {
            return $"{MonkeyBusiness(20)}";
        }

        [Description("What is the level of monkey business after 10000 rounds?")]
        public override string SolvePart2() {
            Setup();
            long lcm = monkeys[0].Test;
            for (int i = 1; i < monkeys.Count; i++) {
                lcm = lcm * monkeys[i].Test / Extensions.GCD(lcm, monkeys[i].Test);
            }
            return $"{MonkeyBusiness(10000, lcm)}";
        }
        private long MonkeyBusiness(int totalRounds, long divisor = 0) {
            int rounds = 0;
            while (rounds++ < totalRounds) {
                for (int i = 0; i < monkeys.Count; i++) {
                    Monkey monkey = monkeys[i];
                    for (int j = 0; j < monkey.Items.Count; j++) {
                        monkey.Inspected++;
                        long itemWorry = monkey.Items[j];
                        long newWorry = divisor == 0 ? monkey.Operation(itemWorry) / 3L : monkey.Operation(itemWorry) % divisor;
                        if ((newWorry % monkey.Test) == 0) {
                            monkeys[monkey.True].Items.Add(newWorry);
                        } else {
                            monkeys[monkey.False].Items.Add(newWorry);
                        }
                    }
                    monkey.Items.Clear();
                }
            }
            monkeys.Sort((left, right) => right.Inspected.CompareTo(left.Inspected));
            return (long)monkeys[0].Inspected * monkeys[1].Inspected;
        }

        private class Monkey {
            public delegate long CalculateOperation(long old);
            public int ID;
            public List<long> Items = new();
            public CalculateOperation Operation;
            public int Test;
            public int True;
            public int False;
            public int Inspected;
            public override string ToString() {
                return $"{ID} {Inspected}";
            }
        }
    }
}