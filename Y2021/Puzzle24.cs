using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
namespace AdventOfCode.Y2021 {
    [Description("Arithmetic Logic Unit")]
    public class Puzzle24 : ASolver {
        private ALU.CalculateSingle[] monads;

        public override void Setup() {
            monads = ALU.Compile(Input);
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart1() {
            return GetMonadNumber(true);
        }

        [Description("What is the smallest model number accepted by MONAD?")]
        public override string SolvePart2() {
            return GetMonadNumber(false);
        }

        private string GetMonadNumber(bool largest) {
            char[] finalDigits = new char[14];
            Stack<(int, int)> values = new Stack<(int, int)>();
            for (int i = 0; i < 14; i++) {
                int offset = (int)monads[i](0, 0);
                int check = -1;
                for (int j = 1; j < 26; j++) {
                    if (monads[i](0, j) == 0) {
                        check = j;
                        break;
                    }
                }

                if (check > 0) {
                    (int digit, int lastOffset) = values.Pop();
                    lastOffset -= check;
                    if (lastOffset <= 0) {
                        finalDigits[digit] = largest ? '9' : (char)('1' - lastOffset);
                        finalDigits[i] = largest ? (char)('9' + lastOffset) : '1';
                    } else {
                        finalDigits[digit] = largest ? (char)('9' - lastOffset) : '1';
                        finalDigits[i] = largest ? '9' : (char)('1' + lastOffset);
                    }
                    //Console.WriteLine($"Digit {i + 1,2} Offset={offset} Check={-check}");
                } else {
                    values.Push((i, offset));
                    //Console.WriteLine($"Digit {i + 1,2} Offset={offset}");
                }
            }

            return new string(finalDigits);
        }

        public static class ALU {
            public delegate long CalculateSingle(byte input, long z);

            public static CalculateSingle[] Compile(string program) {
                DynamicMethod[] methods = new DynamicMethod[14];
                for (int i = 0; i < methods.Length; i++) {
                    methods[i] = new DynamicMethod("ALUS", typeof(long), new Type[] { typeof(byte), typeof(long) });
                }

                ILGenerator il = null;
                Dictionary<char, int> localIndex = new Dictionary<char, int>() { { 'w', 0 }, { 'x', 1 }, { 'y', 2 }, { 'z', 3 } };
                List<string> lines = program.Lines();

                byte charIndex = 0;
                for (int i = 0; i < lines.Count; i++) {
                    string line = lines[i];

                    string[] splits = line.Split(' ');
                    string type = splits[0];
                    string value1 = splits[1];
                    string value2 = splits.Length > 2 ? splits[2] : string.Empty;
                    bool isOperand = false;
                    int val2;
                    if (int.TryParse(value2, out val2)) {
                        isOperand = true;
                    }

                    switch (type) {
                        case "inp":
                            if (il != null) {
                                il.Emit(OpCodes.Ldloc_3);
                                il.Emit(OpCodes.Ret);
                            }
                            il = methods[charIndex++].GetILGenerator();
                            il.DeclareLocal(typeof(long));//W
                            il.DeclareLocal(typeof(long));//X
                            il.DeclareLocal(typeof(long));//Y
                            il.DeclareLocal(typeof(long));//Z

                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Conv_I8);
                            il.Emit(OpCodes.Stloc, localIndex[value1[0]]);

                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Stloc, localIndex['z']);
                            break;
                        case "add":
                            il.Emit(OpCodes.Ldloc, localIndex[value1[0]]);
                            if (isOperand) {
                                il.Emit(OpCodes.Ldc_I8, (long)val2);
                            } else {
                                il.Emit(OpCodes.Ldloc, localIndex[value2[0]]);
                            }
                            il.Emit(OpCodes.Add);
                            il.Emit(OpCodes.Stloc, localIndex[value1[0]]);
                            break;
                        case "mul":
                            if (isOperand && val2 == 0) {//change (? *= 0) to (? = 0)
                                il.Emit(OpCodes.Ldc_I4_0);
                                il.Emit(OpCodes.Conv_I8);
                                il.Emit(OpCodes.Stloc, localIndex[value1[0]]);
                            } else {
                                il.Emit(OpCodes.Ldloc, localIndex[value1[0]]);
                                if (isOperand) {
                                    il.Emit(OpCodes.Ldc_I8, (long)val2);
                                } else {
                                    il.Emit(OpCodes.Ldloc, localIndex[value2[0]]);
                                }
                                il.Emit(OpCodes.Mul);
                                il.Emit(OpCodes.Stloc, localIndex[value1[0]]);
                            }
                            break;
                        case "div":
                            if (!isOperand || val2 != 1) {//ignore (? /= 1)
                                il.Emit(OpCodes.Ldloc, localIndex[value1[0]]);
                                if (isOperand) {
                                    il.Emit(OpCodes.Ldc_I8, (long)val2);
                                } else {
                                    il.Emit(OpCodes.Ldloc, localIndex[value2[0]]);
                                }
                                il.Emit(OpCodes.Div);
                                il.Emit(OpCodes.Stloc, localIndex[value1[0]]);
                            }
                            break;
                        case "mod":
                            il.Emit(OpCodes.Ldloc, localIndex[value1[0]]);
                            if (isOperand) {
                                il.Emit(OpCodes.Ldc_I8, (long)val2);
                            } else {
                                il.Emit(OpCodes.Ldloc, localIndex[value2[0]]);
                            }
                            il.Emit(OpCodes.Rem);
                            il.Emit(OpCodes.Stloc, localIndex[value1[0]]);
                            break;
                        case "eql":
                            il.Emit(OpCodes.Ldloc, localIndex[value1[0]]);
                            if (isOperand) {
                                il.Emit(OpCodes.Ldc_I8, (long)val2);
                            } else {
                                il.Emit(OpCodes.Ldloc, localIndex[value2[0]]);
                            }
                            Label beq1 = il.DefineLabel();
                            il.Emit(OpCodes.Beq_S, beq1);
                            il.Emit(OpCodes.Ldc_I4_0);
                            Label br0 = il.DefineLabel();
                            il.Emit(OpCodes.Br_S, br0);
                            il.MarkLabel(beq1);
                            il.Emit(OpCodes.Ldc_I4_1);
                            il.MarkLabel(br0);
                            il.Emit(OpCodes.Conv_I8);
                            il.Emit(OpCodes.Stloc, localIndex[value1[0]]);
                            break;
                    }
                }

                il.Emit(OpCodes.Ldloc_3);
                il.Emit(OpCodes.Ret);

                CalculateSingle[] singles = new CalculateSingle[14];
                for (int i = 0; i < singles.Length; i++) {
                    singles[i] = (CalculateSingle)methods[i].CreateDelegate(typeof(CalculateSingle));
                }
                return singles;
            }
        }
    }
}