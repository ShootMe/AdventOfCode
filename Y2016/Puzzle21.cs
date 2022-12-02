using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Scrambled Letters and Hash")]
    public class Puzzle21 : ASolver {
        private List<StringAction> actions;

        public override void Setup() {
            List<string> items = Input.Lines();
            actions = new List<StringAction>();

            for (int i = 0; i < items.Count; i++) {
                actions.Add(new StringAction(items[i]));
            }
        }

        [Description("What is the result of scrambling abcdefgh?")]
        public override string SolvePart1() {
            char[] password = "abcdefgh".ToCharArray();
            for (int i = 0; i < actions.Count; i++) {
                actions[i].Execute(password);
            }
            return new string(password);
        }

        [Description("What is the un-scrambled version of the scrambled password fbgdceah?")]
        public override string SolvePart2() {
            char[] original = "fbgdceah".ToCharArray();
            char[] password = "abcdefgh".ToCharArray();
            char[] test = new char[password.Length];

            for (int i = 0; i < password.Length; i++) {
                int next = i + 1;

                do {
                    Array.Copy(password, test, password.Length);

                    for (int j = 0; j < actions.Count; j++) {
                        actions[j].Execute(test);
                    }

                    if (test[i] == original[i]) { break; }

                    char temp = password[i];
                    password[i] = password[next];
                    password[next++] = temp;
                } while (true);

                while (i < password.Length && test[i] == original[i]) {
                    i++;
                }
            }

            return new string(password);
        }

        private enum ActionType {
            SwapPosition,
            SwapLetters,
            ReversePosition,
            RotateLeft,
            RotateRight,
            RotatePosition,
            MovePosition
        }
        private class StringAction {
            public ActionType Type;
            public byte Value1;
            public byte Value2;

            public StringAction(string description) {
                if (description.StartsWith("swap letter ")) {
                    Type = ActionType.SwapLetters;
                    Value1 = (byte)description[12];
                    Value2 = (byte)description[26];
                } else if (description.StartsWith("swap position ")) {
                    Type = ActionType.SwapPosition;
                    Value1 = (byte)(description[14] - '0');
                    Value2 = (byte)(description[30] - '0');
                } else if (description.StartsWith("reverse positions ")) {
                    Type = ActionType.ReversePosition;
                    Value1 = (byte)(description[18] - '0');
                    Value2 = (byte)(description[28] - '0');
                    if (Value1 > Value2) {
                        byte temp = Value1;
                        Value1 = Value2;
                        Value2 = temp;
                    }
                } else if (description.StartsWith("rotate left ")) {
                    Type = ActionType.RotateLeft;
                    Value1 = (byte)(description[12] - '0');
                } else if (description.StartsWith("rotate right ")) {
                    Type = ActionType.RotateRight;
                    Value1 = (byte)(description[13] - '0');
                } else if (description.StartsWith("rotate based ")) {
                    Type = ActionType.RotatePosition;
                    Value1 = (byte)description[35];
                } else if (description.StartsWith("move position ")) {
                    Type = ActionType.MovePosition;
                    Value1 = (byte)(description[14] - '0');
                    Value2 = (byte)(description[28] - '0');
                }
            }

            public void Execute(char[] value) {
                switch (Type) {
                    case ActionType.MovePosition: MovePosition(value); break;
                    case ActionType.ReversePosition: Array.Reverse(value, Value1, Value2 - Value1 + 1); break;
                    case ActionType.RotateLeft: RotateLeft(value, Value1); break;
                    case ActionType.RotateRight: RotateLeft(value, value.Length - (Value1 % value.Length)); break;
                    case ActionType.SwapPosition: SwapPosition(value, Value1, Value2); break;
                    case ActionType.SwapLetters: SwapLetters(value); break;
                    case ActionType.RotatePosition: RotatePosition(value); break;
                }
            }
            public void Undo(char[] value) {
                switch (Type) {
                    case ActionType.MovePosition: MovePosition(value); break;
                    case ActionType.ReversePosition: Array.Reverse(value, Value1, Value2 - Value1 + 1); break;
                    case ActionType.RotateLeft: RotateLeft(value, Value1); break;
                    case ActionType.RotateRight: RotateLeft(value, value.Length - (Value1 % value.Length)); break;
                    case ActionType.SwapPosition: SwapPosition(value, Value1, Value2); break;
                    case ActionType.SwapLetters: SwapLetters(value); break;
                    case ActionType.RotatePosition: RotatePosition(value); break;
                }
            }
            private void MovePosition(char[] value) {
                char temp = value[Value1];
                if (Value1 < Value2) {
                    for (int i = Value1; i < Value2; i++) {
                        value[i] = value[i + 1];
                    }
                } else {
                    for (int i = Value1; i > Value2; i--) {
                        value[i] = value[i - 1];
                    }
                }
                value[Value2] = temp;
            }
            private void RotateLeft(char[] value, int shift) {
                int amount = value.Length - (shift % value.Length);
                char[] temp = new char[value.Length];
                for (int i = value.Length - 1; i >= 0; i--) {
                    int index = i + amount;
                    if (index >= value.Length) { index -= value.Length; }

                    temp[index] = value[i];
                }

                Array.Copy(temp, value, value.Length);
            }
            private void SwapPosition(char[] value, int pos1, int pos2) {
                char temp = value[pos1];
                value[pos1] = value[pos2];
                value[pos2] = temp;
            }
            private void SwapLetters(char[] value) {
                int pos1 = 0;
                int pos2 = 0;
                for (int i = 0; i < value.Length; i++) {
                    char val = value[i];
                    if (val == Value1) {
                        pos1 = i;
                    } else if (val == Value2) {
                        pos2 = i;
                    }
                }
                SwapPosition(value, pos1, pos2);
            }
            private void RotatePosition(char[] value) {
                int pos1 = 0;
                for (int i = 0; i < value.Length; i++) {
                    char val = value[i];
                    if (val == Value1) {
                        pos1 = i;
                        break;
                    }
                }
                pos1 += pos1 >= 4 ? 2 : 1;
                RotateLeft(value, value.Length - (pos1 % value.Length));
            }
        }
    }
}