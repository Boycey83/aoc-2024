namespace AoC;

public class Day05 : IDay
{
   private record Rule(int Before, int After);
   
   public string Part1(string input)
   {
       var (rules, sequences) = ParseInput(input);
       return sequences
           .Where(s => rules.All(r => SatisfiesRule(s, r)))
           .Select(GetMiddle)
           .Sum()
           .ToString();
   }

   public string Part2(string input)
   {
       var (rules, sequences) = ParseInput(input);
       return sequences
           .AsParallel()
           .Where(s => rules.Any(r => !SatisfiesRule(s, r)))
           .Select(s => SortSequence(s, rules))
           .Select(GetMiddle)
           .Sum()
           .ToString();
   }

   private static bool IsRuleApplicable(Rule rule, List<int> sequence) =>
       sequence.IndexOf(rule.Before) > -1 && sequence.IndexOf(rule.After) > -1;

   private static bool SatisfiesRule(List<int> sequence, Rule rule) =>
       !IsRuleApplicable(rule, sequence) || 
       sequence.IndexOf(rule.Before) < sequence.IndexOf(rule.After);

   private static int GetMiddle(List<int> sequence) => 
       sequence[sequence.Count / 2];

   private static List<int> SortSequence(List<int> sequence, IEnumerable<Rule> rules)
   {
       var applicableRules = rules.Where(r => IsRuleApplicable(r, sequence)).ToList();
       var orderedSequence = new List<int>();
       while (applicableRules.Count > 0)
       {
           var readyRules = GetNextRules(applicableRules);
           var nextSection = GetNextSection(sequence, readyRules);
           orderedSequence = orderedSequence.Concat(nextSection).ToList();
           applicableRules.RemoveAll(r => readyRules.Contains(r));
       }
       return orderedSequence.Concat(sequence.Where(n => !orderedSequence.Contains(n))).ToList();
   }

   private static IEnumerable<int> GetNextSection(IEnumerable<int> sequence, IReadOnlyCollection<Rule> nextRules) =>
       sequence.Where(n => nextRules.Select(r => r.Before).Contains(n));

   private static List<Rule> GetNextRules(IReadOnlyCollection<Rule> applicableRules) =>
       applicableRules
           .Where(r => applicableRules.All(rr => rr.After != r.Before))
           .ToList();
           
   private static (List<Rule> Rules, List<List<int>> Numbers) ParseInput(string input) =>
       (ParseRules(input), ParseSequences(input));

   private static List<Rule> ParseRules(string input) =>
       input.Split(Environment.NewLine)
           .TakeWhile(line => !string.IsNullOrEmpty(line))
           .Select(r => r.Split("|"))
           .Select(p => new Rule(int.Parse(p[0]), int.Parse(p[1])))
           .ToList();

   private static List<List<int>> ParseSequences(string input) =>
       input.Split(Environment.NewLine)
           .SkipWhile(line => !string.IsNullOrEmpty(line))
           .Skip(1)
           .Select(r => r.Split(",")
               .Select(int.Parse)
               .ToList())
           .ToList();
}