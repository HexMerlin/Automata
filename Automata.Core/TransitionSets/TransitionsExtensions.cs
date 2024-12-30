using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace Automata.Core.TransitionSets;

/// <summary>
/// This class provides extension methods for <see cref="SortedSet{Transition}"/>.
/// <para>These provide fast extraction of subsets from various transition sets, such as <see cref="ImmutableTransitions"/>, <see cref="DeterministicTransitions"/> and <see cref="NonDeterministicTransitions"/>.</para>
/// <para>Such subsets are either of type <see cref="ReadOnlySpan{Transition}"/> or <see cref="SortedSet{Transition}"/>, enabling applying these extensions methods for search or refined filtering on the subsets</para>
/// </summary>
public static class TransitionsExtensions
{

    /// <summary>
    /// Returns the transition from the given state with the given symbol.
    /// </summary>
    /// <param name="transitions">The set of transitions to search.</param>
    /// <param name="fromState">The from state.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <remarks><paramref name="transitions"/> must be deterministic and sorted with the default Transition comparer. Otherwise, result is undefined.</remarks>
    /// <returns>The transition from the given state on the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transition Transition(this SortedSet<Transition> transitions, int fromState, int symbol)
       => transitions.GetViewBetween(MinTrans(fromState, symbol), MaxTrans(fromState, symbol)).FirstOrDefault(Core.Transition.Invalid);

    /// <summary>
    /// Returns the transition from the given state with the given symbol.
    /// </summary>
    /// <param name="transitions">The span of transitions to search.</param>
    /// <param name="fromState">The from state.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <remarks><paramref name="transitions"/> must be deterministic and sorted with the default Transition comparer. Otherwise, result is undefined.</remarks>
    /// <returns>The transition from the given state on the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transition Transition(this ReadOnlySpan<Transition> transitions, int fromState, int symbol)
    {
        int index = transitions.BinarySearch(new Transition(fromState, symbol, Constants.InvalidState));
        Debug.Assert(index < 0, $"Binary search returned a non-negative index ({index}), which should be impossible given the search key.");
        index = ~index; // Get the insertion point
        return (index < transitions.Length && transitions[index].FromState == fromState && transitions[index].Symbol == symbol)
            ? transitions[index]
            : Core.Transition.Invalid;
    }

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="transitions">The set of transitions to search.</param>
    /// <param name="fromState">The from state.</param>
    /// <remarks><paramref name="transitions"/> must be deterministic and sorted with the default Transition comparer. Otherwise, result is undefined.</remarks>
    /// <returns>The set of transitions from the given state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SortedSet<Transition> Transitions(this SortedSet<Transition> transitions, int fromState)
        => transitions.GetViewBetween(MinTrans(fromState), MaxTrans(fromState));

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="transitions">The set of transitions to search.</param>
    /// <param name="fromState">The from state.</param>
    /// <remarks><paramref name="transitions"/> must be deterministic and sorted with the default Transition comparer. Otherwise, result is undefined.</remarks>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> containing the transitions from the given state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<Transition> Transitions(this ReadOnlySpan<Transition> transitions, int fromState)
    {
        int startIndex = transitions.BinarySearch(new Transition(fromState, Constants.InvalidSymbolIndex, Constants.InvalidState));
        Debug.Assert(startIndex < 0, $"Binary search returned a non-negative index ({startIndex}), which should be impossible given the search key.");
        startIndex = ~startIndex;
        if (startIndex >= transitions.Length || transitions[startIndex].FromState != fromState)
            return [];  // If no matches exist, return an empty span
         
        int endIndex = transitions.Slice(startIndex + 1).BinarySearch(new Transition(fromState + 1, Constants.InvalidSymbolIndex, Constants.InvalidState));
        Debug.Assert(endIndex < 0, $"Binary search returned a non-negative index ({endIndex}), which should be impossible given the search key.");
        endIndex = ~endIndex;
        return transitions[startIndex..endIndex];
    }

    /// <summary>
    /// Returns the transitions from the given state with the given symbol.
    /// </summary>
    /// <param name="transitions">The set of transitions to search. Can be deterministic.</param>
    /// <param name="fromState">The from state.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <remarks>For this method:
    /// <para><paramref name="transitions"/> are allowed to be non-deterministic. Hence, more than one Transition can be returned.</para>
    /// <para><paramref name="transitions"/> must be sorted with the default Transition comparer. Otherwise, result is undefined.</para>
    /// </remarks>
    /// <returns>The transitions from the given state on the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SortedSet<Transition> TransitionsNonDeterministic(this SortedSet<Transition> transitions, int fromState, int symbol)
       => transitions.GetViewBetween(MinTrans(fromState, symbol), MaxTrans(fromState, symbol));

    /// <summary>
    /// Enumerates all symbols that can be used to transition directly from the given state.
    /// </summary>
    /// <param name="transitions">The set of transitions to search.</param>
    /// <param name="fromState">The from state.</param>
    /// <remarks>
    /// This enumerates over the symbols which is fast. 
    /// <para>If <paramref name="transitions"/> are deterministic, the result will be a proper <c>set</c> (no duplicated symbols)</para>
    /// <para>If <paramref name="transitions"/> are non-deterministic, the result can contain duplicated symbols.</para>
    /// </remarks>
    /// <returns>The collection of symbols that can be used to transition directly from the given state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<int> AvailableSymbols(this SortedSet<Transition> transitions, int fromState)
        => transitions.Transitions(fromState).Select(t => t.Symbol);

    /// <summary>
    /// Enumerates all symbols that can be used to transition directly from the given state.
    /// </summary>
    /// <param name="transitions">The set of transitions to search.</param>
    /// <param name="fromState">The state from which to start.</param>
    /// <remarks>
    /// <para>If <paramref name="transitions"/> are deterministic, the result will be a proper <c>set</c> (no duplicated symbols)</para>
    /// <para>If <paramref name="transitions"/> are non-deterministic, the result can contain duplicated symbols.</para>
    /// </remarks>
    /// <returns>The collection of symbols that can be used to transition directly from the given state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int[] AvailableSymbols(this ReadOnlySpan<Transition> transitions, int fromState)
    {
        transitions = transitions.Transitions(fromState);
        int[] symbols = new int[transitions.Length];
        for (int i = 0; i < transitions.Length; i++)
            symbols[i] = transitions[i].Symbol;
        return symbols;
    }

    /// <summary>
    /// Creates a minimum transition for the given state and symbol.
    /// </summary>
    /// <param name="fromState">The from state.</param>
    /// <param name="symbol">The symbol for the transition (default is <see cref="int.MinValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the minimum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Transition MinTrans(int fromState, int symbol = int.MinValue) => new(fromState, symbol, int.MinValue);

    /// <summary>
    /// Creates a maximum transition for the given state and symbol.
    /// </summary>
    /// <param name="fromState">The from state.</param>
    /// <param name="symbol">The symbol for the transition (default is <see cref="int.MaxValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the maximum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Transition MaxTrans(int fromState, int symbol = int.MaxValue) => new(fromState, symbol, int.MaxValue);
}
