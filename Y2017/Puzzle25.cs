using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("The Halting Problem")]
    public class Puzzle25 : ASolver {
        private State[] states;
        private int state, checksumCount;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            state = items[0][15] - 'A';
            string[] splits = items[1].SplitOn("after ", " steps");
            checksumCount = splits[1].ToInt();

            states = new State[(items.Count - 2) / 10];
            int index = 0;
            State current = null;
            bool newState = true;
            bool zeroState = false;
            for (int i = 3; i < items.Count; i++) {
                string item = items[i];
                if (newState) {
                    newState = false;
                    current = new State() { ID = item[9] - 'A' };
                    states[index++] = current;
                    zeroState = true;
                    i++;
                    continue;
                } else if (zeroState) {
                    if (item.IndexOf("- Write the value ") > 0) {
                        current.ZeroWrite = (byte)(item[22] - '0');
                    } else if (item.IndexOf("- Move one slot to the ") > 0) {
                        current.ZeroMove = item[27] == 'r' ? 1 : -1;
                    } else {
                        current.ZeroState = item[26] - 'A';
                        i++;
                        zeroState = false;
                    }
                } else if (item.IndexOf("- Write the value ") > 0) {
                    current.OneWrite = (byte)(item[22] - '0');
                } else if (item.IndexOf("- Move one slot to the ") > 0) {
                    current.OneMove = item[27] == 'r' ? 1 : -1;
                } else {
                    current.OneState = item[26] - 'A';
                    i++;
                    newState = true;
                }
            }
        }

        [Description("What is the diagnostic checksum it produces once it's working again?")]
        public override string SolvePart1() {
            byte[] tape = new byte[20000];
            int index = tape.Length / 2;
            int currentState = state;
            for (int i = 0; i < checksumCount; i++) {
                State state = states[currentState];
                ref byte value = ref tape[index];
                if (value == 0) {
                    value = state.ZeroWrite;
                    currentState = state.ZeroState;
                    index += state.ZeroMove;
                } else {
                    value = state.OneWrite;
                    currentState = state.OneState;
                    index += state.OneMove;
                }
            }

            int count = 0;
            for (int i = 0; i < tape.Length; i++) {
                count += tape[i];
            }
            return $"{count}";
        }

        private class State {
            public int ID;
            public int ZeroMove;
            public byte ZeroWrite;
            public int ZeroState;
            public int OneMove;
            public byte OneWrite;
            public int OneState;

            public override string ToString() {
                return $"{(char)(ID + 'A')} 0: {ZeroMove} {ZeroWrite} {(char)(ZeroState + 'A')} 1: {OneMove} {OneWrite} {(char)(OneState + 'A')}";
            }
        }
    }
}