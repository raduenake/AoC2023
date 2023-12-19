using System.Collections.Immutable;
using System.Numerics;

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



Func<string, int, int, bool> isValidMove = (history, min, max) =>
{
    var isValid = true;
    if (history.Count() > 1)
    {
        // at least 2 moves
        var lastTwoMoves = history[^2..];
        if (lastTwoMoves[0] != lastTwoMoves[1])
        {
            // turn?
            var lastMinMoves = history[..^1].TakeLast(min);
            isValid = lastMinMoves.Count() == min && lastMinMoves.Skip(1).All(m => m == lastMinMoves.First());
        }
        else
        {
            // is it continous?
            var lastMaxMoves = history[..^1].TakeLast(max).ToList();
            lastMaxMoves.Reverse();
            var cCnt = 0;
            foreach (var c in lastMaxMoves)
            {
                if (c != lastTwoMoves[1])
                {
                    break;
                }
                cCnt++;
            }
            isValid = cCnt >= min - 1 && cCnt < max;
        }
    }

    return isValid;
};

var shortDist = (int stepsInLine, int maxSteps) => { return 0; };
shortDist = (int stepsInLine, int maxSteps) =>
{
    var prioQueue = new PriorityQueue<(Vector2, Vector2, string, int), int>();
    var visited = new HashSet<int>();

    //start is "Free";
    prioQueue.Enqueue((Vector2.Zero, Vector2.Zero, "", 1), (-1) * input[Vector2.Zero]);
    (Vector2 last, Vector2 prev, string moveHistory, int stepsToGo) current;
    var loss = 0;
    while (prioQueue.TryDequeue(out current, out loss))
    {
        if (current.last.X == maxX && current.last.Y == maxY)
        {
            return loss + input[current.last];
        }

        var dirOfTravel = current.last != current.prev ? current.last - current.prev : Vector2.Zero;
        var dirUnits = new[] { (increment: Vector2.Zero, move: "_", stepsToGo: 0) };
        if (dirOfTravel == Vector2.Zero)
        {
            dirUnits = [
                (increment: Vector2.UnitX, move: moveDict[Vector2.UnitX], stepsInLine),
                (increment: Vector2.UnitY, move: moveDict[Vector2.UnitY], stepsInLine)
            ];
        }
        else
        {
            dirUnits = [
                (increment: rotateDeg(dirOfTravel, -90), move: moveDict[rotateDeg(dirOfTravel, -90)], stepsInLine),
                (increment: dirOfTravel, move: moveDict[dirOfTravel], current.stepsToGo),
                (increment: rotateDeg(dirOfTravel, 90), move: moveDict[rotateDeg(dirOfTravel, 90)], stepsInLine)
            ];
        }

        foreach (var dirUnit in dirUnits)
        {
            var stepsToGo = dirUnit.stepsToGo;

            var last = Vector2.Zero;
            var next = current.last;
            var nextLoss = loss;
            var moveHistory = current.moveHistory;

            var tmpVisited = new HashSet<int>();
            while (stepsToGo > 0)
            {
                last = next;

                next = last + dirUnit.increment;
                if (!input.ContainsKey(next))
                {
                    break;
                }

                moveHistory += dirUnit.move;
                nextLoss += input[last];

                var hMove = hash(next, last, string.Join("", moveHistory.TakeLast(maxSteps + 1)));
                tmpVisited.Add(hMove);

                stepsToGo--;
            }

            if (!input.ContainsKey(next) || !isValidMove(moveHistory, stepsInLine, maxSteps))
                continue;

            foreach (var t in tmpVisited.Take(tmpVisited.Count() - 1))
            {
                visited.Add(t);
            }

            var tmpMoveHistory = moveHistory.TakeLast(maxSteps + 1);
            var h = hash(next, last, string.Join("", tmpMoveHistory));

            if (!visited.Contains(h))
            {
                visited.Add(h);
                prioQueue.Enqueue((next, last, string.Join("", tmpMoveHistory), 1), nextLoss);
            }
        }
    }
    return 0;
};

var p1 = shortDist(1, 3);
Console.WriteLine($"P1: {p1}");

var p2 = shortDist(4, 10);
Console.WriteLine($"P1: {p2}");