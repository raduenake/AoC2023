// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;

Console.WriteLine("Hello, World!");

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .SelectMany((l, i) => l.Select((c, j) => (loc: (i: i, j: j), sym: c)))
    .Where(x => x.sym != '.')
    .ToImmutableArray();

var nonNumbers = input.Where(stuff => stuff.sym < '0' || stuff.sym > '9').ToImmutableArray();
var allPartNumbers = input.Where(stuff => !nonNumbers.Contains(stuff));

Func<(int i, int j), IEnumerable<(int i, int j)>> getNbr = ((int i, int j) x) =>
{
    /*
    x x x
    x p x
    x x x
    */
    var nbrs = new List<(int i, int j)>() {
        (x.i-1, x.j-1),
        (x.i-1, x.j),
        (x.i-1, x.j+1),
        (x.i, x.j-1),
        (x.i, x.j+1),
        (x.i+1,x.j-1),
        (x.i+1, x.j),
        (x.i+1, x.j+1)
    };

    return nbrs;
};

Func<IEnumerable<((int i, int j) loc, char sym)>, int> getPartNo = (IEnumerable<((int i, int j) loc, char sym)> pNo) =>
{
    int x = 0;
    var isPart = pNo.Any(dd => getNbr(dd.loc).Any(ddLocNbr => nonNumbers.Any(nnLoc => ddLocNbr == nnLoc.loc)));
    if (isPart)
    {
        x = int.Parse(string.Join("", pNo.Select(pp => pp.sym)));
    }
    return x;
};

var actualPartNumbers = new List<(int pNo, IEnumerable<(int i, int j)> locations)>();

var enumOverPartNumbers = allPartNumbers.GetEnumerator();
var partNumber = Enumerable.Empty<((int i, int j) loc, char sym)>().ToList();
while (enumOverPartNumbers.MoveNext())
{
    var digit = enumOverPartNumbers.Current;
    var prevDigit = partNumber.LastOrDefault();

    if (partNumber.Any() && (prevDigit.loc.j + 1 != digit.loc.j || prevDigit.loc.i != digit.loc.i))
    {
        //test part for neighbors and get it's Number Out
        int x = getPartNo(partNumber);
        if (x > 0)
        {
            actualPartNumbers.Add((x, partNumber.Select(pn => pn.loc)));
        }

        // reset the partNumber aggregator
        partNumber = Enumerable.Empty<((int i, int j) loc, char sym)>().ToList();
    }

    partNumber.Add(digit);
}
if (partNumber.Any())
{
    int x = getPartNo(partNumber);
    if (x > 0)
    {
        actualPartNumbers.Add((x, partNumber.Select(pn => pn.loc)));
    }
}

Console.WriteLine($"P1: {actualPartNumbers.Sum(aPN => aPN.pNo)}");

var gears = nonNumbers.Where(nn => nn.sym == '*');
var gRatio = gears.Sum(gear =>
{
    var gearRatio = 0;
    var gearNbrs = getNbr(gear.loc);
    var nbrParts = actualPartNumbers.Where(apn => gearNbrs.Any(gn => apn.locations.Contains(gn)));
    if (nbrParts.Count() == 2)
    {
        gearRatio = nbrParts.Aggregate(1, (acc, item) => acc * item.pNo);
    }
    return gearRatio;
});

Console.WriteLine($"P2: {gRatio}");