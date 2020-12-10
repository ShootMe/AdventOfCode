using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
namespace AdventOfCode.Core {
    public static class Tools {
        public static void GeneratePuzzleTemplate(string path, string session, int year, int day) {
            Console.WriteLine($"Generating Template for {year}-{day}");

            using (ZipWebClient web = new ZipWebClient()) {
                web.Headers.Add("cookie", $"session={session}");

                byte[] data = web.DownloadData($"https://adventofcode.com/{year}/day/{day}");

                string html = Encoding.Default.GetString(data);
                int index1 = html.IndexOf("<article", StringComparison.OrdinalIgnoreCase);
                int index2 = html.IndexOf("</article>", StringComparison.OrdinalIgnoreCase);
                int index5 = html.IndexOf("<article", index1 + 1, StringComparison.OrdinalIgnoreCase);
                int index6 = html.IndexOf("</article>", index2 + 1, StringComparison.OrdinalIgnoreCase);
                if (index1 < 0 || index2 < 0) {
                    Console.WriteLine($"Failed to download Puzzle data {year}-{day}", ConsoleColor.Red);
                    return;
                }

                string dayHeader = $"<h2>--- Day {day}: ";
                int index3 = html.IndexOf(dayHeader);
                int index4 = html.IndexOf(" ---</h2>", index3);
                string dayTitle = html.Substring(index3 + dayHeader.Length, index4 - index3 - dayHeader.Length);

                Directory.CreateDirectory(Path.Combine(path, $"Y{year}"));
                Directory.CreateDirectory(Path.Combine(path, $"Y{year}\\Inputs"));
                Directory.CreateDirectory(Path.Combine(path, $"Y{year}\\Descriptions"));

                string classPath = Path.Combine(path, $"Y{year}\\Puzzle{day:00}.cs");
                if (!File.Exists(classPath)) {
                    string templateClass =
@$"using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y{year} {{
    public class Puzzle{day:00} : ASolver {{
        private List<string> items;
        public Puzzle{day:00}(string input) : base(input) {{ Name = ""{dayTitle}""; }}

        public override void Setup() {{
             items = Tools.GetLines(Input);
        }}

        [Description(""What is the answer?"")]
        public override string SolvePart1() {{
            for (int i = 0; i < items.Count; i++) {{
                
            }}
            return string.Empty;
        }}

        [Description(""What is the answer?"")]
        public override string SolvePart2() {{
            return string.Empty;
        }}
    }}
}}";
                    File.WriteAllText(classPath, templateClass);
                }

                string secondPart = index5 > 0 ? html.Substring(index5, index6 - index5 + 10) : string.Empty;
                string descriptionHtml =
$@"<head>
<link rel=""stylesheet"" type=""text/css"" href=""../../style.css""/>
</head>
{html.Substring(index1, index2 - index1 + 10)}
{secondPart}".TrimEnd();

                string descriptionPath = Path.Combine(path, $@"Y{year}\Descriptions\puzzle{day:00}.html");
                File.WriteAllText(descriptionPath, descriptionHtml);

