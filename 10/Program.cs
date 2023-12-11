using System.Numerics;

var pipeNbrs = new List<(char c, IEnumerable<Vector2> n)> {
    (c: '|', n: new [] { new Vector2(-1, 0), new Vector2(1, 0)}),
    (c: '-', n: new[] {new Vector2(0, -1), new Vector2(0, 1)}),
    (c: 'L', n: new[] {new Vector2(-1, 0), new Vector2(0, 1)}),
    (c: 'J', n: new[] {new Vector2(-1, 0), new Vector2(0, -1)}),
    (c: '7', n: new[] {new Vector2(1, 0), new Vector2(0, -1)}),
    (c: 'F', n: new[] {new Vector2(1, 0), new Vector2(0, 1)}),
    (c: '.', n: []),
    (c: 'S', n: new[] {new Vector2(-1, 0), new Vector2(1, 0),new Vector2(0, -1), new Vector2(0, 1)})
}.ToDictionary(k => k.c, v => v.n);

var file = System.IO.File.OpenText("input.txt");
var matrix = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .SelectMany((line, i) => line.Select((c, j) => (pos: new Vector2(i, j), isPipe: c != '.', n: pipeNbrs[c])))
    .Where(v => v.isPipe)
    .ToDictionary(k => k.pos, v => v.n);

Func<Vector2, IEnumerable<Vector2>> nextInLoop = (point) =>
{
    return matrix[point].Select(n => point + n).Where(n1 => matrix.ContainsKey(n1));
};

var traverse = () =>
{
    var start = matrix.First(v => v.Value.Count() == 4);

    var visit = new HashSet<Vector2>();
    var toSee = new Queue<Vector2>();
    toSee.Enqueue(start.Key);
    while (toSee.Any())
    {
        var current = toSee.Dequeue();
        visit.Add(current);

        var nbrs = nextInLoop(current);
        foreach (var nbr in nbrs.Where(n => !visit.Contains(n)))
        {
            toSee.Enqueue(nbr);
        }
    }
    return visit;
};

var buildLoop = (HashSet<Vector2> path) =>
{
    var loop = new HashSet<Vector2>();
    var curr = path.First();
    while (true)
    {
        loop.Add(curr);

        path.Remove(curr);
        if (!path.Any())
        {
            break;
        }

        curr = nextInLoop(curr).First(n => path.Contains(n));
    }
    return loop;
};

var pipe = buildLoop(traverse());
Console.WriteLine($"P1: {pipe.Count / 2}");

var isContainedInsidePipe = (List<Vector2> pipe, Vector2 point) =>
{
    int i, j;
    var nVertexes = pipe.Count();
    bool isIn = false;
    for (i = 0, j = nVertexes - 1; i < nVertexes; j = i++)
    {
        if (((pipe[i].Y > point.Y) != (pipe[j].Y > point.Y)) &&
            (point.X < (pipe[j].X - pipe[i].X) * (point.Y - pipe[i].Y) / (pipe[j].Y - pipe[i].Y) + pipe[i].X))
        {
            isIn = !isIn;
        }
    }
    return isIn;
};

var coords = Enumerable.Range(0, (int) Math.Max(pipe.Max(s => s.X), pipe.Max(s => s.Y)));
var field = coords.SelectMany(i => coords.Select(j => new Vector2(i,j)));
var insideCount = field.Count(f => !pipe.Contains(f) && isContainedInsidePipe(pipe.ToList(), f));
Console.WriteLine($"P2: {insideCount}");
