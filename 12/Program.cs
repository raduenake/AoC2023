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

Func<string, string, bool> isMatch = (s, pattern) =>
{
    var r = false;
    if (s.Length != pattern.Length)
        return r;

    for (int i = 0; i < s.Length; i++)
    {
        r = pattern[i] switch
        {
            '#' => s[i] == '1',
            '.' => s[i] == '0',
            _ => true
        };
        if (!r)
            break;
    }
    return r;
};

Func<string, List<int>, bool> isBinOneMatch = (s, digitCounts) =>
{
    var r = false;
    var oneGroups = s.Split('0').Where(s => !string.IsNullOrEmpty(s)).ToList();
    if (oneGroups.Count() == digitCounts.Count())
    {
        for (int i = 0; i < oneGroups.Count(); i++)
        {
            r = oneGroups[i].Count() == digitCounts[i];
            if (!r)
                break;
        }
    }
    return r;
};

Func<IEnumerable<char>, int, IEnumerable<IEnumerable<char>>> perm = (list, length) => { return Enumerable.Empty<IEnumerable<char>>(); };
perm = (list, length) =>
{
    if (length == 1)
    {
        var l1 = list.SelectMany(c => c switch { '?' => new[] { new[] { '#' }, new[] { '.' } }, _ => new[] { new[] { c } } });
        return l1;
    }
    var abc = perm(list, length - 1);
    return perm(list, length - 1);
};

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
