namespace AoC;

public class Day07 : IDay
{
    private class Equation(long result, IEnumerable<long> numbers)
    {
        public long Result { get; } = result;
        public List<long> Numbers { get; } = numbers.ToList();
    }
    
    private static long Add(long a, long b) => a + b;
    private static long Mul(long a, long b) => a * b;
    private static long Concat(long a, long b) => long.Parse($"{a}{b}");

    private static readonly List<Func<long, long, long>> Part1Ops = [ Add, Mul ];
    private static readonly List<Func<long, long, long>> Part2Ops = [ Add, Mul, Concat ];

    public string Part1(string input) => 
        ParseInput(input)
            .AsParallel()
            .Where(eq => ValidateEquation(Part1Ops, eq))
            .Sum(eq => eq.Result)
            .ToString();

    public string Part2(string input) => 
        ParseInput(input)
            .AsParallel()
            .Where(eq => ValidateEquation(Part2Ops, eq))
            .Sum(eq => eq.Result)
            .ToString();

    private static bool ValidateEquation(ICollection<Func<long, long, long>> operationFns, Equation equation) =>
        equation.Numbers.Count == 1 
            ? equation.Numbers[0] == equation.Result
            : operationFns.Any(op => ValidateEquation(operationFns, GetNextEquation(equation, op)));

    private static Equation GetNextEquation(Equation current, Func<long, long, long> operationFn) => 
        new(
            current.Result, 
            new[] { operationFn(current.Numbers[0], current.Numbers[1]) }.Concat(current.Numbers.Skip(2))
        );

    private static IEnumerable<Equation> ParseInput(string input) =>
        from line in input.Split(Environment.NewLine)
        let parts = line.Split(": ")
        let result = long.Parse(parts[0])
        let numbers = parts[1].Split(" ").Select(long.Parse)
        select new Equation(result, numbers);
}