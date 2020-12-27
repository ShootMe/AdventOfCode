using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
namespace AdventOfCode.Core {
    public static class Runner {
        public static void AdventYear(int year = 0, int dayToRun = 0) {
            Type[] types = typeof(Tools).Assembly.GetTypes();
            List<Type> puzzles = new List<Type>();

            string adventYear = year == 0 ? $"AdventOfCode.Y" : $"AdventOfCode.Y{year}.Puzzle";
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                if (!type.IsNestedPrivate && typeof(ASolver).IsAssignableFrom(type) && type.FullName.IndexOf(adventYear, StringComparison.OrdinalIgnoreCase) == 0) {
                    puzzles.Add(type);
                }
            }

            year = 0;
            puzzles.Sort(delegate (Type left, Type right) {
                return left.FullName.CompareTo(right.FullName);
            });

            long totalTime = 0;
            for (int i = 0; i < puzzles.Count; i++) {
                Type puzzle = puzzles[i];
                int day = Tools.ParseInt(puzzle.FullName, puzzle.FullName.Length - 2);
                int pYear = Tools.ParseInt(puzzle.FullName, puzzle.FullName.Length - 13, 4);

                if (pYear != year) {
                    if (totalTime > 0) {
                        Write($"--- Total Time:");
                        WriteTime(totalTime);
                        WriteLine();
                    }

                    Write($"--- Puzzles");
                    Write($" {pYear}", ConsoleColor.Yellow);
                    WriteLine($" ---");
                    WriteLine();
                    year = pYear;
                    totalTime = 0;
                }

                if (dayToRun == 0 || dayToRun == day) {
                    string[] files = Directory.GetFiles(@$"Y{pYear}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
                    for (int j = 0; j < files.Length; j++) {
                        string filePath = files[j];
                        string input = File.ReadAllText(filePath);

                        int dotIndex = filePath.LastIndexOf('.');
                        int index1 = filePath.IndexOf($"puzzle{day:00}");
                        int index2 = filePath.IndexOf('-', index1);
                        int index3 = filePath.IndexOf('-', index2 + 1);
                        int index4 = filePath.IndexOf('-', index3 + 1);

                        if (index3 < 0) {
                            index3 = dotIndex;
                        }
                        if (index4 < 0) {
                            index4 = dotIndex;
                        }

                        string answer1 = string.Empty;
                        if (index2 > 0) {
                            answer1 = filePath.Substring(index2 + 1, index3 - index2 - 1);
                        }

                        string answer2 = string.Empty;
                        if (index3 < index4) {
                            answer2 = filePath.Substring(index3 + 1, index4 - index3 - 1);
                        }

                        string inputName = "Given";
                        if (index4 < dotIndex) {
                            inputName = filePath.Substring(index4 + 1, dotIndex - index4 - 1);
                        }

                        ASolver solver = (ASolver)Activator.CreateInstance(puzzle, input);
                        Stopwatch sw = new Stopwatch();

                        sw.Start();
                        solver.Setup();
                        sw.Stop();

                        totalTime += sw.ElapsedTicks / 1000;
                        totalTime += AdventDay(day, solver, sw.ElapsedTicks / 1000, inputName, answer1, answer2);
                    }
                }
            }
            Write($"--- Total Time:");
            WriteTime(totalTime);
        }
        private static long AdventDay(int day, ASolver puzzle, long constructorMS, string inputName, string answer1, string answer2) {
            Write($"Day {day}", ConsoleColor.Yellow);
            if (!string.IsNullOrEmpty(puzzle.Name)) { Write($": {puzzle.Name}"); }
            Write($"  ({inputName})", ConsoleColor.Yellow);
            WriteLine();

            Stopwatch sw = new Stopwatch();
            long timeTaken = 0;
            string description = GetDescription($"{puzzle.GetType().FullName}.SolvePart1");
            if (!string.IsNullOrEmpty(description)) {
                Write("    ");
                WriteLine(description, ConsoleColor.Blue);

                Write("    ");
                sw.Start();
                string answer = puzzle.SolvePart1();
                sw.Stop();

                Write(string.IsNullOrEmpty(answer) ? "N/A" : answer, string.IsNullOrEmpty(answer) || (!string.IsNullOrEmpty(answer1) && !answer.Equals(answer1, StringComparison.OrdinalIgnoreCase)) ? ConsoleColor.Red : string.IsNullOrEmpty(answer1) ? ConsoleColor.Yellow : ConsoleColor.Cyan);
                WriteTime(sw.ElapsedTicks / 1000 + constructorMS);
                timeTaken += sw.ElapsedTicks / 1000;
            }

            description = GetDescription($"{puzzle.GetType().FullName}.SolvePart2");
            if (!string.IsNullOrEmpty(description)) {
                Write("    ");
                WriteLine(description, ConsoleColor.Blue);

                Write("    ");
                sw.Restart();
                string answer = puzzle.SolvePart2();
                sw.Stop();

                Write(string.IsNullOrEmpty(answer) ? "N/A" : answer, string.IsNullOrEmpty(answer) || (!string.IsNullOrEmpty(answer2) && !answer.Equals(answer2, StringComparison.OrdinalIgnoreCase)) ? ConsoleColor.Red : string.IsNullOrEmpty(answer2) ? ConsoleColor.Yellow : ConsoleColor.Cyan);
                WriteTime(sw.ElapsedTicks / 1000 + constructorMS);
                timeTaken += sw.ElapsedTicks / 1000;
            }

            WriteLine();
            return timeTaken;
        }
        private static void WriteTime(long time) {
            WriteLine($" ({(double)time / 10:0.0} ms)", time > 10000 ? ConsoleColor.Red : time > 5000 ? ConsoleColor.Yellow : time > 1000 ? ConsoleColor.Cyan : ConsoleColor.Green);
        }
        public static void Write(string text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black) {
            ConsoleColor currentFore = Console.ForegroundColor;
            ConsoleColor currentBack = Console.BackgroundColor;
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.Write(text);
            Console.ForegroundColor = currentFore;
            Console.BackgroundColor = currentBack;
        }
        public static void WriteLine(string text = null, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black) {
            ConsoleColor currentFore = Console.ForegroundColor;
            ConsoleColor currentBack = Console.BackgroundColor;
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(text);
            Console.ForegroundColor = currentFore;
            Console.BackgroundColor = currentBack;
        }
        private static string GetDescription(string fullName) {
            Type[] types = typeof(Tools).Assembly.GetTypes();

            MemberInfo info = null;
            int index = fullName.LastIndexOf('.');
            string method = fullName.Substring(index + 1);
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                if (fullName.IndexOf(type.FullName, StringComparison.OrdinalIgnoreCase) == 0) {
                    info = type.GetMethod(method);
                }
            }

            if (info == null) { return string.Empty; }
            DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttribute(typeof(DescriptionAttribute), false);
            return description == null ? string.Empty : description.Description;
        }
    }
}