using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
                int day = puzzle.FullName.Substring(puzzle.FullName.Length - 2).ToInt();
                int pYear = puzzle.FullName.Substring(puzzle.FullName.Length - 13, 4).ToInt();

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
                        if (input.IndexOf("\r") >= 0) {
                            input = input.Replace("\r", string.Empty);
                            File.WriteAllText(filePath, input);
                        }

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

                        string onlyRunInput = GetInputName(puzzle);
                        if (string.IsNullOrEmpty(onlyRunInput) || onlyRunInput == inputName) {
                            ASolver solver = (ASolver)Activator.CreateInstance(puzzle);
                            solver.Input = input;
                            Stopwatch sw = new Stopwatch();

                            sw.Start();
                            solver.Setup();
                            sw.Stop();

                            totalTime += sw.ElapsedTicks / 1000;
                            totalTime += AdventDay(pYear, day, solver, sw.ElapsedTicks / 1000, inputName, answer1, answer2);
                        }
                    }
                }
            }
            Write($"--- Total Time:");
            WriteTime(totalTime);
        }
        private static long AdventDay(int year, int day, ASolver puzzle, long constructorMS, string inputName, string answer1, string answer2) {
            Type puzzleType = puzzle.GetType();

            Write($"Day {day}", ConsoleColor.Yellow);
            string puzzleName = GetDescription(puzzleType);
            if (!string.IsNullOrEmpty(puzzleName)) { Write($": {puzzleName}"); }
            Write($"  ({inputName})", ConsoleColor.Yellow);
            Write($" (Setup {(double)constructorMS / 10:0.0} ms)", ConsoleColor.Gray);
            WriteLine();

            Stopwatch sw = new Stopwatch();
            long timeTaken = 0;
            string description = GetDescription(puzzleType, "SolvePart1");
            if (!string.IsNullOrEmpty(description)) {
                Write("    ");
                WriteLine(description, ConsoleColor.Blue);

                Write("    ");
                sw.Start();
                string answer = puzzle.SolvePart1();
                sw.Stop();

                Write(string.IsNullOrEmpty(answer) ? "N/A" : answer, string.IsNullOrEmpty(answer) || (!string.IsNullOrEmpty(answer1) && !answer.Equals(answer1, StringComparison.OrdinalIgnoreCase)) ? ConsoleColor.Red : string.IsNullOrEmpty(answer1) ? ConsoleColor.Yellow : ConsoleColor.Cyan);
                WriteTime(sw.ElapsedTicks / 1000);
                timeTaken += sw.ElapsedTicks / 1000;

                if (inputName == "Given" && string.IsNullOrEmpty(answer1) && !string.IsNullOrEmpty(answer) && ShouldSubmit(puzzleType, "SolvePart1")) {
                    WriteSolutionResult(year, day, 1, answer);
                }
            }

            description = GetDescription(puzzleType, "SolvePart2");
            if (!string.IsNullOrEmpty(description)) {
                Write("    ");
                WriteLine(description, ConsoleColor.Blue);

                Write("    ");
                sw.Restart();
                string answer = puzzle.SolvePart2();
                sw.Stop();

                Write(string.IsNullOrEmpty(answer) ? "N/A" : answer, string.IsNullOrEmpty(answer) || (!string.IsNullOrEmpty(answer2) && !answer.Equals(answer2, StringComparison.OrdinalIgnoreCase)) ? ConsoleColor.Red : string.IsNullOrEmpty(answer2) ? ConsoleColor.Yellow : ConsoleColor.Cyan);
                WriteTime(sw.ElapsedTicks / 1000);
                timeTaken += sw.ElapsedTicks / 1000;

                if (inputName == "Given" && string.IsNullOrEmpty(answer2) && !string.IsNullOrEmpty(answer) && ShouldSubmit(puzzleType, "SolvePart2")) {
                    WriteSolutionResult(year, day, 2, answer);
                }
            }

            WriteLine();
            return timeTaken;
        }
        private static void WriteSolutionResult(int year, int day, int part, string answer) {
            Write("    Submitting Solution... ");
            string solutionResult = Tools.SubmitSolution(year, day, part, answer);
            WriteLine(solutionResult, solutionResult == "Correct" ? ConsoleColor.Green : ConsoleColor.Red);
        }
        private static void WriteTime(long time) {
            Color cc;
            if (time >= 20000) {
                cc = Color.FromArgb(255, 0, 0);
            } else if (time >= 5000) {
                int green = -(int)(255d * (time - 5000) / 15000d);
                cc = Color.FromArgb(255, 255 + green, 0);
            } else {
                int red = (int)(255d * time / 5000d);
                cc = Color.FromArgb(red, 255, 0);
            }

            bool bold = false;
            Console.WriteLine($"\x1b[38;2;{cc.R};{cc.G};{cc.B}{(bold ? ";1" : "")}m{$" ({(double)time / 10:0.0} ms)"}\x1b[0m");
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
        private static bool ShouldSubmit(Type type, string methodName) {
            MemberInfo info = type.GetMethod(methodName);
            if (info == null) { return false; }

            SubmitAttribute submit = (SubmitAttribute)info.GetCustomAttribute(typeof(SubmitAttribute), false);
            return submit != null;
        }
        private static string GetDescription(Type type, string methodName) {
            MemberInfo info = type.GetMethod(methodName);
            if (info == null) { return string.Empty; }

            DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttribute(typeof(DescriptionAttribute), false);
            return description == null ? string.Empty : description.Description;
        }
        private static string GetDescription(Type type) {
            DescriptionAttribute description = (DescriptionAttribute)type.GetCustomAttribute(typeof(DescriptionAttribute), false);
            return description == null ? string.Empty : description.Description;
        }
        private static string GetInputName(Type type) {
            RunAttribute inputName = (RunAttribute)type.GetCustomAttribute(typeof(RunAttribute), false);
            return inputName == null ? string.Empty : inputName.InputName;
        }
    }
}