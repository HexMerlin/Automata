using System.Collections;
using System.Runtime.CompilerServices;

namespace Automata.Core.TransitionSets;

/// <summary>
/// A highly optimized immutable set of deterministic transitions.
/// </summary>
/// <remarks>
/// <see cref="ImmutableTransitions"/> is characterized by:
/// <list type="bullet">
/// <item>
/// <description>
/// <b>Immutable:</b> Structural and behavioral invariance, ensuring thread safety and predictable behavior.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Performance:</b> Optimized for fast, read-only operations with minimal memory overhead.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Ordering:</b> Transitions are sorted according to the default order of <see cref="Core.Transition"/>.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Deterministic:</b> The set cannot contain more than one transition with a specific (FromState, Symbol) pair.
/// </description>
/// </item>
/// </list>
/// </remarks>
public class ImmutableTransitions : IEnumerable<Transition>
{
    private readonly Transition[] transitions;

    public readonly int StateCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableTransitions"/> class, ensuring the input is sorted and deterministic.
    /// </summary>
    /// <param name="transitions">The input set of transitions.</param>
    /// <exception cref="ArgumentException">Thrown if the transitions are not deterministic.</exception>
    public ImmutableTransitions(IEnumerable<Transition> transitions)
    { 
        this.transitions = transitions.OrderBy(t => t).ToArray();
        this.StateCount = 1 + MaxStateAndAssert(this.transitions);
    }

    /// <returns>The number of transitions in the set.</returns>
    public int TransitionCount => transitions.Length;

    /// <summary>
    /// Returns the transition from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The source state.</param>
    /// <param name="symbol">The symbol of the transition.</param>
    /// <returns>
    /// The transition from the given state with the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Transition Transition(int fromState, int symbol)
        => new ReadOnlySpan<Transition>(transitions).Transition(fromState, symbol);

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The source state.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> containing the transitions from the given state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<Transition> Transitions(int fromState)
        => new ReadOnlySpan<Transition>(transitions).Transitions(fromState);

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReachableState(int fromState, int symbol)
        => Transition(fromState, symbol).ToState;

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <remarks>Since the underlying set is deterministic, the returned symbols is a proper <c>set</c>, meaning every symbol can occur only once.</remarks>
    /// <returns>The set of symbols that can be used to transition directly from the given state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int[] AvailableSymbols(int fromState)
       => Transitions(fromState).AvailableSymbols(fromState);

    /// <summary>
    /// Returns a set of symbols that can be used to transition directly from the given states.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    public IntSet AvailableSymbols(IEnumerable<int> fromStates)
    {
        HashSet<int> symbols = new();
        foreach (int fromState in fromStates)
        {
            var transitions = this.Transitions(fromState);
            for (int i = 0; i < transitions.Length; i++)
                symbols.Add(transitions[i].Symbol);
        }
        return new IntSet(symbols);
    }
     
    /// <summary>
    /// Finds the maximum state in the set of transitions.
    /// Also asserts that the transitions are deterministic.
    /// </summary>
    /// <param name="transitions">The transition array</param>
    /// <returns>The maximum state referenced, or <see cref="Constants.InvalidState"/> if the array is empty.</returns>
    /// <exception cref="ArgumentException">If the transitions are not deterministic.</exception>
    private static int MaxStateAndAssert(Transition[] transitions)
    {
        int maxState = Constants.InvalidState;
        int fromState = Constants.InvalidState;
        int symbol = Constants.InvalidSymbolIndex;
     
        for (int i = 0; i < transitions.Length; i++)
        { 
            Transition t = transitions[i];
            if (t.FromState == fromState && t.Symbol == symbol)
                throw new ArgumentException("The transitions must be deterministic: every (FromState, Symbol)-tuple must be unique.");
            (fromState, symbol, int toState) = t;
            maxState = Math.Max(maxState, Math.Max(fromState, toState));
        }
        return maxState;
    }

    public IEnumerator<Transition> GetEnumerator() => ((IEnumerable<Transition>)transitions).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



  


}
