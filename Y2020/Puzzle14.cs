using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Docking Data")]
    public class Puzzle14 : ASolver {
        private List<string> items;

        public override void Setup() {
            items = Tools.GetLines(Input);
        }

        [Description("What is the sum of all values left in memory after it completes?")]
        public override string SolvePart1() {
            long orValue = 0;
            long andValue = 0;
            Dictionary<int, long> memory = new Dictionary<int, long>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                if (item[1] == 'a') {
                    long mask = 0x800000000;
                    andValue = 0;
                    orValue = 0;
                    for (int j = 7; j < item.Length; j++) {
                        switch (item[j]) {
                            case '1': orValue |= mask; break;
                            case 'X': andValue |= mask; break;
                        }
                        mask >>= 1;
                    }
                } else {
                    string[] splits = item.SplitOn("[", "]", " = ");
                    int address = splits[1].ToInt();
                    long value = splits[3].ToInt();
                    if (memory.ContainsKey(address)) {
                        memory[address] = (value & andValue) | orValue;
                    } else {
                        memory.Add(address, (value & andValue) | orValue);
                    }
                }
            }

            long sum = 0;
            foreach (long value in memory.Values) {
                sum += value;
            }
            return $"{sum}";
        }

        [Description("What is the sum of all values left in memory after it completes?")]
        public override string SolvePart2() {
            long orValue = 0;
            long andValue = 0;
            Dictionary<long, long> memory = new Dictionary<long, long>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                if (item[1] == 'a') {
                    long mask = 0x800000000;
                    andValue = 0;
                    orValue = 0;
                    for (int j = 7; j < item.Length; j++) {
                        switch (item[j]) {
                            case '1': orValue |= mask; break;
                            case 'X': andValue |= mask; break;
                        }
                        mask >>= 1;
                    }
                } else {
                    string[] splits = item.SplitOn("[", "]", " = ");
                    long address = splits[1].ToLong();
                    long value = splits[3].ToLong();
                    address = (address | orValue) & ~andValue;
                    Enumerate(memory, andValue, 35, address, value);
                }
            }

            long sum = 0;
            foreach (long value in memory.Values) {
                sum += value;
            }
            return $"{sum}";
        }

        private void Enumerate(Dictionary<long, long> memory, long mask, int index, long address, long value) {
            if (index < 0) {
                if (!memory.ContainsKey(address)) {
                    memory.Add(address, value);
                } else {
                    memory[address] = value;
                }
                return;
            }

            long bit = 1L << index;
            while (bit != 0 && (mask & bit) == 0) {
                bit >>= 1;
                index--;
            }

            Enumerate(memory, mask, index - 1, address, value);
            Enumerate(memory, mask, index - 1, address | bit, value);
        }
    }
}