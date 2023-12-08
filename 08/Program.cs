using System.Collections.Immutable;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .ToImmutableArray();

var instructions = input.First().Select(inst => inst switch { 'L' => 0, 'R' => 1, _ => -1 }).ToList();
var map = input.Skip(1).Select(l =>
{
    var locAndDirs = l.Split("=");
    return (
        loc: locAndDirs.First().Trim(),
        lr: locAndDirs.Last().Replace("(", "").Replace(")", "").Split(",").Select(llrr => llrr.Trim()).ToList()
    );
}).ToDictionary(kSel => kSel.loc, vSel => vSel.lr);

Func<string, Func<string, bool>, List<int>, Dictionary<string, List<string>>, ulong> solve = (start, end, instr, map) =>
{
    var stepCounter = 0UL;

    var here = start;
    var instrEnum = instr.GetEnumerator();
    while (!end(here))
    {
        if (!instrEnum.MoveNext())
        {
            instrEnum = instr.GetEnumerator();
            instrEnum.MoveNext();
        }

        stepCounter++;
        here = map[here][instrEnum.Current];
    }
    return stepCounter;
};

Console.WriteLine($"P1: {solve("AAA", (node) => node == "ZZZ", instructions, map)}");

var starts = map.Keys.Where(k => k.EndsWith("A")).ToImmutableArray();
var results = starts.AsParallel().Select(ss => solve(ss, (node) => node.EndsWith("Z"), instructions, map)).Select(r => (long)r).ToList();
var sol = MathNet.Numerics.Euclid.LeastCommonMultiple(results);

Console.WriteLine($"P2: {sol}");
