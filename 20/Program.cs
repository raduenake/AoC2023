using System.Collections.Immutable;
using _20;

var file = System.IO.File.OpenText("input.txt");
var input = file.ReadToEnd()
    .Split("\n")
    .Where(l => !string.IsNullOrEmpty(l))
    .Select(l =>
    {
        var splits = l.Split("->");
        var typeAndId = splits[0].Trim();
        var gateId = string.Join("", typeAndId[1..]);
        var gate = typeAndId.First() switch
        {
            '&' => new Conjunction(gateId, splits[1]),
            '%' => new Toggle(gateId, splits[1]),
            _ => (Gate)new PassThrough("broadcaster", splits[1])
        };
        return gate;
    }
    ).ToImmutableArray();

var gates = input.ToDictionary(kv => kv.GateId, kv => kv);
foreach (var inputGate in input)
{
    var currentGate = gates[inputGate.GateId];
    foreach (var connectedGateId in currentGate.ConnectedIds)
    {
        if (!gates.ContainsKey(connectedGateId))
        {
            gates.Add(connectedGateId, new PassThrough(connectedGateId, ""));
        }
        currentGate.AddChild(gates[connectedGateId]);
    }
}

// assume this is a conjunction (RX gate is the "target")
var rxParent = (Conjunction)gates.First(g => g.Value.Connections.Any(go => go.GateId == "rx")).Value;

var lows = 0;
var highs = 0;
var iteration = 0;

while (true)
{
    var pulseQueue = new Queue<Pulse>();

    var pulse = new Pulse { FromGateId = "BTN", ToGateId = "broadcaster", PulseSignal = Signal.LOW, Iteration = iteration };
    pulseQueue.Enqueue(pulse);
    while (pulseQueue.TryDequeue(out pulse))
    {
        var currentGate = gates[pulse.ToGateId];
        var pulses = currentGate.HandlePulse(pulse);
        foreach (var p in pulses)
        {
            pulseQueue.Enqueue(p);
        }

        // part1 only cares about first 1000 pushes
        if (iteration < 1000)
        {
            lows += pulse.PulseSignal == Signal.LOW ? 1 : 0;
            highs += pulse.PulseSignal == Signal.HIGH ? 1 : 0;
        }
    }
    iteration++;
    
    // go until we find "actuation" iterations for all the parent of rx gate
    if (rxParent.ParentHistory.Count() == rxParent.ParentChangeIteration.Count())
    {
        break;
    }
}

Console.WriteLine($"P1: {lows * highs}");

// 0 based iteration in history; we stopped as soon as we found a way to activate all the inputs of RX gate
var p2 = MathNet.Numerics.Euclid.LeastCommonMultiple(rxParent.ParentChangeIteration.Select(kv => (long)kv.Value + 1).ToList());
Console.WriteLine($"P1: {p2}");
