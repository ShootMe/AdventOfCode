# AdventOfCode
Solutions to the Advent of Code programming puzzles in C# .NET8

https://adventofcode.com/

### Puzzles Completed
| Year | 2015 | 2016 | 2017 | 2018 | 2019 | 2020 | 2021 | 2022 | 2023 |
 ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
 1 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 2 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 3 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 4 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 5 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 6 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 7 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 8 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 9 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 10 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 11 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 12 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 13 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 14 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 15 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 16 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: |
 17 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 18 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 19 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 20 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 21 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 22 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 23 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 24 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 25 | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | :star: :star: | |
 
### Template Files
To auto generate a template for a day, you will need to have the SESSION variable filled with the correct value, which can be grabbed by looking at the cookies for the advent of code website after being signed in.

You will then need to add an App.config file to the solution as such:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
		<add key="SESSION" value="Your session cookie" />
	</appSettings>
</configuration>
```

You can then run Runner.AdventYear(year, day)

This will download the puzzle description as puzzleXX.html and parse out just the problem portion.
If you have not beat part 1 yet, you will need to run the Tools.GeneratePuzzleTemplate method manually,
or add the [Submit] attribute on the SolvePart1() method to submit your answer and it will then redownload the puzzle description for part 2.

The puzzle input is downloaded as well as puzzleXX\~\~.txt

It then generates a PuzzleXX.cs class file for that year.

### Input Files
Are in the format puzzleXX\~Part1Solution\~Part2Solution.txt

When ran, the output is checked against those values and if it matches it will output those as blue text, otherwise it will be red.

You can add multiple inputs by adding an extra description to the end.

ie) puzzle01\~123\~456.txt

ie) puzzle01\~654\~321\~Test.txt

ie) puzzle01\~ABC\~DEF\~Extra.txt