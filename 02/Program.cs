using System.Collections.Immutable;

Func<(int R, int G, int B), (int R, int G, int B), (int R, int G, int B)> addT = ((int R, int G, int B) x, (int R, int G, int B) y) =>
{
    return (x.R + y.R, x.G + y.G, x.B + y.B);
};

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .Select(l =>
    {
        var gSplit = l.Split(":");
        var game = int.Parse(gSplit.First().Split(" ").Last());

        var draws = gSplit.Last().Trim().Split(";")
            .Select(gLine =>
            {
                var drawColors = gLine.Trim().Split(",").Select(color => color.Trim()).ToList();
                var drw = drawColors.Aggregate((R: 0, G: 0, B: 0), ((int R, int G, int B) acc, string next) =>
                {
                    var drwCS = next.Split(" ");
                    var drwCSVal = int.Parse(drwCS.First());
                    var nextVal = drwCS.Last() switch
                    {
                        "red" => (R: drwCSVal, G: 0, B: 0),
                        "green" => (R: 0, G: drwCSVal, B: 0),
                        "blue" => (R: 0, G: 0, B: drwCSVal),
                        _ => (R: 0, G: 0, B: 0)
                    };
                    return addT(acc, nextVal);
                });
                return drw;
            }).ToList();
        return (Game: game, Draws: draws);
    })
    .ToImmutableArray();

Func<(int R, int G, int B), (int R, int G, int B), bool> isGamePossible = ((int R, int G, int B) game, (int R, int G, int B) dice) =>
{
    return game.R <= dice.R && game.G <= dice.G && game.B <= dice.B;
};

var dice = (R: 12, G: 13, B: 14);
int sumOfPossibleGames = input.Sum(i => i.Draws.All(d => isGamePossible(d, dice)) ? i.Game : 0);
Console.WriteLine($"P1: {sumOfPossibleGames}");

var sumOfPowers = input.Sum(g =>
{
    var fewestCubes = (R: 0, G: 0, B: 0);
    foreach (var draw in g.Draws)
    {
        fewestCubes.R = draw.R > fewestCubes.R ? draw.R : fewestCubes.R;
        fewestCubes.G = draw.G > fewestCubes.G ? draw.G : fewestCubes.G;
        fewestCubes.B = draw.B > fewestCubes.B ? draw.B : fewestCubes.B;
    }
    return fewestCubes.R * fewestCubes.G * fewestCubes.B;
});
Console.WriteLine($"P1: {sumOfPowers}");