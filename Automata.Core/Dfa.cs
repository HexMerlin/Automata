using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Automata.Core;

/// <summary>
/// Deterministic finite automaton (DFA).
/// </summary>
/// <remarks>
/// A DFA is a finite state machine that accepts or rejects finite sequences of symbols.
/// <para>· A DFA cannot contain epsilon transitions</para> 
/// <para>· Any mutation of a DFA is guaranteed to preserve its deterministic property.</para>
/// <para>· Mutation can make certain states unreachable (contain <c>Dead states</c>).</para>
/// </remarks>
public class Dfa : FsaDet
{
    #region Data

    private int initialState = Constants.InvalidState;

    private int maxState = Constants.InvalidState;

    private readonly HashSet<int> finalStates = new();

    private readonly SortedSet<Transition> transitions = new();

    #endregion Data

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the DFA.</param>
    public Dfa(Alphabet alphabet) : base(alphabet) { }

    /// <summary>
    /// Initializes a new instance of a <see cref="Dfa"/> class from an existing <see cref="Mfa"/>.
    /// <para>
    /// The resulting DFA is not only isomorphic to the original MFA but also graphically identical, 
    /// preserving identical state and transition IDs, and <see cref="MaxState"/>.
    /// </para>
    /// </summary>
    /// <param name="mfa">The MFA to convert into a DFA.</param>
    public Dfa(Mfa mfa) : this(mfa.Alphabet, mfa.InitialState, mfa.FinalStates, mfa.Transitions())
        => this.maxState = mfa.MaxState;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet, transitions, initial state, and final states.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the DFA.</param>
    /// <param name="initialState">Initial state of the DFA.</param>
    /// <param name="finalStates">Final states of the DFA.</param>
    /// <param name="transitions">Transitions of the DFA.</param>
    internal Dfa(Alphabet alphabet, int initialState, IEnumerable<int> finalStates, IEnumerable<Transition> transitions) : this(alphabet)
    {
        SetInitial(initialState);
        SetFinal(finalStates);
        UnionWith(transitions);
    }

    /// <summary>
    /// Private constructor of the <see cref="Dfa"/> class with the specified alphabet, initial state, maximum state, final states, and transitions.
    /// <para>
    /// Unsafe constructor that does not validate any arguments.
    /// </para>
    /// </summary>
    /// <param name="alphabet">Alphabet used by the DFA.</param>
    /// <param name="initialState">Initial state of the DFA.</param>
    /// <param name="maxState">Maximum state number in the DFA.</param>
    /// <param name="finalStates">Set of final states in the DFA.</param>
    /// <param name="transitions">Set of transitions in the DFA.</param>
    private Dfa(Alphabet alphabet, int initialState, int maxState, HashSet<int> finalStates, SortedSet<Transition> transitions) : base(alphabet)
    {
        this.initialState = initialState;
        this.maxState = maxState;
        this.finalStates = finalStates;
        this.transitions = transitions;
    }

    /// <summary>
    /// Creates a <see cref="Dfa"/> that represents the empty language (∅), with a specified alphabet.
    /// The DFA has zero states and zero transitions.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the MFA.</param>
    public static Dfa CreateEmpty(Alphabet alphabet) => new(alphabet);

    /// <summary>
    /// Creates a <see cref="Mfa"/> that represents the language <c>{ϵ}</c>, with a specified alphabet.
    /// The DFA has a single initial, final state and zero transitions.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the DFA.</param>
    public static Dfa CreateEpsilonAccepting(Alphabet alphabet) => new(alphabet, 0, 0, [0], []);


    #endregion Constructors

    #region Accessors

    /// <summary>
    /// Initial state of the DFA.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="Constants.InvalidState"/> if the DFA has no initial state.
    /// </remarks>
    public override int InitialState => initialState;

    /// <summary>
    /// Range encompassing all states of the automaton.
    /// </summary>
    /// <remarks>
    /// All states lies inside this range; but contiguity of states is not guaranteed.
    /// <para>Exclusive <see cref="Range.End"/> equals the minimal unused state ID.</para>
    /// <para>Non-empty automata: <c>StateRange = [0, <see cref="MaxState"/> + 1)</c>.</para>
    /// <para>Empty automata: <c>StateRange = [−1, −1)</c>).</para>
    /// </remarks>
    public override Range StateRange => initialState != Constants.InvalidState
        ? new Range(0, maxState + 1)
        : new Range(Constants.InvalidState, Constants.InvalidState);

