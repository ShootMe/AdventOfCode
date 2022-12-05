using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Supply Stacks")]
    public class Puzzle05 : ASolver {
        private CrateStack[] stacks;
        private List<(int amount, int from, int to)> moves = new();
        private int lastCrateLine;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            stacks = new CrateStack[(lines[0].Length + 1) / 4];

            lastCrateLine = 0;
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                if (line[1] == '1') {
                    lastCrateLine = i;
                    break;
                }
            }

            for (int i = 0; i < lastCrateLine; i++) {
                string line = lines[i];
                for (int j = 1; j < line.Length; j += 4) {
                    if (line[j] != ' ') {
                        CrateStack stack = stacks[j / 4];
                        if (stack == null) {
                            stack = new CrateStack(lastCrateLine * stacks.Length);
                            stacks[j / 4] = stack;
                            stack.Size = lastCrateLine - i;
                        }

                        stack.Stack[lastCrateLine - i - 1] = line[j];
                    }
                }
            }

            if (moves.Count == 0) {
                for (int i = lastCrateLine + 2; i < lines.Length; i++) {
                    string line = lines[i];
                    string[] splits = line.SplitOn("move ", " from ", " to ");
                    int amount = splits[1].ToInt();
                    int from = splits[2].ToInt() - 1;
                    int to = splits[3].ToInt() - 1;

                    moves.Add((amount, from, to));
                }
            }
        }

        [Description("What crate ends up on top of each stack?")]
        public override string SolvePart1() {
            for (int i = 0; i < moves.Count; i++) {
                (int amount, int from, int to) = moves[i];
                stacks[from].MoveTo(stacks[to], amount);
            }

            return TopOfStacks();
        }

        [Description("What crate ends up on top of each stack?")]
        public override string SolvePart2() {
            Setup();
            for (int i = 0; i < moves.Count; i++) {
                (int amount, int from, int to) = moves[i];
                stacks[from].MoveToAtOnce(stacks[to], amount);
            }

            return TopOfStacks();
        }

        private string TopOfStacks() {
            char[] result = new char[stacks.Length];
            for (int i = 0; i < stacks.Length; i++) {
                result[i] = stacks[i].Top;
            }
            return new string(result);
        }

        private class CrateStack {
            public char[] Stack;
            public int Size;

            public CrateStack(int maxSize) {
                Stack = new char[maxSize];
            }
            public char Top { get { return Stack[Size - 1]; } }
            public void MoveTo(CrateStack target, int amount) {
                Size -= amount;
                for (int i = Size + amount - 1; i >= Size; i--) {
                    target.Stack[target.Size++] = Stack[i];
                }
            }
            public void MoveToAtOnce(CrateStack target, int amount) {
                for (int i = Size - amount; i < Size; i++) {
                    target.Stack[target.Size++] = Stack[i];
                }
                Size -= amount;
            }
            public override string ToString() {
                return $"Size={Size} Top={Top}";
            }
        }
    }
}