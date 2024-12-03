using AoC;

const int defaultDay = 3;
var day = args.Length > 0 && int.TryParse(args[0], out var d) ? d : defaultDay;
var sw = System.Diagnostics.Stopwatch.StartNew();

var input = await GetInputForDayAsync(day);
var parseTime = sw.Elapsed;

var solver = GetSolver(day);
var part1Result = solver.Part1(input);
var part1Time = sw.Elapsed - parseTime;

var part2Result = solver.Part2(input);
var part2Time = sw.Elapsed - parseTime - part1Time;

var totalTime = sw.Elapsed;
sw.Stop();

Console.WriteLine($"""
                   Day {day}
                   Part 1: {part1Result} ({part1Time.TotalMilliseconds:F2}ms)
                   Part 2: {part2Result} ({part2Time.TotalMilliseconds:F2}ms)
                   Parse: {parseTime.TotalMilliseconds:F2}ms
                   Total: {totalTime.TotalMilliseconds:F2}ms
                   """);

return;

static IDay GetSolver(int day) => day switch
{
    1 => new Day01(),
    _ => throw new ArgumentException($"Day {day} not implemented")
};

Task<string> GetInputForDayAsync(int i) => 
    File.ReadAllTextAsync($@"C:\code\aoc-2024\inputs\{i:D2}.txt");