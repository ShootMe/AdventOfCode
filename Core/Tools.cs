using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
namespace AdventOfCode.Core {
    public static class Tools {
        private const string BaseURL = $"https://adventofcode.com/";
        private static HttpClient web;
        private static DateTime nextAvailableRequestTime = DateTime.MinValue;
        static Tools() {
            web = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.All, AllowAutoRedirect = false });
            web.DefaultRequestHeaders.UserAgent.ParseAdd(".NET (github.com/ShootMe/AdventOfCode)");
            web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            web.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SESSION"])) {
                web.DefaultRequestHeaders.Add("cookie", $"session={ConfigurationManager.AppSettings["SESSION"]}");
            }
        }
        public static string GetSolutionRootPath() {
            string runningExePath = AppDomain.CurrentDomain.BaseDirectory;
            int index = runningExePath.IndexOf(@"\AdventOfCode\") + 14;
            if (index < 14) {
                index = runningExePath.IndexOf(@"\bin\") + 1;
            }
            return runningExePath.Substring(0, index);
        }
        public static bool GeneratePuzzleTemplate(int year, int day) {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SESSION"])) {
                throw new ArgumentNullException("SESSION", "Missing SESSION code in the App.config");
            }

            string path = GetSolutionRootPath();
            string descriptionPath = $@"{path}Y{year}\Descriptions\puzzle{day:00}.html";
            string html = File.Exists(descriptionPath) ? File.ReadAllText(descriptionPath) : DownloadHtml($"{year}/day/{day}");
            PuzzleDescription description = new PuzzleDescription(html);

            if (description.Part1 == null) {
                Console.WriteLine($"Failed to download Puzzle data {year}-{day}", ConsoleColor.Red);
                nextAvailableRequestTime = DateTime.Now.AddSeconds(1.5);
                return false;
            } else if (File.Exists(descriptionPath) && description.Part2 == null) {
                html = DownloadHtml($"{year}/day/{day}");
                description = new PuzzleDescription(html);
            }

            Directory.CreateDirectory($"{path}Y{year}");
            Directory.CreateDirectory(@$"{path}Y{year}\Inputs");
            Directory.CreateDirectory(@$"{path}Y{year}\Descriptions");

            string classPath = @$"{path}Y{year}\Puzzle{day:00}.cs";
            if (!File.Exists(classPath)) {
                GenerateClassTemplate(year, day, description.Title, description.Part1?.Question, description.Part2?.Question, classPath);
            } else if (!string.IsNullOrEmpty(description.Part2?.Question)) {
                string classSource = File.ReadAllText(classPath);
                if (classSource.IndexOf($"[Description(\"What is the answer?\")]") > 0) {
                    File.WriteAllText(classPath, classSource.Replace($"[Description(\"What is the answer?\")]", $"[Description(\"{description.Part2.Question}\")]"));
                }
            }

            string descriptionHtml =
$@"<head>
<link rel=""stylesheet"" type=""text/css"" href=""../../style.css""/>
</head>
{description.Part1.Description}
{description.Part2?.Description}".TrimEnd();

            File.WriteAllText(descriptionPath, descriptionHtml);

            string[] files = Directory.GetFiles(@$"{path}Y{year}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
            int givenCount = 0;
            for (int i = 0; i < files.Length; i++) {
                if (!files[i].EndsWith("-Example.txt", StringComparison.OrdinalIgnoreCase)) {
                    givenCount++;
                }
            }
            if (givenCount == 0) {
                html = DownloadHtml($"{year}/day/{day}/input");
                File.WriteAllText($@"{path}Y{year}\Inputs\puzzle{day:00}--.txt", html.TrimEnd());
            }
            nextAvailableRequestTime = DateTime.Now.AddSeconds(1.1);

            return true;
        }
        private static void GenerateClassTemplate(int year, int day, string dayTitle, string part1Question, string part2Question, string classPath) {
            Console.WriteLine($"Generating class template for {year}-{day}");

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
            foreach (string line in Input.Split('\n')) {{
                numbers.Add(line.ToInt());
            }}
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
            Console.WriteLine($"Downloading {url} ...");
            if (DateTime.Now < nextAvailableRequestTime) {
                Thread.Sleep((int)(nextAvailableRequestTime - DateTime.Now).TotalMilliseconds);
            }
            Task<HttpResponseMessage> responseTask = web.GetAsync($"{BaseURL}{url}");
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result.EnsureSuccessStatusCode();
            Task<string> contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            return contentTask.Result.ToString();
        }
        public static string SubmitSolution(int year, int day, int part, string solution) {
            if (DateTime.Now < nextAvailableRequestTime) {
                Thread.Sleep((int)(nextAvailableRequestTime - DateTime.Now).TotalMilliseconds);
            }
            Task<HttpResponseMessage> responseTask = web.PostAsync($"{BaseURL}{year}/day/{day}/answer", new FormUrlEncodedContent(new Dictionary<string, string>() { { "level", $"{part}" }, { "answer", solution } }));
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;
            bool completed = response.IsSuccessStatusCode;
            if (!completed) { return "Failed to submit solution"; }
            Task<string> contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            nextAvailableRequestTime = DateTime.Now.AddSeconds(1.1);
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
                if (part == 1) { DownloadProblem(year, day, true); }
                return $"Correct";
            }
            return "Unknown";
        }
        public static void DownloadProblem(int year, int dayToRun, bool showDescriptionInBrowser) {
            DateTime target = DateTime.Parse($"{year}-12-{dayToRun:00}T00:00:00-05:00");
            TimeSpan nowTillTarget = target - DateTime.Now;
            if (nowTillTarget.TotalMinutes > 0 && nowTillTarget.TotalMinutes < 30) {
                Console.WriteLine("Waiting till puzzle releases at midnight EST...");
                (int left, int top) = Console.GetCursorPosition();
                Console.CursorVisible = false;
                while (DateTime.Now < target) {
                    Console.WriteLine($"{target - DateTime.Now}");
                    Console.SetCursorPosition(left, top);
                    Thread.Sleep(200);
                }
                Console.CursorVisible = true;
                Console.SetCursorPosition(left, top + 1);
            } else if (nowTillTarget.TotalMinutes > 30) {
                Console.WriteLine($"Puzzle release date is farther than 30 minutes in the future ({nowTillTarget.TotalMinutes:0} minutes). Please try again later.");
                return;
            }

            if (GeneratePuzzleTemplate(year, dayToRun) && showDescriptionInBrowser) {
                string htmlPath = @$"{GetSolutionRootPath()}Y{year}\Descriptions\puzzle{dayToRun:00}.html";
                Process.Start(new ProcessStartInfo($"{htmlPath}") { UseShellExecute = true });
            }
        }
    }
    internal class PuzzleDescription {
        public string Title;
        public PuzzlePart Part1;
        public PuzzlePart Part2;

        public PuzzleDescription(string html) {
            int part1Start = html.IndexOf("<article", StringComparison.OrdinalIgnoreCase);
            int part1End = html.IndexOf("</article>", StringComparison.OrdinalIgnoreCase);
            if (part1Start < 0 || part1End < 0) {
                return;
            }
            Part1 = new PuzzlePart(html.Substring(part1Start, part1End - part1Start + 10));

            int part2Start = html.IndexOf("<article", part1Start + 1, StringComparison.OrdinalIgnoreCase);
            int part2End = html.IndexOf("</article>", part1End + 1, StringComparison.OrdinalIgnoreCase);
            if (part2Start < part2End && part2Start >= 0) {
                Part2 = new PuzzlePart(html.Substring(part2Start, part2End - part2Start + 10));
            }

            int dayHeaderStart = html.IndexOf("<h2>--- Day ");
            dayHeaderStart = html.IndexOf(':', dayHeaderStart) + 2;
            int dayHeaderEnd = html.IndexOf(" ---</h2>", dayHeaderStart);
            Title = html.Substring(dayHeaderStart, dayHeaderEnd - dayHeaderStart);
        }
    }
    internal class PuzzlePart {
        public string Description;
        public string Question;

        public PuzzlePart(string html) {
            int start = html.LastIndexOf("<p>", StringComparison.OrdinalIgnoreCase);
            int end = html.LastIndexOf("</p>", StringComparison.OrdinalIgnoreCase);
            Description = html;
            Question = start < end && start >= 0 ? html.Substring(start + 3, end - start - 3).Replace("<em>", string.Empty).Replace("</em>", string.Empty).Replace("<code>", string.Empty).Replace("</code>", string.Empty) : string.Empty;
        }
    }
}