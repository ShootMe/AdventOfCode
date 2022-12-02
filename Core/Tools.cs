using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace AdventOfCode.Core {
    public static class Tools {
        private const string BaseURL = $"https://adventofcode.com/";
        private static HttpClient web;
        static Tools() {
            web = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.All, AllowAutoRedirect = false });
            web.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("github.com-ShootMe-AdventOfCode", "1.0"));
            web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            web.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SESSION"])) {
                web.DefaultRequestHeaders.Add("cookie", $"session={ConfigurationManager.AppSettings["SESSION"]}");
            }
        }
        private static string GetSolutionRootPath() {
            string runningExePath = AppDomain.CurrentDomain.BaseDirectory;
            return runningExePath.Substring(0, runningExePath.IndexOf(@"\bin\"));
        }
        public static void GeneratePuzzleTemplate(int year, int day) {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SESSION"])) {
                throw new ArgumentNullException("SESSION", "Missing SESSION code in the App.config");
            }

            Console.WriteLine($"Generating Template for {year}-{day}");

            string path = GetSolutionRootPath();
            string html = DownloadHtml($"{year}/day/{day}");
            int part1Start = html.IndexOf("<article", StringComparison.OrdinalIgnoreCase);
            int part1End = html.IndexOf("</article>", StringComparison.OrdinalIgnoreCase);
            int part2Start = html.IndexOf("<article", part1Start + 1, StringComparison.OrdinalIgnoreCase);
            int part2End = html.IndexOf("</article>", part1End + 1, StringComparison.OrdinalIgnoreCase);
            if (part1Start < 0 || part1End < 0) {
                Console.WriteLine($"Failed to download Puzzle data {year}-{day}", ConsoleColor.Red);
                return;
            }

            Directory.CreateDirectory(Path.Combine(path, $"Y{year}"));
            Directory.CreateDirectory(Path.Combine(path, $"Y{year}\\Inputs"));
            Directory.CreateDirectory(Path.Combine(path, $"Y{year}\\Descriptions"));

            string dayHeader = $"<h2>--- Day {day}: ";
            int dayHeaderStart = html.IndexOf(dayHeader);
            int dayHeaderEnd = html.IndexOf(" ---</h2>", dayHeaderStart);
            string dayTitle = html.Substring(dayHeaderStart + dayHeader.Length, dayHeaderEnd - dayHeaderStart - dayHeader.Length);

            int part1QuestionStart = html.LastIndexOf("<p>", part1End, StringComparison.OrdinalIgnoreCase);
            int part1QuestionEnd = html.LastIndexOf("</p>", part1End, StringComparison.OrdinalIgnoreCase);
            string part1Question = part1QuestionStart > part1Start && part1QuestionEnd > part1Start ? html.Substring(part1QuestionStart + 3, part1QuestionEnd - part1QuestionStart - 3).Replace("<em>", string.Empty).Replace("</em>", string.Empty) : string.Empty;

            int part2QuestionStart = part2End > part1End ? html.LastIndexOf("<p>", part2End, StringComparison.OrdinalIgnoreCase) : 0;
            int part2QuestionEnd = part2End > part1End ? html.LastIndexOf("</p>", part2End, StringComparison.OrdinalIgnoreCase) : 0;
            string part2Question = part2QuestionStart > 0 && part2QuestionStart > part2Start && part2QuestionEnd > part2Start ? html.Substring(part2QuestionStart + 3, part2QuestionEnd - part2QuestionStart - 3).Replace("<em>", string.Empty).Replace("</em>", string.Empty) : string.Empty;

            string classPath = Path.Combine(path, $"Y{year}\\Puzzle{day:00}.cs");
            if (!File.Exists(classPath)) {
                GenerateClassTemplate(year, day, dayTitle, part1Question, part2Question, classPath);
            } else if (!string.IsNullOrEmpty(part2Question)) {
                string classSource = File.ReadAllText(classPath);
                if (classSource.IndexOf($"[Description(\"What is the answer?\")]") > 0) {
                    File.WriteAllText(classPath, classSource.Replace($"[Description(\"What is the answer?\")]", $"[Description(\"{part2Question}\")]"));
                }
            }

            string secondPart = part2Start > 0 ? html.Substring(part2Start, part2End - part2Start + 10) : string.Empty;
            string descriptionHtml =
$@"<head>
<link rel=""stylesheet"" type=""text/css"" href=""../../style.css""/>
</head>
{html.Substring(part1Start, part1End - part1Start + 10)}
{secondPart}".TrimEnd();

            string descriptionPath = Path.Combine(path, $@"Y{year}\Descriptions\puzzle{day:00}.html");
            File.WriteAllText(descriptionPath, descriptionHtml);

            string[] files = Directory.GetFiles(@$"Y{year}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
            if (files.Length == 0) {
                html = DownloadHtml($"{year}/day/{day}/input");
                File.WriteAllText(Path.Combine(path, $@"Y{year}\Inputs\puzzle{day:00}--.txt"), html.TrimEnd());
            }
        }
        private static void GenerateClassTemplate(int year, int day, string dayTitle, string part1Question, string part2Question, string classPath) {
            part1Question = string.IsNullOrEmpty(part1Question) ? "What is the answer?" : part1Question;
            part2Question = string.IsNullOrEmpty(part2Question) ? "What is the answer?" : part2Question;

            string templateClass =
@$"using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y{year} {{
    [Description(""{dayTitle}"")]
    public class Puzzle{day:00} : ASolver {{
        private List<int> numbers = new();

        public override void Setup() {{
            Input.Slice('\n', line => {{
                numbers.Add(line.ToInt());
            }});
        }}

        [Description(""{part1Question}"")]
        public override string SolvePart1() {{
            int total = 0;
            for (int i = 0; i < numbers.Count; i++) {{
                int number = numbers[i];
                total += number;
            }}
            return $""{{total}}"";
        }}

        [Description(""{part2Question}"")]
        public override string SolvePart2() {{
            return string.Empty;
        }}
    }}
}}";
            File.WriteAllText(classPath, templateClass);
        }
        private static string DownloadHtml(string url) {
            Task<HttpResponseMessage> responseTask = web.GetAsync($"{BaseURL}{url}");
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result.EnsureSuccessStatusCode();
            Task<string> contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            return contentTask.Result.ToString();
        }
        public static string SubmitSolution(int year, int day, int part, string solution) {
            Task<HttpResponseMessage> responseTask = web.PostAsync($"{BaseURL}{year}/day/{day}/answer", new FormUrlEncodedContent(new Dictionary<string, string>() { { "level", $"{part}" }, { "answer", solution } }));
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;
            bool completed = response.IsSuccessStatusCode;
            if (!completed) { return "Failed to submit solution"; }
            Task<string> contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            string result = contentTask.Result.ToString();
            int index;
            if ((index = result.IndexOf("That's not the right answer", StringComparison.OrdinalIgnoreCase)) > 0) {
                index = result.IndexOf("please wait ", index, StringComparison.OrdinalIgnoreCase);
                int endIndex = result.IndexOf(" before trying ", index);
                return $"Incorrect. Please wait {result.Substring(index + 12, endIndex - index - 12)}";
            } else if ((index = result.IndexOf("You gave an answer too recently", StringComparison.OrdinalIgnoreCase)) > 0) {
                index = result.IndexOf(" before trying ", index);
                index = result.IndexOf("you have ", index, StringComparison.OrdinalIgnoreCase);
                int endIndex = result.IndexOf(" left to wait", index);
                return $"You gave an answer too recently. Wait {result.Substring(index + 9, endIndex - index - 9)}";
            } else if (result.IndexOf("That's the right answer", StringComparison.OrdinalIgnoreCase) > 0 || result.IndexOf("You've finished every puzzle", StringComparison.OrdinalIgnoreCase) > 0) {
                if (part == 1) { GeneratePuzzleTemplate(year, day); }
                return $"Correct";
            }
            return "Unknown";
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number) {
            return (int)ParseLong(number, 0, number.Length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, int startIndex) {
            return (int)ParseLong(number, startIndex, number.Length - startIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, int startIndex, int length) {
            return (int)ParseLong(number, startIndex, length);
        }
        public static long ParseLong(string number, int startIndex, int length) {
            if (length <= 0) { return 0; }

            int i = 0;
            while (i < length && !char.IsDigit(number[i + startIndex])) {
                i++;
            }

            bool isNegative = i > 0 ? number[i + startIndex - 1] == '-' : false;
            long result = 0;
            while (i < length) {
                byte value = (byte)number[i + startIndex];
                if (value >= 0x30 && value <= 0x39) {
                    result = result * 10 + value - 0x30;
                }
                i++;
            }
            return isNegative ? -result : result;
        }
    }
}