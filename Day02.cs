namespace AoC;

public class Day02 : IDay
{
    public string Part1(string input) =>
        ParseInput(input)
            .Count(IsValidSequence)
            .ToString();

    public string Part2(string input) => 
        ParseInput(input)
            .Count(IsValidWithRemoval)
            .ToString();

    private static bool IsValidSequence(List<int> sequence)
    {
        var differences = sequence
            .Zip(sequence.Skip(1))
            .Select(pair => pair.Second - pair.First)
            .ToList();
        return IsAllInValidRange(differences) && IsSequenceInOneDirection(differences);
    }

    private static bool IsSequenceInOneDirection(IList<int> differences) => 
        differences.All(d => d > 0) || differences.All(d => d < 0);

    private static bool IsAllInValidRange(IList<int> differences) => 
        differences.All(d => Math.Abs(d) is >= 1 and <= 3);

    private static bool IsValidWithRemoval(List<int> sequence) =>
        IsValidSequence(sequence) || 
        Enumerable.Range(0, sequence.Count)
            .Any(i => IsValidSequence(SequenceWithoutOneNumber(sequence, i)));

    private static List<int> SequenceWithoutOneNumber(List<int> sequence, int i) => 
        sequence
            .Where((_, index) => index != i)
            .ToList();

    private static List<List<int>> ParseInput(string input) =>
        input.Split(Environment.NewLine)
            .Select(line => line.Split(" ").Select(int.Parse).ToList())
            .ToList();
}