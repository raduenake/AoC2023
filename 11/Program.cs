using System.Collections.Immutable;
using System.Numerics;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .SelectMany((line, i) =>
    {
        var temp = line.Select((c, j) => (pos: new Vector2(i, j), ch: c));
        return temp.Where(posAndChar => posAndChar.ch != '.').Select(posAndChar => posAndChar.pos);
    })
    .ToImmutableArray();

Func<Vector2, uint, Vector2> expandPosition = (coord, ratio) =>
{
    var newX = Enumerable.Range(0, (int)coord.X).Count(x => !input.Any(p => x == (int)p.X));
    var newY = Enumerable.Range(0, (int)coord.Y).Count(y => !input.Any(p => y == (int)p.Y));
    return new Vector2(coord.X + newX * (ratio - 1), coord.Y + newY * (ratio - 1));
};

Func<Vector2, Vector2, ulong> manhattanDist = (a, b) =>
{
    return (ulong)(Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y));
};

var expansion1 = input.Select(pp => expandPosition(pp, 2)).ToImmutableArray();
var dist = expansion1.SelectMany((p1, i) => expansion1.Skip(i + 1).Select(p2 => (A: p1, B: p2)))
    .Select(aAndB => manhattanDist(aAndB.A, aAndB.B));
Console.WriteLine($"P1: {dist.Aggregate(0UL, (acc, theDistance) => acc += theDistance)}");

var expansion2 = input.Select(pp => expandPosition(pp, 1_000_000)).ToImmutableArray();
var dist2 = expansion2.SelectMany((p1, i) => expansion2.Skip(i + 1).Select(p2 => (A: p1, B: p2)))
    .Select(aAndB => manhattanDist(aAndB.A, aAndB.B));
Console.WriteLine($"P2: {dist2.Aggregate(0UL, (acc, theDistance) => acc += theDistance)}");
