using System.Collections.Immutable;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .ToImmutableArray();

var times = input.First().Split(":").Last().Split(" ").Where(s => s != "").Select(int.Parse);
var records = input.Last().Split(":").Last().Split(" ").Where(s => s != "").Select(int.Parse);

var timeAndRecord = times.Zip(records);
var zz = timeAndRecord.Select(tr =>
{
    var possibilities = Enumerable.Range(0, tr.First).Select(speed => (s: speed, dist: speed * (tr.First - speed)));
    var wins = possibilities.Where(pos => pos.dist > tr.Second);
    return wins;
});
Console.WriteLine($"P1: {zz.Aggregate(1, (acc, curr) => acc * curr.Count())}");

var time = long.Parse(input.First().Split(":").Last().Split(" ").Where(s => s != "").Aggregate("", (acc, s) => acc + s));
var distance = long.Parse(input.Last().Split(":").Last().Split(" ").Where(s => s != "").Aggregate("", (acc, s) => acc + s));

// a * x^2 + b * x + c = 0
Func<long, long, long, IEnumerable<double>> quadraticFormulaSolution = (a, b, c) =>
{
    var discriminant = Math.Pow(b, 2) - 4 * a * c;
    if (discriminant < 0)
    {
        // we discard "complex" roots
        return Enumerable.Empty<double>();
    }
    else
    {
        return (new[] { -1.0, 1.0 }).Select(s => (s * Math.Sqrt(discriminant) - b) / (2.0 * a));
    }
};

var quadraticSolution = quadraticFormulaSolution(1, -time, distance);
Console.WriteLine($"P2: {Math.Floor(quadraticSolution.Max()) - Math.Ceiling(quadraticSolution.Min()) + 1}");
