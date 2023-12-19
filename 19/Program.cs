using System.Collections.Immutable;
using System.Text.RegularExpressions;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .ToImmutableList();

var workflows = new Dictionary<string, List<(string destination, string rating, int ratingValue, string logicOp)>>();
var ratings = new List<Dictionary<string, int>>();

Action<string> addToWorkflow = line =>
{
    var r = new Regex("^(.*)\\{(.*)\\}$");
    var matches = r.Match(line);
    var id = matches.Groups[1].Value;
    var ops = matches.Groups[2].Value.Split(",").Select(op =>
    {
        var r = (destination: "", rating: "", ratingValue: 0, logicOp: "");
        if (op.Contains(":"))
        {
            var opSplit = op.Split(":");
            var logic = "<";
            if (opSplit[0].Contains(">"))
            {
                logic = ">";
            }
            var logicSplit = opSplit[0].Split(logic);
            r = (destination: opSplit[1], rating: logicSplit[0], ratingValue: int.Parse(logicSplit[1]), logicOp: logic);
        }
        else
        {
            r = (destination: op, rating: "", ratingValue: 0, logicOp: "");
        }
        return r;
    }).ToList();
    workflows[id] = ops;
};

Action<string> addToRatings = line =>
{
    var r = new Regex("^\\{(.*)\\}$");
    var matches = r.Match(line);
    var ratingValues = matches.Groups[1].Value.Split(",").Select(v =>
    {
        var rSplit = v.Split("=");
        return (rating: rSplit[0], ratingValue: int.Parse(rSplit[1]));
    }).ToDictionary(kv => kv.rating, kv => kv.ratingValue);

    ratings.Add(ratingValues);
};

var isWorkflow = true;
foreach (var l in input)
{
    if (string.IsNullOrEmpty(l))
    {
        isWorkflow = false;
        continue;
    }
    if (isWorkflow)
    {
        addToWorkflow(l);
    }
    else
    {
        addToRatings(l);
    }
}

var p1 = 0;
foreach (var ratingValue in ratings)
{
    var ratingOutcome = "A";
    var currentSelector = "in";
    while (true)
    {
        var current = workflows[currentSelector];
        var nextSelector = string.Empty;

        var currentOpsEnum = current.GetEnumerator();
        while (currentOpsEnum.MoveNext())
        {
            var currentOp = currentOpsEnum.Current;

            var opEvaluation = true;
            if (!string.IsNullOrEmpty(currentOp.rating))
            {
                var value = ratingValue[currentOp.rating];
                opEvaluation = currentOp.logicOp == ">" ? value > currentOp.ratingValue : value < currentOp.ratingValue;
            }

            if (opEvaluation == true)
            {
                nextSelector = currentOp.destination;
                break;
            }
        }
        if (!string.IsNullOrEmpty(nextSelector))
        {
            if (nextSelector == "A" || nextSelector == "R")
            {
                ratingOutcome = nextSelector;
                break;
            }
            currentSelector = nextSelector;
        }
    }
    p1 += ratingOutcome switch
    {
        "A" => ratingValue.Sum(v => v.Value),
        _ => 0
    };
}

Console.WriteLine($"P1: {p1}");

Func<string, Dictionary<string, (int min, int max)>, ulong> rng = (currentS, ranges) => { return 0L; };
rng = (currentSelector, ranges) =>
{
    if (currentSelector == "R")
        return 0;

    if (currentSelector == "A")
        return ranges.Values.Aggregate(1UL, (acc, r) => acc * (ulong)(r.max - r.min + 1));

    var current = workflows[currentSelector];
    var result = 0UL;

    foreach (var currentOp in current)
    {
        if (string.IsNullOrEmpty(currentOp.rating))
        {
            result += rng(currentOp.destination, ranges);
        }
        else
        {
            var (min, max) = ranges[currentOp.rating];
            if (currentOp.logicOp == ">")
            {
                if (min > currentOp.ratingValue)
                {
                    result += rng(currentOp.destination, ranges);
                    return result;
                }

                if (max > currentOp.ratingValue)
                {
                    var tmpRanges = new Dictionary<string, (int min, int max)>(ranges)
                    {
                        [currentOp.rating] = (currentOp.ratingValue + 1, max)
                    };
                    result += rng(currentOp.destination, tmpRanges);

                    ranges[currentOp.rating] = (min, currentOp.ratingValue);
                }
            }
            else
            {
                if (max < currentOp.ratingValue)
                {
                    result += rng(currentOp.destination, ranges);
                    return result;
                }

                if (min < currentOp.ratingValue)
                {
                    var tmpRanges = new Dictionary<string, (int min, int max)>(ranges)
                    {
                        [currentOp.rating] = (min, currentOp.ratingValue - 1)
                    };
                    result += rng(currentOp.destination, tmpRanges);

                    ranges[currentOp.rating] = (currentOp.ratingValue, max);
                }
            }
        }
    }
    return result;
};

var accRanges = (new[] { "x", "m", "a", "s" }).ToDictionary(kv => kv, kv => (1, 4000));
var p2 = rng("in", accRanges);

Console.WriteLine($"P2: {p2}");
