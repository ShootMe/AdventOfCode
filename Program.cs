using AdventOfCode.Core;
namespace AdventOfCode {
    public class Program {
        public static void Main(string[] args) {
            //Synacor Challenge
            //Synacor.Emulator.RunChallenge();

            //Run code for all years and days
            //Runner.AdventYear();

            //Run code for a specific year
            //Runner.AdventYear(2022);

            //Run code for a specific day in the year
            //Class is auto generated if it doesn't exist and
            //will auto download description and input if current date > specified and they don't exist
            //or within 30 minutes of the problem start (and will wait till midnight EST)
            //Problem description will be brought up in the default browser if it was pulled
            Runner.AdventYear(2023, 1);
        }
    }
}