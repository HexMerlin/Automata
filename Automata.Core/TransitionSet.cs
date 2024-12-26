namespace Automata.Core;

/// <summary>
/// Represents a generic mutable set of transitions for fast lookup and retrieval.
/// </summary>
/// <remarks>This class maintains two ordered sets with the exact same set of transitions, 
/// but with different sort orders. One set is ordered so that all from-states are ordered are consecutive and increasing, 
/// <para>and the other set is ordered where all to-states are consecutive and increasing.</para>
/// <para>That enables fast retrieval of all transitions from or to a certain state, respectively.</para>
/// </remarks>
/// <typeparam name="T">The type of transition, either <see cref="SymbolicTransition"/> or <see cref="EpsilonTransition"/>.</typeparam>
/// <seealso cref="ITransition{T}"/>
/// <seealso cref="SymbolicTransition"/>
/// <seealso cref="EpsilonTransition"/>
public class TransitionSet<T> where T : struct, ITransition<T>
{
    protected readonly SortedSet<T> orderByFromState = new();
    protected readonly SortedSet<T> orderByToState = new(T.CompareByToState());

    /// <summary>
    /// Initializes a new empty set.
    /// </summary>
    public TransitionSet() { }

    /// <summary>
    /// Initializes a new set with an initial set of transitions.
    /// </summary>
    /// <param name="initialTransitions">Initial transitions to populate with.</param>
    public TransitionSet(IEnumerable<T> initialTransitions) => UnionWith(initialTransitions);

    /// <summary>
    /// Returns the number of transitions in the set.
    /// </summary>
    public int Count => orderByFromState.Count;

    /// <summary>
    /// Adds a transition to the set.
    /// </summary>
    /// <param name="transition">The element to add.</param>
    public void Add(T transition)
    {
        orderByFromState.Add(transition);
        orderByToState.Add(transition);
    }
    
    /// <summary>
    /// Removes a transition from the set.
    /// </summary>
    /// <param name="transition">The transition to remove.</param>
    public void Remove(T transition)
    {
        orderByFromState.Remove(transition);
        orderByToState.Remove(transition);
    }

    /// <summary>
    /// Adds all provided transitions that are currently not present in set.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void UnionWith(IEnumerable<T> transitions)
    {
        orderByFromState.UnionWith(transitions);
        orderByToState.UnionWith(transitions);
    }

    /// <summary>
    /// Removes all of the provided transitions that are currently in the set.
    /// </summary>
    /// <param name="transitions">The transitions to remove.</param>
    public void ExceptWith(IEnumerable<T> transitions)
    {
        orderByFromState.ExceptWith(transitions);
        orderByToState.ExceptWith(transitions);
    }

    /// <summary>
    /// Fast retrieval of the minimum state referenced by any transition in the set.
    /// </summary>
    /// <remarks>The minimum state is either a state that occurs as a FromState or ToState. 
    /// <para>Efficiently, we retrieve the minimum FromState from the set ordered by FromState,</para>
    /// <para>and retrieve the minimum ToState in the set ordered by ToState, and then take the min of these two.</para>
    /// </remarks>
    /// <returns>The minimum state referenced by any transition in the set, or <see cref="Constants.InvalidState"/> if the set was empty</returns>
    public int MinState => Count > 0 ? Math.Min(orderByFromState.Min.FromState, orderByToState.Min.ToState) : Constants.InvalidState;

    /// <summary>
    /// Fast retrieval of the maximum state referenced by any transition in the set.
    /// </summary>
    /// <remarks>The maximum state is either a state that occurs as a FromState or ToState. 
    /// <para>Efficiently, we retrieve the maximum FromState from the set ordered by FromState,</para>
    /// <para>and retrieve the maximum ToState in the set ordered by ToState, and then take the max of these two.</para>
    /// </remarks>
    /// <returns>The maximum state referenced by any transition in the set, or <see cref="Constants.InvalidState"/> if the set was empty</returns>
    public int MaxState => Count > 0 ? Math.Max(orderByFromState.Max.FromState, orderByToState.Max.ToState) : Constants.InvalidState;

    /// <returns>The set of transitions in the default order.</returns>
    public IReadOnlySet<T> Transitions => orderByFromState;
}

