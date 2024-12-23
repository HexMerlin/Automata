namespace Automata.Core;

/// <summary>
/// Denotes a mutable set of transitions for fast lookup and retrieval.
/// </summary>
/// <remarks><see cref="TransitionSet{T}"/> maintains two ordered sets with the exact same set of transitions, 
/// but with different sort orders. One set is ordered where all from-states are ordered are contiguous, 
/// <para>and the other set is ordered where all to-states are contiguous.</para>
/// <para>That enables fast retrieval of all transitions from or to a certain state, respectively.</para>
/// </remarks>
/// <typeparam name="T">The type of transition.</typeparam>
/// <seealso cref="ITransition{T}"/>
/// <seealso cref="Transition"/>
/// <seealso cref="EpsilonTransition"/>
public class TransitionSet<T> where T : struct, ITransition<T>
{
    protected readonly SortedSet<T> orderDefault;
    protected readonly SortedSet<T> orderByToState;

    /// <summary>
    /// Initializes a new instance of <see cref="TransitionSet{T}"/>.
    /// </summary>
    /// <param name="initialTransitions">The initial elements to populate the sets.</param>
    public TransitionSet(IEnumerable<T> initialTransitions)
    {
        orderDefault = new SortedSet<T>(initialTransitions);
        orderByToState = new SortedSet<T>(initialTransitions, T.CompareByToState());
    }

    /// <summary>
    /// Returns the number of transitions in the set.
    /// </summary>
    public int Count => orderDefault.Count;

    /// <summary>
    /// Adds a transition to the set.
    /// </summary>
    /// <param name="transition">The element to add.</param>
    public void Add(T transition)
    {
        orderDefault.Add(transition);
        orderByToState.Add(transition);
    }

    /// <summary>
    /// Adds multiple transitions to the set.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void AddAll(IEnumerable<T> transitions)
    {
        foreach (T transition in transitions)
            Add(transition);
    }


    /// <summary>
    /// Removes a transition from the set.
    /// </summary>
    /// <param name="transition">The transition to remove.</param>
    public void Remove(T transition)
    {
        orderDefault.Remove(transition);
        orderByToState.Remove(transition);
    }

    /// <summary>
    /// Returns the minimum state referenced by any transition in the set.
    /// </summary>
    /// <remarks>The minimum state is either a state that occurs as a FromState or ToState. 
    /// <para>We make a fast check for the minimum FromState in the set ordered by FromState,</para>
    /// <para>and for the minimum ToState in the set ordered by ToState, and take the min of these two.</para>
    /// </remarks>
    public int MinState => Math.Min(orderDefault.Min.FromState, orderByToState.Min.ToState);

    /// <summary>
    /// Returns the maximum state referenced by any transition in the set.
    /// </summary>
    /// <remarks>The maximum state is either a state that occurs as a FromState or ToState. 
    /// <para>We make a fast check for the maximum FromState in the set ordered by FromState,</para>
    /// <para>and for the maximum ToState in the set ordered by ToState, and take the max of these two.</para>
    /// </remarks>
    public int MaxState => Math.Max(orderDefault.Max.FromState, orderByToState.Max.ToState);

    /// <returns>The set of transitions in default order.</returns>
    public IReadOnlySet<T> Transitions => orderDefault;
}

