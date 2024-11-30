namespace Automata;

public readonly record struct EpsilonTransition(int FromState, int ToState) : IComparable<EpsilonTransition>
{

    /// <summary>
    /// Denotes the string representation for ε - the empty epsilon string.
    /// Epsilon is not part of any alphabet, the symbol is used solely for output purposes, never for computation.
    /// </summary>
    public const string Epsilon = "ε";

    public EpsilonTransition Reverse() => new EpsilonTransition(ToState, FromState);


    public int CompareTo(EpsilonTransition other)
    {
        int fromStateComparison = FromState.CompareTo(other.FromState);
        return fromStateComparison != 0 ? fromStateComparison : ToState.CompareTo(other.ToState);
    }

    public static Comparer<EpsilonTransition> CompareByToState() => Comparer<EpsilonTransition>.Create((t1, t2) =>
    {
        int c = t1.ToState.CompareTo(t2.ToState);
        return c != 0 ? c : t1.FromState.CompareTo(t2.FromState);
    });
}

