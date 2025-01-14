namespace Automata.Core.Operations;

public static partial class Ops
{
    /// <summary>
    /// Applying Optional closure (?).
    /// Ensures the NFA accepts the empty string (ε), modifying it to represent L? = L ∪ {ε}.
    /// If the NFA already accepts ε, it is returned unmodified.
    /// </summary>
    /// <param name="source">The source NFA to modify in place.</param>
    /// <returns>The same automaton, with potential modification.</returns>
    public static Nfa OptionWith(this Nfa source)
    {
        if (source.IsEmptyLanguage)
            return source; // (empty language)? = empty language

        if (source.AcceptsEpsilon)
            return source; // Already accepts epsilon, return source

        int newState = source.MaxState + 1;
        source.Add(new EpsilonTransition(source.InitialStates.First(), newState));
        source.SetFinal(newState);
        return source;
    }
}