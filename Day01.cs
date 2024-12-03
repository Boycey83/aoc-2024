namespace AoC;

public class Day01 : IDay
{
    public string Part1(string input)
    {
        var (list1, list2) = ParseLists(input);
        return list1.OrderBy(x => x)
            .Zip(list2.OrderBy(x => x))
            .Sum(pair => Math.Abs(pair.First - pair.Second))
            .ToString();
    }

    public string Part2(string input)
    {
        var (list1, list2) = ParseLists(input);
        var frequencyMap = CreateFrequencyMap(list2);

        return list1
            .Sum(num => num * frequencyMap.GetValueOrDefault(num, 0))
            .ToString();
    }
    
    private static Dictionary<int, int> CreateFrequencyMap(IEnumerable<int> list) =>
        list.GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());

    private static (List<int>, List<int>) ParseLists(string input) =>
        input.Split(Environment.NewLine)
            .Select(line => {
                var parts = line.Split("  ");
                return (int.Parse(parts[0]), int.Parse(parts[1]));
            })
            .Aggregate((new List<int>(), new List<int>()), (acc, pair) => {
                acc.Item1.Add(pair.Item1);
                acc.Item2.Add(pair.Item2);
                return acc;
            });
}
