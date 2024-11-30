namespace Automata;

public readonly record struct Transition(int FromState, int Symbol, int ToState)  : IComparable<Transition>
{
    public Transition Reverse() => new Transition(ToState, Symbol, FromState);

    public Transition() : this(-1, -1, -1)
    {
    }

    public int CompareTo(Transition other)
    {
        int c = FromState.CompareTo(other.FromState);
        if (c != 0) return c;
       
        c = Symbol.CompareTo(other.Symbol);
        if (c != 0) return c;
        
        return ToState.CompareTo(other.ToState);
    }


    public static Comparer<Transition> CompareByToState() => Comparer<Transition>.Create((t1, t2) =>
    {
        int c = t1.ToState.CompareTo(t2.ToState);
        if (c != 0) return c;

        c = t1.Symbol.CompareTo(t2.Symbol);
        if (c != 0) return c;

        return t1.FromState.CompareTo(t2.FromState);
    });
}