    /// <summary>
    /// Upper limit for the maximum state number in the DFA. 
    /// <para>A value (<see cref="MaxState"/> + 1) is guaranteed to be an unused state number.</para>
    /// </summary>
    /// <remarks>
    /// This value represents an upper limit for state numbers in the DFA.
    /// The actual maximum state number may be lower, as removed states are not tracked for performance reasons.
    /// </remarks>
    public override int MaxState => maxState;

    /// <summary>
    /// Number of transitions in the automaton.
    /// </summary>
    public override int TransitionCount => transitions.Count;

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the specified state is a final state.
    /// </returns>
    public override bool IsFinal(int state) => finalStates.Contains(state);

    /// <summary>
    /// Final states of the DFA.
    /// </summary>
    public override IReadOnlyCollection<int> FinalStates => finalStates;

    /// <summary>
    /// Returns a view of the specified state.
    /// </summary>
    /// <param name="fromState">The state origin.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fromState"/> is negative.</exception>
    /// <returns>A <see cref="StateView"/> for the given state.</returns>
    public override StateView State(int fromState)
        => new(fromState, Transitions(fromState).ToArray());

    ///<inheritdoc/>
    public override int Transition(int fromState, int symbol)
        => transitions.GetViewBetween(
            Core.Transition.MinTrans(fromState.ShouldNotBeNegative(), symbol.ShouldNotBeNegative()),
            Core.Transition.MaxTrans(fromState, symbol)
        ).FirstOrDefault(Core.Transition.Invalid).ToState;

    /// <summary>
    /// Gets the transitions of the DFA.
    /// </summary>
    /// <returns>An collection of transitions.</returns>
    public override IReadOnlyCollection<Transition> Transitions() => transitions;

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <returns>Set of transitions from the given state.</returns>
    private SortedSet<Transition> Transitions(int fromState)
        => transitions.GetViewBetween(
            Core.Transition.MinTrans(fromState.ShouldNotBeNegative()),
            Core.Transition.MaxTrans(fromState)
        );

    #endregion Accessors

    #region Mutators

    /// <summary>
    /// Sets the initial state of the DFA, updating the maximum state number if necessary.
    /// </summary>
    /// <param name="state">State to set as the initial state.</param>
    
    public void SetInitial(int state) => initialState = VerifyAndUpdateMaxState(state);

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">State to set or remove as a final state.</param>
    /// <param name="final">
    /// If <see langword="true"/>, the state is added to the final states; otherwise, it is removed.
    /// </param>
    public void SetFinal(int state, bool final = true) => IncludeIf(final, state, finalStates);

    /// <summary>
    /// Sets the specified states as final states or removes them from the final states.
    /// </summary>
    /// <param name="states">States to set or remove as final states.</param>
    /// <param name="final">
    /// If <see langword="true"/>, the states are added to the final states; otherwise, they are removed.
    /// </param>
    public void SetFinal(IEnumerable<int> states, bool final = true) => IncludeIf(final, states, finalStates);

    /// <summary>
    /// Adds a transition to the DFA, ensuring it remains deterministic.
    /// </summary>
    /// <remarks>
    /// If adding the transition would introduce nondeterminism (i.e., a transition with the same from-state and symbol already exists), the new transition will not be added.
    /// </remarks>
    /// <param name="transition">Transition to add.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the transition was added.
    /// </returns>
    public bool Add(Transition transition)
    {
        if (TryTransition(transition.FromState, transition.Symbol, out _))
            return false; // Cannot add; would introduce nondeterminism
        maxState = Math.Max(MaxState, Math.Max(transition.FromState, transition.ToState));
        return transitions.Add(transition);
    }

    /// <summary>
    /// Adds all provided transitions that are currently not present in the set.
    /// </summary>
    /// <param name="transitions">Transitions to add.</param>
    public void UnionWith(IEnumerable<Transition> transitions)
    {
        foreach (Transition transition in transitions)
            Add(transition);
    }