                string[] files = Directory.GetFiles(@$"Y{year}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
                if (files.Length == 0) {
                    data = web.DownloadData($"https://adventofcode.com/{year}/day/{day}/input");
                    html = Encoding.Default.GetString(data);
                    File.WriteAllText(Path.Combine(path, $@"Y{year}\Inputs\puzzle{day:00}--.txt"), html.TrimEnd());
                }
            }
        }
        public static string GetInput(string filePath) {
            if (!File.Exists(filePath)) { return string.Empty; }

            using (StreamReader sr = new StreamReader(filePath)) {
                return sr.ReadToEnd();
            }
        }
        public static List<string> GetLines(string input) {
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (c == '\n') {
                    lines.Add(line.ToString());
                    line.Clear();
                } else if (c != '\r') {
                    line.Append(c);
                }
            }
            lines.Add(line.ToString());
            return lines;
        }
        public static List<string> GetSections(string input, char newLineReplacement = '\n') {
            List<string> sections = new List<string>();
            StringBuilder section = new StringBuilder();
            char last = '\0';
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (c == '\n' && last == '\n') {
                    sections.Add(section.ToString());
                    section.Clear();
                    last = '\0';
                } else if (c != '\r') {
                    if (c != '\n') {
                        if (last == '\n') {
                            section.Append(newLineReplacement);
                        }
                        section.Append(c);
                    }
                    last = c;
                }
            }
            sections.Add(section.ToString());
            return sections;
        }
        public static int[] GetNumbers(string input, char splitChar = '\n') {
            List<int> numbers = new List<int>();
            int startIndex = 0;
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (c == splitChar) {
                    numbers.Add(ParseInt(input, startIndex, i - startIndex));
                    startIndex = i + 1;
                }
            }
            numbers.Add(ParseInt(input, startIndex));
            return numbers.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ParseLong(string number) {
            if (string.IsNullOrEmpty(number)) { return 0; }

            int i = 0;
            while (i < number.Length && !char.IsDigit(number[i])) {
                i++;
            }

            bool isNegative = i > 0 ? number[i - 1] == '-' : false;
            long result = 0;
            while (i < number.Length) {
                byte value = (byte)number[i];
                if (value >= 0x30 && value <= 0x39) {
                    result = result * 10 + value - 0x30;
                }
                i++;
            }
            return isNegative ? -result : result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number) {
            if (string.IsNullOrEmpty(number)) { return 0; }

            int i = 0;
            while (i < number.Length && !char.IsDigit(number[i])) {
                i++;
            }

            bool isNegative = i > 0 ? number[i - 1] == '-' : false;
            int result = 0;
            while (i < number.Length) {
                byte value = (byte)number[i];
                if (value >= 0x30 && value <= 0x39) {
                    result = result * 10 + value - 0x30;
                }
                i++;
            }
            return isNegative ? -result : result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, int startIndex) {
            int length = number.Length - startIndex;
            if (length <= 0) { return 0; }

            int i = 0;
            while (i < length && !char.IsDigit(number[i + startIndex])) {
                i++;
            }

            bool isNegative = i > 0 ? number[i + startIndex - 1] == '-' : false;
            int result = 0;
            while (i < length) {
                byte value = (byte)number[i + startIndex];
                if (value >= 0x30 && value <= 0x39) {
                    result = result * 10 + value - 0x30;
                }
                i++;
            }
            return isNegative ? -result : result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, int startIndex, int length) {
            if (length <= 0) { return 0; }

            int i = 0;
            while (i < length && !char.IsDigit(number[i + startIndex])) {
                i++;
            }

            bool isNegative = i > 0 ? number[i + startIndex - 1] == '-' : false;
            int result = 0;
            while (i < length) {
                byte value = (byte)number[i + startIndex];
                if (value >= 0x30 && value <= 0x39) {
                    result = result * 10 + value - 0x30;
                }
                i++;
            }
            return isNegative ? -result : result;
        }
        public static void For<T>(IEnumerable<T> items, Action<T> action) {
            foreach (T item in items) {
                action(item);
            }
        }
        public static IEnumerable<T[]> Permute<T>(T[] nums) {
            return DoPermute(nums, 0, nums.Length - 1);
        }
        private static IEnumerable<T[]> DoPermute<T>(T[] nums, int start, int end) {
            if (start == end) {
                yield return nums;
            } else {
                for (var i = start; i <= end; i++) {
                    Swap(ref nums[start], ref nums[i]);
                    foreach (T[] seq in DoPermute(nums, start + 1, end)) {
                        yield return seq;
                    }
                    Swap(ref nums[start], ref nums[i]);
                }
            }
        }
        private static void Swap<T>(ref T a, ref T b) {
            var temp = a;
            a = b;
            b = temp;
        }
        public static int GCD(int a, int b) {
            if (a == 0 && b == 0) { return 1; }
            a = a < 0 ? -a : a;
            b = b < 0 ? -b : b;
            if (a == 0) { return b; }
            if (b == 0) { return a; }

            int r;
            if (b > a) {
                r = b % a;
                if (r == 0) { return a; }
                if (r == 1) { return 1; }
                b = r;
            }
            while (true) {
                r = a % b;
                if (r == 0) { return b; }
                if (r == 1) { return 1; }
                a = b;
                b = r;
            }
        }
    }
}