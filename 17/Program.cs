using System.Collections.Immutable;
using System.Net.Security;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .SelectMany((l, i) =>
        l.Select((c, j) => (pos: new Vector2(i, j), val: int.Parse("" + c)))
    ).ToImmutableDictionary(kv => kv.pos, kv => kv.val);

var degToRad = (float deg) => (float)(Math.PI / 180.0) * deg;
var rotateDeg = (Vector2 vec, float deg) =>
{
    var tmp = Vector2.Transform(vec, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, degToRad(deg)));
    return new Vector2((int)Math.Round(tmp.X), (int)Math.Round(tmp.Y));
};

var x = rotateDeg(Vector2.UnitX, -90);

var maxX = input.Keys.Max(k => k.X);
var maxY = input.Keys.Max(k => k.Y);

var moveDict = new[] { (Vector2.UnitX, "D"), ((-1) * Vector2.UnitX, "U"), (Vector2.UnitY, "R"), ((-1) * Vector2.UnitY, "L") }.ToDictionary(kv => kv.Item1, kv => kv.Item2);
var hash = (Vector2 last, Vector2 prev, string hist) =>
{
    return last.GetHashCode() + prev.GetHashCode() + hist.GetHashCode();
};

var shortDist = (int maxSteps, int stepsInLine) => { return 0; };
shortDist = (int maxSteps, int stepsInLine) =>
{
    var prioQueue = new PriorityQueue<(Vector2, Vector2, string), int>();
    var visited = new HashSet<int>();

    //start is "Free";
    prioQueue.Enqueue((Vector2.Zero, Vector2.Zero, ""), (-1) * input[Vector2.Zero]);
    (Vector2 last, Vector2 prev, string moveHistory) current;
    var loss = 0;
    while (prioQueue.TryDequeue(out current, out loss))
    {
        if (current.last.X == maxX && current.last.Y == maxY)
        {
            var lastSteps = current.moveHistory.TakeLast(stepsInLine);
            if (lastSteps.Count() == stepsInLine && lastSteps.Take(stepsInLine - 1).All(s => s == lastSteps.Last()))
                return loss + input[current.last];
        }

        var dirOfTravel = current.last != current.prev ? current.last - current.prev : Vector2.Zero;
        var dirUnits = new[] { (increment: Vector2.Zero, move: "_") };
        if (dirOfTravel == Vector2.Zero)
        {
            dirUnits = [
                (increment: Vector2.UnitX, move: moveDict[Vector2.UnitX]),
                (increment: Vector2.UnitY, move: moveDict[Vector2.UnitY])
            ];
        }
        else
        {
            dirUnits = [
                (increment: rotateDeg(dirOfTravel, -90), move: moveDict[rotateDeg(dirOfTravel, -90)]),
                (increment: dirOfTravel, move: moveDict[dirOfTravel]),
                (increment: rotateDeg(dirOfTravel, 90), move: moveDict[rotateDeg(dirOfTravel, 90)])
            ];
        }

        foreach (var dirUnit in dirUnits)
        {
            var next = current.last + dirUnit.increment;
            if (!input.ContainsKey(next))
                continue;

            var moveHistory = current.moveHistory + dirUnit.move;
            var tmpMoveHistory = moveHistory.TakeLast(maxSteps + 1);
            if (
                (tmpMoveHistory.Count() == maxSteps + 1) || 
                (tmpMoveHistory.Count() < stepsInLine && tmpMoveHistory.TakeLast(stepsInLine).All(s => s == tmpMoveHistory.TakeLast(stepsInLine).Last())) ||
                (tmpMoveHistory.Count() > 1 && 
                tmpMoveHistory.Last() != tmpMoveHistory.Take(tmpMoveHistory.Count() - 1).Last() &&
                tmpMoveHistory.Take(tmpMoveHistory.Count() - 1).TakeLast(stepsInLine).Count() == stepsInLine &&
                tmpMoveHistory.Take(tmpMoveHistory.Count() - 1).TakeLast(stepsInLine)
                    .Take(stepsInLine - 1).Any(h => h != tmpMoveHistory.Take(tmpMoveHistory.Count() - 1).Last()))
            )
            
            {
                continue;
            }

            var nextLoss = loss + input[current.last];
            var h = hash(next, current.last, string.Join("", tmpMoveHistory));

            if (!visited.Contains(h))
            {
                visited.Add(h);
                prioQueue.Enqueue((next, current.last, string.Join("", tmpMoveHistory)), nextLoss);
            }
        }
    }
    return 0;
};

// var p1 = shortDist(3);

// Console.WriteLine($"P1: {p1}");

var p1 = shortDist(10, 4);

Console.WriteLine($"P1: {p1}");