    /// <summary>
    /// Trim the DFA in-place to only include states and transitions that are both accessible and co-accessible.
    /// Thus, all states and transitions that are not on a path from the initial state to a final state are removed.
    /// </summary>
    /// <returns>
    /// A tuple containing post-trim information:
    /// <list type="bullet">
    /// <item><description>Remaining trimmed states.</description></item>
    /// <item><description>Remaining transitions in reversed form. This can be used for back-tracking in the DFA.</description></item>
    /// </list>
    /// </returns>
    public (HashSet<int> allStates, SortedSet<Transition> reverseTransitions) Trim()
    {
        if (!HasInitialState)
        {
            ClearAll();
            return ([], []);
        }
        HashSet<int> reachable = new() { InitialState };
        Queue<int> queue = new();
        queue.Enqueue(InitialState);
        while (queue.Count > 0)
        {
            int fromState = queue.Dequeue();
            foreach (Transition t in Transitions(fromState))
                if (reachable.Add(t.ToState))
                    queue.Enqueue(t.ToState);
        }

        SortedSet<Transition> reverseTrans = new(transitions.Select(t => new Transition(t.ToState, t.Symbol, t.FromState)));
        HashSet<int> reachingFinal = new(FinalStates);

        foreach (var f in FinalStates)
            queue.Enqueue(f);

        while (queue.Count > 0)
        {
            int state = queue.Dequeue();
            IEnumerable<int> fromStates = reverseTrans.GetViewBetween(Core.Transition.MinTrans(state), Core.Transition.MaxTrans(state))
                    .Select(t => t.ToState); //'.ToState' means actually getting fromStates since we are operating on reverseTrans

            foreach (int fromState in fromStates)
                if (reachingFinal.Add(fromState))
                    queue.Enqueue(fromState);
        }

        reachable.IntersectWith(reachingFinal); // keep only co-accessible states
        initialState = reachable.Contains(InitialState) ? InitialState : Constants.InvalidState;
        transitions.RemoveWhere(t => !reachable.Contains(t.FromState) || !reachable.Contains(t.ToState));
        reverseTrans.RemoveWhere(t => !reachable.Contains(t.FromState) || !reachable.Contains(t.ToState));
        finalStates.IntersectWith(reachable);
        return (reachable, reverseTrans);
    }

    /// <summary>
    /// Returns a new minimal DFA.
    /// </summary>
    /// <remarks>Switchable in code between Brzozowski's and Hopcroft's algorithms</remarks>
    /// <returns>A new minimal DFA.</returns>
    /// <seealso cref="Minimal_Hopcroft"></seealso>
    /// <seealso cref="Minimal_Brzozowski"/>
    internal Dfa Minimal()
    {
        return Minimal_Brzozowski();
        //return Minimal_Hopcroft(); 
    }

    /// <summary>
    /// Returns a new minimal DFA using Brzozowski's algorithm.
    /// </summary>
    /// <returns>A new minimal DFA.</returns>
    /// <seealso cref="Minimal_Hopcroft"></seealso>
    internal Dfa Minimal_Brzozowski()
    {
        Dfa reversed = Ops.Deterministic(Ops.Reversal(this));
        Dfa min = Ops.Deterministic(Ops.Reversal(reversed));
        if (min.TransitionCount == 0 && !AcceptsEpsilon)
            min.ClearAll();
        return min;
    }

    /// <summary>
    /// Returns a new minimal DFA using Hopcroft's algorithm.
    /// Current instance will be trimmed (to only include states and transitions that are both accessible and co-accessible).
    /// </summary>
    /// <returns>A new minimal DFA.</returns>
    /// <seealso cref="Trim"></seealso>
    /// <seealso cref="Minimal_Brzozowski"></seealso>"/>
    internal Dfa Minimal_Hopcroft()
    {
        // Return simple if DFA has no initial state or no transitions or no final states
        if (! HasInitialState || TransitionCount == 0 || FinalStates.Count == 0) 
            return AcceptsEpsilon ? Dfa.CreateEpsilonAccepting(Alphabet) : Dfa.CreateEmpty(Alphabet);

        // Step 1: Trim the DFA in-place and get trim information
        (HashSet<int> allStates, SortedSet<Transition> reverseTransitions) = Trim();

        //Add the rest of code here for the full implementation of Hopcroft's algorithm

        throw new NotImplementedException();
    }

    /// <summary>
    /// Clears all states and transitions. The DFA will be equivalent to the empty language (∅).
    /// </summary>
    /// <remarks>
    /// The alphabet is not cleared.
    /// </remarks>
    public void ClearAll()
    {
        initialState = Constants.InvalidState;
        maxState = Constants.InvalidState;
        finalStates.Clear();
        transitions.Clear();
    }

