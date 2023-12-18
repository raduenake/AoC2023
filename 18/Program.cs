using System.Collections.Immutable;
using System.Numerics;
using System.Text.RegularExpressions;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .Select(l =>
    {
        var rx = new Regex(@"^(\D*)\s(\d*)\s\((.*)\)$");
        var match = rx.Match(l);
        var dir = match.Groups[1].Value switch
        {
            "U" => new Vector2(-1, 0),
            "D" => new Vector2(1, 0),
            "L" => new Vector2(0, -1),
            "R" => new Vector2(0, 1),
            _ => new Vector2(0, 0)
        };
        var amount = int.Parse(match.Groups[2].Value);
        var color = match.Groups[3].Value;
        return (dir, amount, color);
    })
    .ToImmutableArray();

var polygon = new List<Vector2>() { new Vector2(0, 0) };
var lastVertex = polygon.Last();
foreach (var instr in input)
{
    int i = instr.amount;
    var temp = new Vector2(lastVertex.X, lastVertex.Y);

    temp += instr.dir * instr.amount;
    polygon.Add(temp);

    lastVertex = polygon.Last();
}

var polygonSegments = polygon.Take(polygon.Count() - 1).Zip(polygon.Skip(1));
// shoelace https://en.wikipedia.org/wiki/Shoelace_formula
var polyArea = 0.5 * Math.Abs(polygonSegments.Aggregate(0L, (acc, it) => acc += ((long)it.First.Y + (long)it.Second.Y) * ((long)it.First.X - (long)it.Second.X)));
// perimeter length 
var polyPerimeterLength = polygonSegments.Aggregate(0L, (acc, it) => acc += (long)Math.Sqrt(Math.Pow(it.First.X - it.Second.X, 2) + Math.Pow(it.First.Y - it.Second.Y, 2)));

// area + 1/2 perimeter + 1
var p1 = polyArea + (polyPerimeterLength / 2) + 1;
Console.WriteLine($"P1: {p1}");

polygon = new List<Vector2>() { new Vector2(0, 0) };
lastVertex = polygon.Last();
foreach (var instr in input)
{
    var dist = instr.color.Skip(1);
    var amount = Convert.ToInt32(string.Join("", dist.Take(dist.Count() - 1)), 16);
    var dir = dist.Last() switch
    {
        '0' => new Vector2(0, 1),
        '1' => new Vector2(1, 0),
        '2' => new Vector2(0, -1),
        '3' => new Vector2(-1, 0),
        _ => new Vector2(0, 0)
    };

    var temp = new Vector2(lastVertex.X, lastVertex.Y);

    temp += dir * amount;
    polygon.Add(temp);

    lastVertex = polygon.Last();
}

polygonSegments = polygon.Take(polygon.Count() - 1).Zip(polygon.Skip(1));
// shoelace https://en.wikipedia.org/wiki/Shoelace_formula
polyArea = 0.5 * Math.Abs(polygonSegments.Aggregate(0L, (acc, it) => acc += ((long)it.First.Y + (long)it.Second.Y) * ((long)it.First.X - (long)it.Second.X)));
// perimeter length 
polyPerimeterLength = polygonSegments.Aggregate(0L, (acc, it) => acc += (long)Math.Sqrt(Math.Pow(it.First.X - it.Second.X, 2) + Math.Pow(it.First.Y - it.Second.Y, 2)));

// area + 1/2 perimeter + 1
var p2 = polyArea + (polyPerimeterLength / 2) + 1;
Console.WriteLine($"P2: {p2}");
