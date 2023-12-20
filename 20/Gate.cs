namespace _20
{
    public enum Signal
    {
        UNDEFINED,
        LOW,
        HIGH
    }

    public class Pulse
    {
        public Pulse()
        {
            FromGateId = "";
            ToGateId = "";
            PulseSignal = Signal.UNDEFINED;
            Iteration = 0;
        }

        public Pulse(Pulse other)
        {
            FromGateId = other.FromGateId;
            ToGateId = other.ToGateId;
            PulseSignal = other.PulseSignal;
            Iteration = other.Iteration;
        }

        public string FromGateId;
        public string ToGateId;
        public Signal PulseSignal;
        public int Iteration;
    }

    public abstract class Gate
    {
        public List<string> ConnectedIds { get; }

        protected Gate(string gateId, string connected)
        {
            GateId = gateId;
            ConnectedIds = connected.Split(",").Select(c => c.Trim()).ToList();
            Connections = new List<Gate>();
            State = Signal.UNDEFINED;
        }

        public string GateId;
        public Signal State { get; protected set; }
        public List<Gate> Connections;

        public void AddChild(Gate connectedGate)
        {
            Connections.Add(connectedGate);
            connectedGate.AddParent(this);
        }

        protected virtual void AddParent(Gate parentGate)
        {
        }

        public virtual List<Pulse> HandlePulse(Pulse pulse)
        {
            State = pulse.PulseSignal;
            return Connections.Select(connGate =>
                new Pulse(pulse)
                {
                    FromGateId = GateId,
                    ToGateId = connGate.GateId
                }
                ).ToList();
        }
    }

    public class PassThrough : Gate
    {
        public PassThrough(string gateId, string connected) : base(gateId, connected)
        {
        }
    }

    public class Toggle : Gate
    {
        // if receives a high pulse, it is ignored and nothing happens.
        // if a flip-flop module receives a low pulse, it toggles state.
        public Toggle(string gateId, string connected) : base(gateId, connected)
        {
            // starts as a LOW signal
            State = Signal.LOW;
        }

        public override List<Pulse> HandlePulse(Pulse pulse)
        {
            if (pulse.PulseSignal == Signal.HIGH)
            {
                return Enumerable.Empty<Pulse>().ToList();
            }
            State = State == Signal.LOW ? Signal.HIGH : Signal.LOW;
            return base.HandlePulse(new Pulse(pulse) { PulseSignal = State });
        }
    }

    public class Conjunction : Gate
    {
        // remember the type of the most recent pulse received from each of their connected input modules; 
        // they initially default to remembering a low pulse for each input. 
        // when a pulse is received, the conjunction module first updates its memory for that input. 
        // then, if it remembers high pulses for all inputs, it sends a low pulse; otherwise, it sends a high pulse.
        public Conjunction(string gateId, string connected) : base(gateId, connected)
        {
            ParentHistory = new Dictionary<string, Signal>();
            ParentChangeIteration = new Dictionary<string, int>();
        }

        protected override void AddParent(Gate parentGate)
        {
            ParentHistory[parentGate.GateId] = Signal.LOW;
        }

        public Dictionary<string, Signal> ParentHistory { get; private set; }

        public Dictionary<string, int> ParentChangeIteration { get; private set; }

        public override List<Pulse> HandlePulse(Pulse pulse)
        {
            if (pulse.PulseSignal != ParentHistory[pulse.FromGateId]) {
                ParentChangeIteration[pulse.FromGateId] = pulse.Iteration;
            }

            ParentHistory[pulse.FromGateId] = pulse.PulseSignal;
            State = ParentHistory.All(kv => kv.Value == Signal.HIGH) ? Signal.LOW : Signal.HIGH;

            return base.HandlePulse(new Pulse(pulse) { PulseSignal = State });
        }
    }
}