using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Digital Plumber")]
    public class Puzzle12 : ASolver {
        private Dictionary<int, string[]> programs;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            programs = new Dictionary<int, string[]>();
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index = item.IndexOf(' ');
                int program = Tools.ParseInt(item, 0, index);
                programs.Add(program, item.Substring(index + 5).Split(", "));
            }
        }

        [Description("How many programs are in the group that contains program ID 0?")]
        public override string SolvePart1() {
            return $"{CountAndRemove(0)}";
        }

        [Description("How many groups are there in total?")]
        public override string SolvePart2() {
            int groups = 1;
            while (programs.Count > 0) {
                int nextProgram;
                using (var enumerator = programs.Keys.GetEnumerator()) {
                    enumerator.MoveNext();
                    nextProgram = enumerator.Current;
                }
                CountAndRemove(nextProgram);
                groups++;
            }
            return $"{groups}";
        }

        private int CountAndRemove(int program) {
            HashSet<int> seen = new HashSet<int>();
            seen.Add(program);
            Queue<int> toEvaluate = new Queue<int>();
            toEvaluate.Enqueue(program);
            while (toEvaluate.Count > 0) {
                program = toEvaluate.Dequeue();
                string[] pipes = programs[program];
                programs.Remove(program);

                for (int i = 0; i < pipes.Length; i++) {
                    int value = Tools.ParseInt(pipes[i]);
                    if (seen.Add(value)) {
                        toEvaluate.Enqueue(value);
                    }
                }
            }
            return seen.Count;
        }
    }
}