    /// <summary>
    /// Conditionally includes or excludes a state in a set based on the provided condition.
    /// </summary>
    /// <param name="condition">If <see langword="true"/>, the state is added; otherwise, it is removed.</param>
    /// <param name="state">State to include or exclude.</param>
    /// <param name="set">Set to modify.</param>
    private void IncludeIf(bool condition, int state, HashSet<int> set)
    {
        if (condition)
            set.Add(VerifyAndUpdateMaxState(state));
        else
            set.Remove(state);
    }

    /// <summary>
    /// Conditionally includes or excludes a collection of states in a set based on the provided condition.
    /// </summary>
    /// <param name="condition">If <see langword="true"/>, the states are added; otherwise, they are removed.</param>
    /// <param name="states">States to include or exclude.</param>
    /// <param name="set">Set to modify.</param>
    private void IncludeIf(bool condition, IEnumerable<int> states, HashSet<int> set)
    {
        if (condition)
        {
            foreach (int state in states)
                set.Add(VerifyAndUpdateMaxState(state));
        }
        else
        {
            set.ExceptWith(states);
        }
    }

    /// <summary>
    /// Asserts that the state is non-negative and updates the maximum state number if the provided state is greater.
    /// </summary>
    /// <param name="state">State to compare and potentially update.</param>
    /// <returns>The input state.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="state"/> is negative.</exception>
    /// <remarks>
    /// This method ensures that the state is valid (non-negative) and updates the <see cref="MaxState"/> if the provided state exceeds the current maximum state.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int VerifyAndUpdateMaxState(int state)
    {
        state.ShouldNotBeNegative();
        maxState = Math.Max(MaxState, state);
        return state;
    }

    #endregion Mutators

    #region Misc Accessors

    /// <summary>
    /// Maps states to their canonical state numbers in a breadth-first order.
    /// </summary>
    /// <remarks>
    /// This method traverses the DFA in a well-defined breadth-first order and assigns a unique canonical state number to each state.
    /// A BFS traversal starts in the <see cref="InitialState"/> and processes transitions breadth-first, then in lexicographical order of their symbols, ensuring a consistent and well-defined ordering of states.
    /// The initial state is always assigned the canonical state number <c>0</c>.
    /// The resulting map can be used to remap states to their canonical numbers.
    /// </remarks>
    /// <returns>
    /// A dictionary mapping each state to its canonical state number.
    /// </returns>
    internal Dictionary<int, int> StatesToCanonicalStatesMap()
    {
        if (! HasInitialState)
            return new Dictionary<int, int>();

        StringComparer comparer = StringComparer.Ordinal; //Default canonical string comparer

        Dictionary<int, int> stateToCanonicalMap = new();
        Queue<int> queue = new();

        // Enqueue the initial state and add it to the map
        queue.Enqueue(InitialState);
        stateToCanonicalMap.Add(InitialState, stateToCanonicalMap.Count);

        while (queue.Count > 0)
        {
            int state = queue.Dequeue();

            foreach (int toState in Transitions(state)
                .OrderBy(t => Alphabet[t.Symbol], comparer)
                .Select(t => t.ToState))
            {
                if (!stateToCanonicalMap.ContainsKey(toState))
                {
                    queue.Enqueue(toState);
                    stateToCanonicalMap.Add(toState, stateToCanonicalMap.Count);
                }
            }
        }

        return stateToCanonicalMap;
    }

    /// <summary>
    /// Returns a canonical string representation of the DFA's data.
    /// Used by unit tests and for debugging. 
    /// </summary>
    public override string ToCanonicalString()
    {
        StringBuilder sb = new();
        sb.Append($"I: {InitialState}");
        sb.Append($", F#={finalStates.Count}");

        if (finalStates.Count > 0)
        {
            sb.Append($": [{string.Join(", ", finalStates)}]");
        }
        sb.Append($", T#={transitions.Count}");
        if (transitions.Count > 0)
        {
            sb.Append(": [");
            for (int i = 0; i < transitions.Count; i++)
            {
                Transition t = transitions.ElementAt(i);
                sb.Append($"{t.FromState}->{t.ToState} {Alphabet[t.Symbol]}");
                if (i < transitions.Count - 1) sb.Append(", ");
            }
            sb.Append(']');
        }
        return sb.ToString();
    }

    #endregion Misc Accessors

}
