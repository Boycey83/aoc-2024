using System.Text.RegularExpressions;

namespace AoC;

public partial class Day03 : IDay
{
    private const string Do = "do()";
    private const string Dont = "don't()";
    private const StringComparison Compare = StringComparison.InvariantCulture;
    
    [GeneratedRegex(@"mul\((?<First>\d+),(?<Second>\d+)\)")]
    private static partial Regex MulGeneratedRegex();
    private static readonly Regex Parser = MulGeneratedRegex();

    public string Part1(string input) => 
        ExecuteMulInstructions(ParseInput(input)).ToString();

    public string Part2(string input) =>
        GetEnabledInstructions(ParseInput(input))
            .Sum(ExecuteMulInstructions)
            .ToString();

    private static int ExecuteMulInstructions(string instructions) =>
        Parser.Matches(instructions)
            .Select(ParseMatch)
            .Sum(t => t.First * t.Second);

    private static (int First, int Second) ParseMatch(Match match) => 
        (int.Parse(match.Groups["First"].Value), int.Parse(match.Groups["Second"].Value));
    
    private static IEnumerable<string> GetEnabledInstructions(string instructions)
    {
        var remaining = PrepareInstruction(instructions);
        while (remaining.Length > 0)
        {            
            var doIndex = remaining.IndexOf(Do, Compare);
            if (doIndex == -1)
            {
                yield break;
            }
            
            var afterDo = doIndex + Do.Length;
            var dontIndex = remaining.IndexOf(Dont, afterDo, Compare);
            yield return remaining[afterDo..dontIndex];
            
            var afterDont = dontIndex + Dont.Length;
            remaining = remaining[afterDont..];
        }
    }

    private static string PrepareInstruction(string instructions) => 
        $"{Do}{instructions}{Dont}";

    private static string ParseInput(string input) =>
        string.Join("", input.Split(Environment.NewLine));
}