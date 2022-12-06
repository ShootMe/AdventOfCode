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

            string adventYear = year == 0 ? $"AdventOfCode.Y" : dayToRun == 0 ? $"AdventOfCode.Y{year}.Puzzle" : $"AdventOfCode.Y{year}.Puzzle{dayToRun:00}";
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                if (!type.IsNestedPrivate && typeof(ASolver).IsAssignableFrom(type) && type.FullName.IndexOf(adventYear, StringComparison.OrdinalIgnoreCase) == 0) {
                    puzzles.Add(type);
                }
            }

            if (dayToRun > 0 && year > 0 && puzzles.Count == 0) {
                Tools.DownloadProblem(year, dayToRun, true);
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
                    if (files.Length == 0 && dayToRun > 0) {
                        Tools.DownloadProblem(pYear, dayToRun, false);
                        files = Directory.GetFiles(@$"Y{pYear}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
                    }
                    for (int j = 0; j < files.Length; j++) {
                        string filePath = files[j];
                        string input = File.ReadAllText(filePath);
                        if (input.IndexOf('\r') >= 0) {
                            input = input.Replace("\r", string.Empty);
                            File.WriteAllText(filePath, input);
                        }

                        int dotIndex = filePath.LastIndexOf('.');
                        int indexPart1 = filePath.IndexOf('-');
                        int indexPart2 = filePath.IndexOf('-', indexPart1 + 1);
                        indexPart2 = indexPart2 < 0 ? dotIndex : indexPart2;
                        int indexDesc = filePath.IndexOf('-', indexPart2 + 1);
                        indexDesc = indexDesc < 0 ? dotIndex : indexDesc;

                        string answer1 = indexPart1 > 0 ? filePath.Substring(indexPart1 + 1, indexPart2 - indexPart1 - 1) : string.Empty;
                        string answer2 = indexPart2 < indexDesc ? filePath.Substring(indexPart2 + 1, indexDesc - indexPart2 - 1) : string.Empty;
                        string inputName = indexDesc >= dotIndex ? "Given" : filePath.Substring(indexDesc + 1, dotIndex - indexDesc - 1);

                        string onlyRunInput = GetInputName(puzzle);
                        if (string.IsNullOrEmpty(onlyRunInput) || onlyRunInput == inputName) {
                            ASolver solver = (ASolver)Activator.CreateInstance(puzzle);
                            solver.Input = input;
                            Stopwatch sw = new Stopwatch();

                            sw.Start();
                            solver.Setup();
                            sw.Stop();

                            totalTime += sw.ElapsedTicks / 100;
                            totalTime += AdventDay(pYear, day, solver, sw.ElapsedTicks / 100, inputName, answer1, answer2);
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
            Write($" (Setup {(double)constructorMS / 100:0.00} ms)", ConsoleColor.Gray);
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
                WriteTime(sw.ElapsedTicks / 100);
                timeTaken += sw.ElapsedTicks / 100;

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
                WriteTime(sw.ElapsedTicks / 100);
                timeTaken += sw.ElapsedTicks / 100;

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
            if (time >= 200000) {
                cc = Color.FromArgb(255, 0, 0);
            } else if (time >= 50000) {
                int green = -(int)(255d * (time - 50000) / 150000d);
                cc = Color.FromArgb(255, 255 + green, 0);
            } else {
                int red = (int)(255d * time / 50000d);
                cc = Color.FromArgb(red, 255, 0);
            }

            bool bold = false;
            Console.WriteLine($"\x1b[38;2;{cc.R};{cc.G};{cc.B}{(bold ? ";1" : "")}m{$" ({(double)time / 100:0.00} ms)"}\x1b[0m");
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