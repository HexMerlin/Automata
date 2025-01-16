//namespace Automata.Core;

///// <summary>
///// A fast readonly view of a <c>state</c> in an automaton, providing access to transitions from this specific state.
///// </summary>
///// <remarks>
///// Implementations:
///// <para><see cref="StateViewMfa"/> (with <see cref="Mfa"/>): uses a light-weight ReadonlySpan that points to contiguous memory.</para>
///// <para><see cref="StateViewDfa"/> (with <see cref="Dfa"/>): uses a light-weight record that points to a Red-Black-Tree node.</para>
///// </remarks>
//public interface IStateView
//{
//    /// <summary>
//    /// State from which the transitions originate.
//    /// </summary>
//    int State { get; }

//    /// <summary>
//    /// Returns the state reachable from the current state on the specified symbol.
//    /// </summary>
//    /// <param name="symbol">Symbol for the transition.</param>
//    /// <returns>
//    /// The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.
//    /// </returns>
//    /// <seealso cref="TryTransition(int, out int)"/>
//    int Transition(int symbol);

//    /// <summary>
//    /// Tries to get the state reachable from the current state on the specified symbol.
//    /// </summary>
//    /// <param name="symbol">Symbol for the transition.</param>
//    /// <param name="toState">The reachable state, or <see cref="Constants.InvalidState"/> if the method returns false.</param>
//    /// <returns><see langword="true"/> <c>iff</c> a reachable state exists.</returns>
//    /// <seealso cref="Transition(int)"/>
//    bool TryTransition(int symbol, out int toState);


//}