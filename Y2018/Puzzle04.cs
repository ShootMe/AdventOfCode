using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Repose Record")]
    public class Puzzle04 : ASolver {
        private LogRecord[] records;
        private Dictionary<int, int> countToGuard = new Dictionary<int, int>();
        private int[] sleepTime;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            records = new LogRecord[items.Count];
            for (int i = 0; i < records.Length; i++) {
                records[i] = items[i];
            }
            Array.Sort(records);

            Dictionary<int, int> guards = new Dictionary<int, int>();
            int lastID = 0;
            int guardCount = 0;
            for (int i = 0; i < records.Length; i++) {
                LogRecord record = records[i];
                if (record.Action == LogAction.StartShift) {
                    lastID = record.ID;
                    if (!guards.ContainsKey(record.ID)) {
                        countToGuard.Add(guardCount, record.ID);
                        guards.Add(record.ID, guardCount++);
                    }
                }
                record.ID = lastID;
            }

            sleepTime = new int[guards.Count * 60];

            DateTime startSleep = DateTime.MinValue;
            lastID = 0;
            for (int i = 0; i < records.Length; i++) {
                LogRecord record = records[i];
                if (record.Action == LogAction.StartShift) {
                    lastID = record.ID;
                    startSleep = DateTime.MinValue;
                } else if (record.Action == LogAction.FallAsleep) {
                    startSleep = record.Time;
                } else if (record.Action == LogAction.WakeUp) {
                    if (startSleep != DateTime.MinValue) {
                        record.FillTime(sleepTime, guards[lastID] * 60 + startSleep.Minute, startSleep);
                    }
                }
            }
        }

        [Description("What is the ID of the guard you chose multiplied by the minute you chose?")]
        public override string SolvePart1() {
            int maxGuardCount = 0;
            int bestValue = -1;
            for (int i = countToGuard.Count - 1; i >= 0; i--) {
                int maxCount = 0;
                int bestMinute = -1;
                int total = 0;
                for (int j = 0; j < 60; j++) {
                    int value = sleepTime[i * 60 + j];
                    total += value;
                    if (value > maxCount) {
                        maxCount = value;
                        bestMinute = j;
                    }
                }

                if (total > maxGuardCount) {
                    maxGuardCount = total;
                    bestValue = countToGuard[i] * bestMinute;
                }
            }
            return $"{bestValue}";
        }

        [Description("What is the ID of the guard you chose multiplied by the minute you chose?")]
        public override string SolvePart2() {
            int maxGuardCount = 0;
            int bestValue = -1;
            for (int i = countToGuard.Count - 1; i >= 0; i--) {
                int maxCount = 0;
                int bestMinute = -1;
                for (int j = 0; j < 60; j++) {
                    int value = sleepTime[i * 60 + j];
                    if (value > maxCount) {
                        maxCount = value;
                        bestMinute = j;
                    }
                }

                if (maxCount > maxGuardCount) {
                    maxGuardCount = maxCount;
                    bestValue = countToGuard[i] * bestMinute;
                }
            }
            return $"{bestValue}";
        }

        private enum LogAction {
            Unknown,
            FallAsleep,
            WakeUp,
            StartShift
        }
        private class LogRecord : IComparable<LogRecord> {
            public int ID;
            public DateTime Time;
            public LogAction Action;

            public void FillTime(int[] times, int start, DateTime startTime) {
                while (startTime != Time) {
                    times[start++]++;
                    startTime = startTime.AddMinutes(1);
                }
            }

            public static implicit operator LogRecord(string value) {
                LogRecord record = new LogRecord();
                record.Time = DateTime.Parse(value.Substring(1, 16));
                int index1;
                if ((index1 = value.IndexOf("begin")) > 0) {
                    int index2 = value.IndexOf("Guard");
                    record.ID = Tools.ParseInt(value.Substring(index2 + 7, index1 - index2 - 8));
                    record.Action = LogAction.StartShift;
                } else if (value.IndexOf("falls") > 0) {
                    record.Action = LogAction.FallAsleep;
                } else if (value.IndexOf("wakes") > 0) {
                    record.Action = LogAction.WakeUp;
                } else {
                    record.Action = LogAction.Unknown;
                }
                return record;
            }
            public int CompareTo(LogRecord other) {
                return Time.CompareTo(other.Time);
            }
            public override string ToString() {
                return $"[{Time:yyyy-MM-dd HH:mm}] {ID} {Action}";
            }
        }
    }
}