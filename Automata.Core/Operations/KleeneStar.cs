namespace Automata.Core.Operations;

public static partial class Ops
{
    /// <summary>
    /// Adds a Kleene star closure to the specified automaton.
    /// </summary>
    /// <param name="source">Automaton to modify</param>
    /// <returns>Modified <paramref name="source"/> with Kleene Star</returns>
    public static Nfa KleeneStarInPlace(this Nfa source)
    {
        // Kleene star of an empty language is an empty language
        if (source.IsEmptyLanguage)
            return source;

        // Create new initial state
        int newInitialState = source.MaxState + 1;

        // Add epsilon transitions from new initial state to original initial states
        foreach (var state in source.InitialStates)
            source.Add(new EpsilonTransition(newInitialState, state));

        // Remove all initial states
        source.ClearInitialStates();

        // Assign new initial state
        source.SetInitial(newInitialState);

        // Add epsilon transitions from final states back to the new initial state
        foreach (var finalState in source.FinalStates)
            source.Add(new EpsilonTransition(finalState, newInitialState));

        // Remove all final states
        source.ClearFinalStates();

        //Assign the new initial state as the only final state
        source.SetFinal(newInitialState);

        return source;
    }

}

