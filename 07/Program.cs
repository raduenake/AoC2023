using System.Collections.Immutable;
using System.Data;
using _07;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .Select(l =>
    {
        var cardAndBet = l.Split(" ");
        return (hand: cardAndBet.First().Trim().Select(c => "" + c).ToList(), bet: int.Parse(cardAndBet.Last().Trim()));
    })
    .ToImmutableArray();

var sort = input.OrderBy(hW => hW.hand, new CardComparer());
Console.WriteLine($"P1: {sort.Select((sh, i) => sh.bet * (i + 1)).Sum()}");

var sortJ = input.OrderBy(hW => hW.hand, new CardComparerJoker());
Console.WriteLine($"P1: {sortJ.Select((sh, i) => sh.bet * (i + 1)).Sum()}");
