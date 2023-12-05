using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Data;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .ToImmutableArray();

var seeds = input.First().Split(":").Last().Trim().Split(" ").Select(s => long.Parse(s)).ToImmutableArray();

var maps = new List<List<((long min, long max) source, long dest)>>();
var map = new List<((long min, long max) source, long dest)>();
foreach (var l in input.Skip(1))
{
    if (l.Contains("map:"))
    {
        map = new List<((long min, long max) source, long dest)>();
        maps.Add(map);
        continue;
    }

    var ranges = l.Split(" ").Select(r => long.Parse(r)).ToArray();
    map.Add(
        (
            source: (min: ranges[1], max: ranges[1] + ranges[2] - 1),
            dest: ranges[0] - ranges[1]
        )
    );
}

var min = long.MaxValue;
foreach (var seed in seeds)
{
    var location = seed;
    foreach (var m in maps)
    {
        var transform = m.FirstOrDefault(mm => mm.source.min <= location && location <= mm.source.max);
        location += transform.dest;
    }
    if (location < min)
    {
        min = location;
    }
}
Console.WriteLine($"P1: {min}");

var seedZip = seeds.Where((s, i) => i % 2 == 0).Zip(seeds.Where((s, i) => i % 2 != 0));

// go from the 'end'
min = long.MaxValue;
maps.Reverse();

long start = 0;
int range = 1_000_000;
while (true)
{
    var locations = Enumerable.Range(0, range).AsParallel();
    var found = new ConcurrentBag<long>();
    Parallel.ForEach(locations, (locationIterator) =>
    {
        var location = start + locationIterator;
        foreach (var m in maps)
        {
            var transform = m.FirstOrDefault(mm => mm.dest + mm.source.min <= location && location <= mm.dest + mm.source.max);
            location -= transform.dest;
        }
        if (seedZip.Any(sz => sz.First <= location && location <= sz.First + sz.Second - 1))
        {
            found.Add(start + locationIterator);
        }
    });
    
    if (found.Any()) {
        min = found.Min();
        break;
    }
    start += range;
}

Console.WriteLine($"P2: {min}");
