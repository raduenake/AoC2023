using System.Security.Cryptography;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd().Split("\n");

var inputPatterns = new List<List<((int x, int y) pos, char ch)>>();
var current = new List<((int x, int y) pos, char ch)>();
var i = 0;
foreach (var l in input)
{
    if (string.IsNullOrEmpty(l))
    {
        inputPatterns.Add(current);
        current = new List<((int x, int y) pos, char ch)>();
        i = 0;
        continue;
    }

    current.AddRange(l.Select((ch, j) => (pos: (x: i, y: j), ch)));

    i++;
}

Func<List<((int x, int y) pos, char ch)>, int, int, bool> isColumnMirror = (pattern, c1, c2) =>
{
    var isMirror = pattern.Where(pp => pp.pos.y == c1).All(ppCol =>
        pattern.Exists(ppCol2 =>
            ppCol2.pos.y == c2 &&
            ppCol2.pos.x == ppCol.pos.x &&
            ppCol.ch == ppCol2.ch));
    return isMirror;
};

Func<List<((int x, int y) pos, char ch)>, int, int, bool> isLineMirror = (pattern, l1, l2) =>
{
    var isMirror = pattern.Where(pp => pp.pos.x == l1).All(ppLin =>
        pattern.Exists(ppLin2 =>
            ppLin2.pos.x == l2 &&
            ppLin2.pos.y == ppLin.pos.y &&
            ppLin.ch == ppLin2.ch));
    return isMirror;
};

// force finding a different reflection line using the prev value
Func<List<((int x, int y) pos, char ch)>, int, int> getMirrorReflection = (pattern, prevValue) =>
{
    Func<List<((int x, int y) pos, char ch)>, List<((int x, int y) pos, char ch)>> colsToLines = (pattern) =>
    {
        int maxX = pattern.Max(pp => pp.pos.x);
        int maxY = pattern.Max(pp => pp.pos.y);

        return pattern.Select(pp => (pos: (pp.pos.y, pp.pos.x), ch: pp.ch)).OrderBy(pp => pp.pos).ToList();
    };

    Func<List<((int x, int y) pos, char ch)>, int, int, int> getLine = (tempPattern, coef, prev) =>
    {
        var line = 0;
        while (line <= tempPattern.Max(p => p.pos.x))
        {
            var top = Enumerable.Range(0, line + 1).OrderDescending();
            var bottom = Enumerable.Range(line + 1, tempPattern.Max(p => p.pos.x) - line);
            var min = Math.Min(top.Count(), bottom.Count());
            var isMirrored = min > 0 && top.Take(min).Zip(bottom.Take(min)).All(lm => isLineMirror(tempPattern, lm.First, lm.Second));
            if (isMirrored)
            {
                var r = coef * (line + 1);
                if (r != prev)
                {
                    return r;
                }
            }
            line++;
        }
        return 0;
    };

    // find line
    var lineResponse = getLine(pattern, 100, prevValue);
    if (lineResponse != 0) {
        return lineResponse;
    }
    
    // try the columns
    var transposeColsToLines = colsToLines(pattern);
    lineResponse = getLine(transposeColsToLines, 1, prevValue);

    return lineResponse;
};

var p1 = 0;
var p2 = 0;
foreach (var inPattern in inputPatterns)
{
    var r = getMirrorReflection(inPattern, 0);
    p1 += r;

    foreach (var smudge in inPattern)
    {
        var smudgeFree = inPattern.Select(pp => pp.pos == smudge.pos ? (pp.pos, pp.ch == '.' ? '#' : '.') : pp).ToList();
        var r1 = getMirrorReflection(smudgeFree, r);

        if (r1 != 0)
        {
            p2 += r1;
            break;
        }
    }
}

Console.WriteLine($"P1: {p1}");
Console.WriteLine($"P2: {p2}");
