using System.Collections.Immutable;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !String.IsNullOrEmpty(l))
    .ToImmutableList();

var firstPart = input
    .Select(l => $"{l.First(c => c >= '0' && c <= '9')}{l.Last(c => c >= '0' && c <= '9')}")
    .Sum(int.Parse);

Console.WriteLine($"P1: {firstPart}");

var map = Enumerable.Range(1, 9).Select(n =>
    {
        var ns = n switch
        {
            1 => "one",
            2 => "two",
            3 => "three",
            4 => "four",
            5 => "five",
            6 => "six",
            7 => "seven",
            8 => "eight",
            9 => "nine",
            _ => ""
        };
        return (ns, n);
    }
).ToImmutableList();

Func<string, char> extractFirstDigit = (inS) =>
{
    var result = inS;
    map.ForEach(nSn =>
    {
        result = result.Replace(nSn.ns, nSn.n.ToString());
    });
    return result.FirstOrDefault(c => c >= '0' && c <= '9');
};

var secondPart = input.Select(l =>
{
    char firstDigit = '\0';
    char lastDigit = '\0';

    var digitAccumulator = "";
    var i = 0;
    while (i < l.Count())
    {
        digitAccumulator += l[i];
        var v = extractFirstDigit(digitAccumulator);
        if (v != '\0')
        {
            firstDigit = v;
            break;
        }
        i++;
    }

    digitAccumulator = "";
    i = l.Count() - 1;
    while (i >= 0)
    {
        digitAccumulator = l[i] + digitAccumulator;
        var v = extractFirstDigit(digitAccumulator);
        if (v != '\0')
        {
            lastDigit = v;
            break;
        }
        i--;
    }

    return $"{firstDigit}{lastDigit}";
}).Sum(int.Parse);

Console.WriteLine($"P2: {secondPart}");