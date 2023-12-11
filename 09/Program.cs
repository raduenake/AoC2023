using System.Collections.Immutable;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .Select(l => l.Split(" ").Select(int.Parse).ToImmutableArray())
    .ToImmutableArray();

Func<List<int>, List<int>> mapReduce = data =>
{
    return data.Skip(1).Zip(data.Take(data.Count() - 1)).Select(pp => pp.First - pp.Second).ToList();
};

var mapReduceHistory = input.Select(line =>
{
    var history = new List<List<int>>
    {
        line.ToList()
    };
    var pairs = mapReduce(line.ToList());
    while (!pairs.All(p => p == 0))
    {
        history.Add(pairs);
        pairs = mapReduce(pairs);
    }
    
    history.Reverse();
    return history;
});

var p1 = mapReduceHistory.Select(alg => alg.Aggregate(0L, (acc, stuff) => acc += stuff.Last())).Sum();
Console.WriteLine($"P1: {p1}");

var p2 = mapReduceHistory.Select(alg => alg.Aggregate(0L, (acc, stuff) => acc = stuff.First() - acc)).Sum();
Console.WriteLine($"P2: {p2}");
