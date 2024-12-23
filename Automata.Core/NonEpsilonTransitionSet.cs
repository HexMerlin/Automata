using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core;
public class NonEpsilonTransitionSet : TransitionSet<Transition>
{
    public NonEpsilonTransitionSet(IEnumerable<Transition> initialTransitions) : base(initialTransitions) {}


    /// <summary>
    /// Returns the states reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The states reachable from the given state on the given symbol.</returns>
    public IEnumerable<int> TraverseOnSymbol(int fromState, int symbol)
        => orderDefault.GetViewBetween(new Transition(fromState, symbol, int.MinValue), new Transition(fromState, symbol, int.MaxValue)).Select(t => t.ToState);

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given states.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    public IntSet GetAvailableSymbols(IEnumerable<int> fromStates)
        => new(fromStates.SelectMany(s => orderDefault.GetViewBetween(new Transition(s, int.MinValue, int.MinValue), new Transition(s, int.MaxValue, int.MaxValue)).Select(t => t.Symbol)));

}
