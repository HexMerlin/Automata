﻿namespace Automata.Core.Operations;

public static partial class Ops
{

    /// <summary>
    /// Adds a Kleene plus closure (one or many) to the specified automaton.
    /// </summary>
    /// <param name="source">Automaton to modify</param>
    /// <returns>Modified <paramref name="source"/> with Kleene Plus</returns>
    public static Nfa KleenePlusWith(this Nfa source)
    {
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

        return source;
    }
}

