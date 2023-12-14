using System.Collections.Immutable;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .SelectMany((l, i) =>
    {
        return l.Select((c, j) => (Pos: (X: i, Y: j), Ch: c));
    })
    .ToImmutableArray();

Func<char[,], char[,]> rollNorth = (inputTiles) =>
{
    var stableTiles = (char[,])inputTiles.Clone();

    var stable = false;
    while (!stable)
    {
        stable = true;
        for (int i = 0; i < stableTiles.GetLength(0); i++)
            for (int j = 0; j < stableTiles.GetLength(1); j++)
            {
                var lineIdx = i;
                while (
                    lineIdx >= 1 &&
                    stableTiles[lineIdx, j] == 'O' &&
                    stableTiles[lineIdx - 1, j] == '.')
                {
                    stableTiles[lineIdx, j] = '.';
                    stableTiles[lineIdx - 1, j] = 'O';
                    lineIdx--;
                    stable = false;
                }
            }
    }
    return stableTiles;
};

Func<char[,], int> solution = (inputTiles) =>
{
    var sol = 0;
    for (int i = 0; i < inputTiles.GetLength(0); i++)
        for (int j = 0; j < inputTiles.GetLength(1); j++)
            sol += inputTiles[i, j] switch { 'O' => inputTiles.GetLength(0) - i, _ => 0 };
    return sol;
};

var partOne = new char[input.Max(t => t.Pos.X) + 1, input.Max(t => t.Pos.Y) + 1];
foreach (var t in input)
{
    partOne[t.Pos.X, t.Pos.Y] = t.Ch;
}

partOne = rollNorth(partOne);
Console.WriteLine($"P1: {solution(partOne)}");

Func<char[,], int> getTilesHash = (tiles) =>
{
    var s = "";
    foreach (var c in tiles)
    {
        s += c;
    }
    return s.GetHashCode();
};

Func<char[,], char[,]> rotate = (inputTiles) =>
{
    var rotated = new char[inputTiles.GetLength(1), inputTiles.GetLength(0)];
    for (int i = 0; i < inputTiles.GetLength(0); i++)
        for (int j = 0; j < inputTiles.GetLength(1); j++)
            rotated[j, inputTiles.GetLength(0) - i - 1] = inputTiles[i, j];

    return rotated;
};

var totalCycles = 1_000_000_000;
var history = new Dictionary<int, char[,]>();

var lastComputedHash = 0;
var partTwoTempTiles = new char[input.Max(t => t.Pos.X) + 1, input.Max(t => t.Pos.Y) + 1];
foreach (var t in input)
{
    partTwoTempTiles[t.Pos.X, t.Pos.Y] = t.Ch;
}

var stepsUntilCycle = 0;
while (stepsUntilCycle < totalCycles)
{
    var initialHash = getTilesHash(partTwoTempTiles);
    if (history.ContainsKey(initialHash))
    {
        lastComputedHash = initialHash;
        break;
    }
    foreach (var _ in Enumerable.Range(0, 4))
    {
        partTwoTempTiles = rollNorth(partTwoTempTiles);
        partTwoTempTiles = rotate(partTwoTempTiles);
    }
    history.Add(initialHash, (char[,])partTwoTempTiles.Clone());
    stepsUntilCycle++;
}

var cyle = new List<int>();
while (!cyle.Contains(lastComputedHash))
{
    cyle.Add(lastComputedHash);
    lastComputedHash = getTilesHash(history[lastComputedHash]);
}

long toGo = totalCycles - stepsUntilCycle - 1;
long last = toGo % cyle.Count;
var partTwo = history[cyle[(int)last]];

Console.WriteLine($"P2: {solution(partTwo)}");
