using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Category Six")]
    public class Puzzle23 : ASolver {
        private List<IntCode> programs;

        public override void Setup() {
            long[] code = Input.ToLongs(',');

            programs = new List<IntCode>();
            for (int i = 0; i < 50; i++) {
                IntCode program = new IntCode(code);
                program.Run(i);
                programs.Add(program);
            }
        }

        [Description("What is the Y value of the first packet sent to address 255?")]
        public override string SolvePart1() {
            Dictionary<int, Queue<(long, long)>> instructions = new Dictionary<int, Queue<(long, long)>>();
            for (int i = 0; i < programs.Count; i++) {
                instructions.Add(i, new Queue<(long, long)>());
            }

            while (true) {
                for (int i = 0; i < programs.Count; i++) {
                    IntCode program = programs[i];

                    Queue<(long, long)> queue = instructions[i];
                    if (queue.Count == 0) {
                        program.WriteInput(-1);
                    }
                    while (queue.Count > 0) {
                        (long x, long y) result = queue.Dequeue();
                        program.WriteInput(result.x);
                        program.WriteInput(result.y);
                    }

                    program.Run();

                    while (program.HasOutput) {
                        int p = (int)program.Output;
                        program.Run();
                        long x = program.Output;
                        program.Run();
                        long y = program.Output;

                        //Console.WriteLine($"{p} ({x},{y})");
                        if (instructions.ContainsKey(p)) {
                            queue = instructions[p];
                            queue.Enqueue((x, y));
                        } else if (p == 255) {
                            return $"{y}";
                        }
                        program.Run();
                    }
                }
            }
        }

        [Description("What is the first Y value sent by the NAT to computer 0 twice in a row?")]
        public override string SolvePart2() {
            Dictionary<int, Queue<(long, long)>> instructions = new Dictionary<int, Queue<(long, long)>>();
            HashSet<long> natYs = new HashSet<long>();
            for (int i = 0; i < programs.Count; i++) {
                instructions.Add(i, new Queue<(long, long)>());
                programs[i].Reset();
                programs[i].Run(i);
            }

            long natY = -1, natX = 0;
            while (true) {
                for (int i = 0; i < programs.Count; i++) {
                    IntCode program = programs[i];

                    Queue<(long, long)> queue = instructions[i];
                    if (queue.Count == 0) {
                        program.WriteInput(-1);
                    }
                    while (queue.Count > 0) {
                        (long x, long y) result = queue.Dequeue();
                        program.WriteInput(result.x);
                        program.WriteInput(result.y);
                    }

                    program.Run();

                    while (program.HasOutput) {
                        int p = (int)program.Output;
                        program.Run();
                        long x = program.Output;
                        program.Run();
                        long y = program.Output;

                        //Console.WriteLine($"{p} ({x},{y})");
                        if (instructions.ContainsKey(p)) {
                            queue = instructions[p];
                            queue.Enqueue((x, y));
                        } else if (p == 255) {
                            natY = y;
                            natX = x;
                        }
                        program.Run();
                    }
                }

                bool hasPackets = false;
                foreach (Queue<(long, long)> instQueue in instructions.Values) {
                    if (instQueue.Count > 0) {
                        hasPackets = true;
                        break;
                    }
                }

                if (!hasPackets) {
                    if (!natYs.Add(natY)) {
                        return $"{natY}";
                    }
                    instructions[0].Enqueue((natX, natY));
                }
            }
        }
    }
}