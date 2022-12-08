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
                    string basePath = Tools.GetSolutionRootPath();
                    string[] files = Directory.GetFiles(@$"{basePath}Y{pYear}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
                    int givenCount = 0;
                    for (int j = 0; j < files.Length; j++) {
                        if (!files[j].EndsWith("-Example.txt", StringComparison.OrdinalIgnoreCase)) {
                            givenCount++;
                        }
                    }
                    if (givenCount == 0 && dayToRun > 0) {
                        Tools.DownloadProblem(pYear, dayToRun, false);
                        files = Directory.GetFiles(@$"{basePath}Y{pYear}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
                    }
                    for (int j = 0; j < files.Length; j++) {
                        string filePath = files[j];
                        InputData input = new InputData(filePath);

                        string onlyRunInput = GetInputName(puzzle);
                        if (string.IsNullOrEmpty(onlyRunInput) || onlyRunInput == input.Title) {
                            ASolver solver = (ASolver)Activator.CreateInstance(puzzle);
                            solver.Input = input.Data;
                            Stopwatch sw = new Stopwatch();

                            sw.Start();
                            solver.Setup();
                            sw.Stop();

                            totalTime += sw.ElapsedTicks / 100;
                            totalTime += AdventDay(pYear, day, solver, sw.ElapsedTicks / 100, input);
                        }
                    }
                }
            }
            Write($"--- Total Time:");
            WriteTime(totalTime);
        }
        private static long AdventDay(int year, int day, ASolver puzzle, long constructorMS, InputData input) {
            Type puzzleType = puzzle.GetType();

            Write($"Day {day}", ConsoleColor.Yellow);
            string puzzleName = GetDescription(puzzleType);
            if (!string.IsNullOrEmpty(puzzleName)) { Write($": {puzzleName}"); }
            Write($"  ({input.Title})", ConsoleColor.Yellow);
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

                Write(string.IsNullOrEmpty(answer) ? "N/A" : answer, string.IsNullOrEmpty(answer) || (!string.IsNullOrEmpty(input.Answer1) && !answer.Equals(input.Answer1, StringComparison.OrdinalIgnoreCase)) ? ConsoleColor.Red : string.IsNullOrEmpty(input.Answer1) ? ConsoleColor.Yellow : ConsoleColor.Cyan);
                WriteTime(sw.ElapsedTicks / 100);
                timeTaken += sw.ElapsedTicks / 100;

                if (input.Title == "Given" && string.IsNullOrEmpty(input.Answer1) && !string.IsNullOrEmpty(answer) && ShouldSubmit(puzzleType, "SolvePart1")) {
                    if (WriteSolutionResult(year, day, 1, answer)) {
                        string oldPath = input.FullPath;
                        input.Answer1 = answer;
                        input.Name = $"puzzle{day:00}-{input.Answer1}-{input.Answer2}.txt";
                        File.Move(oldPath, input.FullPath);
                    }
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

                Write(string.IsNullOrEmpty(answer) ? "N/A" : answer, string.IsNullOrEmpty(answer) || (!string.IsNullOrEmpty(input.Answer2) && !answer.Equals(input.Answer2, StringComparison.OrdinalIgnoreCase)) ? ConsoleColor.Red : string.IsNullOrEmpty(input.Answer2) ? ConsoleColor.Yellow : ConsoleColor.Cyan);
                WriteTime(sw.ElapsedTicks / 100);
                timeTaken += sw.ElapsedTicks / 100;

                if (input.Title == "Given" && string.IsNullOrEmpty(input.Answer2) && !string.IsNullOrEmpty(answer) && ShouldSubmit(puzzleType, "SolvePart2")) {
                    if (WriteSolutionResult(year, day, 2, answer)) {
                        string oldPath = input.FullPath;
                        input.Answer2 = answer;
                        input.Name = $"puzzle{day:00}-{input.Answer1}-{input.Answer2}.txt";
                        File.Move(oldPath, input.FullPath);
                    }
                }
            }

            WriteLine();
            return timeTaken;
        }
        private static bool WriteSolutionResult(int year, int day, int part, string answer) {
            Write("    Submitting Solution... ");
            string solutionResult = Tools.SubmitSolution(year, day, part, answer);
            WriteLine(solutionResult, solutionResult == "Correct" ? ConsoleColor.Green : ConsoleColor.Red);
            return solutionResult == "Correct";
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
        private class InputData {
            public string BasePath;
            public string Name;
            public string Title;
            public string Answer1;
            public string Answer2;
            public string Data;
            public string FullPath { get { return Path.Combine(BasePath, Name); } }

            public InputData(string path) {
                BasePath = Path.GetDirectoryName(path);
                Name = Path.GetFileName(path);

                Data = File.ReadAllText(path);
                if (Data.IndexOf('\r') >= 0) {
                    Data = Data.Replace("\r", string.Empty);
                    File.WriteAllText(path, Data);
                }

                int extIndex = path.LastIndexOf('.');
                int part1 = path.IndexOf('-');
                int part2 = path.IndexOf('-', part1 + 1);
                part2 = part2 < 0 || part2 > extIndex ? extIndex : part2;
                int desc = path.IndexOf('-', part2 + 1);
                desc = desc < 0 || desc > extIndex ? extIndex : desc;

                Answer1 = part1 > 0 ? path.Substring(part1 + 1, part2 - part1 - 1) : string.Empty;
                Answer2 = part2 < desc ? path.Substring(part2 + 1, desc - part2 - 1) : string.Empty;
                Title = desc >= extIndex ? "Given" : path.Substring(desc + 1, extIndex - desc - 1);
            }
        }
    }
}