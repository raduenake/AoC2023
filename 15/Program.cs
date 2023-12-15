using System.Collections.Immutable;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .SelectMany(l => l.Split(",").Select(spl => spl.Trim()))
    .ToImmutableArray();

Func<string, int> getHash = inS => inS.Aggregate(0, (acc, it) => acc = 17 * (acc + it) % 256);

var p1 = input.Aggregate(0, (acc, it) => acc += getHash(it));
Console.WriteLine($"P1: {p1}");

var partTwoOps = input.Select(op =>
{
    var iOfEqSign = op.IndexOf("=");
    var label = (iOfEqSign > 0) switch
    {
        true => op.Substring(0, iOfEqSign),
        false => op.Substring(0, op.Length - 1)
    };
    var focal = iOfEqSign > 0 ? int.Parse(op.Substring(iOfEqSign + 1, op.Length - iOfEqSign - 1)) : -1;
    return (boxIdx: getHash(label), label: label, focal: focal);
}).ToImmutableArray();

var boxes = Enumerable.Range(0, 256).Select(_ => new List<(string label, int focal)>()).ToArray();
foreach (var op in partTwoOps)
{
    // remove
    if (op.focal == -1)
    {
        boxes[op.boxIdx].RemoveAll(bc => bc.label == op.label);
        continue;
    }

    // add
    var exists = false;
    for (int lensIdx = 0; lensIdx < boxes[op.boxIdx].Count(); lensIdx++)
    {
        if (boxes[op.boxIdx][lensIdx].label == op.label)
        {
            boxes[op.boxIdx][lensIdx] = (op.label, op.focal);
            exists = true;
        }
    }
    if (!exists)
    {
        boxes[op.boxIdx].Add((op.label, op.focal));
    }
}

var p2 = 0;
for (int boxIdx = 0; boxIdx < boxes.Count(); boxIdx++)
{
    var boxFocus = boxes[boxIdx]
        .Select((lens, lensIdx) => (lensIdx + 1) * lens.focal)
        .Aggregate(0, (acc, lensFocal) => acc += (boxIdx + 1) * lensFocal);
    p2 += boxFocus;
}
Console.WriteLine($"P2: {p2}");
