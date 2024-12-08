using System.Collections.Immutable;

namespace AoC;

public record GridPosition(int Row, int Col);
public record GridCell(GridPosition Position, char Value);

public class Day04 : IDay
{
    // Part 1 configuration
    private static readonly char[] PartOneTarget = "XMAS".ToCharArray();
    private static readonly char[] PartOneReverseTarget = PartOneTarget.Reverse().ToArray();
    private static readonly int PartOneLength = PartOneTarget.Length;

    // Part 2 configuration 
    private static readonly char[] PartTwoTarget = "MAS".ToCharArray();
    private static readonly char[] PartTwoReverseTarget = PartTwoTarget.Reverse().ToArray();
    private static readonly int PartTwoLength = PartTwoTarget.Length;

    public string Part1(string input)
    {
        var cells = ParseInput(input);
        
        return new[] { "Horizontal", "Vertical", "TLBR", "TRBL" }
            .Select(direction => 
                GetRotatedSequences(cells, GetRotationGroupingFn(direction), PartOneLength))
            .Sum(sequences => CountMatches(sequences, PartOneTarget, PartOneReverseTarget))
            .ToString();
    }

    public string Part2(string input)
    {
        var cells = ParseInput(input);
        
        var middlePositionsOfMatches = new[] { "TLBR", "TRBL" }
            .Select(direction => 
                FindMiddlePositionOfMatches(
                    GetRotatedSequences(cells, GetRotationGroupingFn(direction), PartTwoLength),
                    PartTwoTarget,
                    PartTwoReverseTarget))
            .ToList();

        return middlePositionsOfMatches[0]
            .Intersect(middlePositionsOfMatches[1])
            .Count()
            .ToString();
    }
    
    private static Func<GridPosition, int> GetRotationGroupingFn(string direction) => direction switch
    {
        "Horizontal" => p => p.Row,
        "Vertical" => p => p.Col,
        "TLBR" => p => p.Row + p.Col,
        "TRBL" => p => p.Row - p.Col,
        _ => throw new ArgumentException($"Unknown direction: {direction}")
    };

    private static IEnumerable<IEnumerable<GridCell>> GetRotatedSequences(
        ImmutableList<GridCell> cells, 
        Func<GridPosition, int> groupingStrategy,
        int minLength)
    {
        return cells
            .GroupBy(cell => groupingStrategy(cell.Position))
            .Where(g => HasMinimumLength(g, minLength));
    }

    private static int CountMatches(IEnumerable<IEnumerable<GridCell>> sequences, 
        char[] target, 
        char[] reverseTarget)
    {
        var matchingWindows = 
            from sequence in sequences
            from startIndex in Enumerable.Range(0, GetPossibleStartPositions(sequence, target.Length))
            let window = TakeWindow(sequence, startIndex, target.Length).Select(c => c.Value).ToList()
            where IsMatch(window, target, reverseTarget)
            select window;

        return matchingWindows.Count();
    }

    private static IEnumerable<GridPosition> FindMiddlePositionOfMatches(
        IEnumerable<IEnumerable<GridCell>> sequences,
        char[] target,
        char[] reverseTarget)
    {
        var middleIndex = target.Length / 2;
        return from sequence in sequences
            from startIndex in Enumerable.Range(0, GetPossibleStartPositions(sequence, target.Length))
            let cells = TakeWindow(sequence, startIndex, target.Length).ToList()
            let values = cells.Select(c => c.Value).ToList()
            where IsMatch(values, target, reverseTarget)
            select cells[middleIndex].Position;
    }

    private static IEnumerable<GridCell> TakeWindow(IEnumerable<GridCell> cells, int startIndex, int length) =>
        cells.Skip(startIndex).Take(length);

    private static bool HasMinimumLength(IEnumerable<GridCell> sequence, int length) =>
        sequence.Count() >= length;

    private static int GetPossibleStartPositions(IEnumerable<GridCell> sequence, int patternLength) =>
        sequence.Count() - patternLength + 1;

    private static bool IsMatch(IList<char> window, IEnumerable<char> target, IEnumerable<char> reverseTarget) =>
        window.SequenceEqual(target) || window.SequenceEqual(reverseTarget);
    
    private static ImmutableList<GridCell> ParseInput(string input)
    {
        var lines = input.Split(Environment.NewLine);
        return (
            from row in lines.Select((line, rowIndex) => (line, rowIndex))
            from col in Enumerable.Range(0, row.line.Length)
            select new GridCell(new GridPosition(row.rowIndex, col), row.line[col])
        ).ToImmutableList();
    }
}