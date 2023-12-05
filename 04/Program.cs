// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .Select(l =>
    {
        var gSplit = l.Split(":");
        var card = int.Parse(gSplit.First().Split(" ").Last());

        var bingo = gSplit.Last().Trim().Split("|");

        var wining = bingo.First().Split(" ").Where(s => !string.IsNullOrEmpty(s)).Select(win => int.Parse(win)).ToList();
        var having = bingo.Last().Split(" ").Where(s => !string.IsNullOrEmpty(s)).Select(have => int.Parse(have)).ToList();
        var wins = wining.Where(having.Contains);

        return (Card: card, wins: wins.Count());
    }).ToImmutableArray();

var p1 = input.Sum(ig => ig.wins > 0 ? Math.Pow(2, ig.wins - 1) : 0);
Console.WriteLine($"P1: {p1}");

var cardCount = input.ToDictionary(ig => ig.Card, ig => 0);
foreach (var ig in input)
{
    cardCount[ig.Card]++;
    for (int i = 1; i <= ig.wins; i++)
    {
        if (cardCount.ContainsKey(ig.Card + i))
            cardCount[ig.Card + i] += cardCount[ig.Card];
    }
}

Console.WriteLine($"P2: {cardCount.Sum(kv => kv.Value)}");
