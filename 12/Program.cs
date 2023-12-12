using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .Select(l =>
    {
        var splits = l.Split(" ");
        var pattern = splits.First().Trim();
        var counts = splits.Last().Split(",").Select(n => int.Parse(n.Trim())).ToList();
        return (pattern, counts);
    })
    .ToImmutableArray();

var history = new Dictionary<string, ulong>();
Func<string, List<int>, ulong> cnt = (pattern, patternCounts) => { return 0UL; };
cnt = (pattern, patternCounts) =>
{
    // cache
    var key = pattern + string.Join(",",patternCounts);
    if (history.ContainsKey(key))
    {
        return history[key];
    }

    if (patternCounts.Count() == 0) {
        return pattern.All(c => c != '#') ? 1UL : 0UL;
    }

    var size = patternCounts.First();
    var total = 0UL;

    for (int i = 0; i < pattern.Length; i++)
    {
        if (i + size <= pattern.Length &&
            pattern.Skip(i).Take(size).All(c => c != '.') &&
            (i == 0 || pattern[i - 1] != '#') &&
            (i + size == pattern.Count() || pattern[i + size] != '#')
        ) {
            total += cnt(string.Join("", pattern.Skip(i + size + 1)), patternCounts.Skip(1).ToList());
        }

        if (pattern[i] == '#')
        {
            break;
        }
    }

    history.Add(key, total);
    return total;
};

var arrangements = 0UL;
foreach (var inputLine in input)
{
    arrangements += cnt(inputLine.pattern, inputLine.counts);
}
Console.WriteLine($"P1: {arrangements}");

arrangements = 0UL;
foreach (var inputLine in input)
{
    var range = Enumerable.Range(0, 5);
    arrangements += cnt(string.Join('?', range.Select(_ => inputLine.pattern)), range.SelectMany(_ => inputLine.counts).ToList());
}

Console.WriteLine($"P2: {arrangements}");
