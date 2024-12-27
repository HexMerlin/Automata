using System.Runtime.CompilerServices;

namespace Automata.Core.TransitionSets;
public static class SortedSetExtensions
{
    /// <summary>
    /// Enumerates all symbols that can be used to transition directly from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <remarks>If the underlying set is deterministic this method will return a set of symbols (no duplicated symbols)</remarks>
    /// <returns>The collection of symbols that can be used to transition directly from the given state.</returns>
    public static IEnumerable<int> AvailableSymbols(this SortedSet<SymbolicTransition> set, int fromState)
        => set.GetViewBetween(MinTrans(fromState), MaxTrans(fromState)).Select(t => t.Symbol);


    /// <summary>
    /// Returns the first transition from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The transition from the given state on the given symbol, or <see cref="SymbolicTransition.Invalid"/> if no such transition exists.</returns>
    public static SymbolicTransition Transition(this SortedSet<SymbolicTransition> set, int fromState, int symbol)
       => set.GetViewBetween(MinTrans(fromState, symbol), MaxTrans(fromState, symbol)).FirstOrDefault(SymbolicTransition.Invalid);

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The set of transitions from the given state.</returns>
    public static SortedSet<SymbolicTransition> Transitions(this SortedSet<SymbolicTransition> set, int fromState)
        => set.GetViewBetween(MinTrans(fromState), MaxTrans(fromState));

    /// <summary>
    /// Returns the transitions from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The transitions from the given state on the given symbol, or <see cref="SymbolicTransition.Invalid"/> if no such transition exists.</returns>
    public static SortedSet<SymbolicTransition> Transitions(this SortedSet<SymbolicTransition> set, int fromState, int symbol)
       => set.GetViewBetween(MinTrans(fromState, symbol), MaxTrans(fromState, symbol));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SymbolicTransition MinTrans(int fromState, int symbol = int.MinValue) => new(fromState, symbol, int.MinValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SymbolicTransition MaxTrans(int fromState, int symbol = int.MaxValue) => new(fromState, symbol, int.MaxValue);


}
