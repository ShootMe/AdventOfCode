# AdventOfCode
Solutions to the Advent of Code programming puzzles in C#

https://adventofcode.com/

### Puzzles Completed
| Year | Stars |
 ------------- | ------------- |
 2015 | 50 / 50 :star: |
 2016 | 50 / 50 :star: |
 2017 | 50 / 50 :star: |
 2018 | 50 / 50 :star: |
 2019 | 50 / 50 :star: |
 2020 | 50 / 50 :star: |
 2021 | 50 / 50 :star: |
 2022 | 50 / 50 :star: |
 2023 | 50 / 50 :star: |
 2024 | 50 / 50 :star: |
 2025 | 23 / 24 :star: |
 
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