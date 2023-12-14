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

var partOne = new char[input.Max(t => t.Pos.X) + 1, input.Max(t => t.Pos.Y) + 1];
foreach (var t in input)
{
    partOne[t.Pos.X, t.Pos.Y] = t.Ch;
}

partOne = rollNorth(partOne);
Console.WriteLine($"P1: {solution(partOne)}");


var partTwoTemp = new char[input.Max(t => t.Pos.X) + 1, input.Max(t => t.Pos.Y) + 1];
foreach (var t in input)
{
    partTwoTemp[t.Pos.X, t.Pos.Y] = t.Ch;
}

var history = new Dictionary<int, char[,]>();
var setLoop = new List<int>();

var totalCycles = 1_000_000_000;
var lastHash = 0;
var stepsUntilLoop = 0;
while (true)
{
    var hash = getTilesHash(partTwoTemp);
    if (history.ContainsKey(hash))
    {
        // loop
        lastHash = hash;
        break;
    }

    foreach (var _ in Enumerable.Range(0, 4))
    {
        partTwoTemp = rotate(rollNorth(partTwoTemp));
    }

    history.Add(hash, partTwoTemp);
    stepsUntilLoop++;
}

// build loop
while (!setLoop.Contains(lastHash))
{
    setLoop.Add(lastHash);
    lastHash = getTilesHash(history[lastHash]);
}

var remainingCycles = totalCycles - stepsUntilLoop - 1;
var lastTileIdx = remainingCycles % setLoop.Count;
var partTwo = history[setLoop[lastTileIdx]];

Console.WriteLine($"P2: {solution(partTwo)}");
