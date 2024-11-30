using System.Collections.Immutable;
using System.Linq;

namespace Automata;

public interface IFsa
{
    Alphabet Alphabet { get; }
    bool EpsilonFree { get; }

    bool IsInitial(int state);
    bool IsFinal(int state);

    IEnumerable<Transition> Transitions { get; }

    IEnumerable<EpsilonTransition> EpsilonTransitions { get; }
}

public class NFA : IFsa
{
    #region Instance
    public Alphabet Alphabet { get; }

    private readonly SortedSet<Transition> nonEpsilonTransitions;
    private readonly SortedSet<Transition> nonEpsilonTransitions_ByToState;

    private readonly SortedSet<EpsilonTransition> epsilonTransitions;
    private readonly SortedSet<EpsilonTransition> epsilonTransitions_ByToState;

    private readonly SortedSet<int> initialStates;
    private readonly SortedSet<int> finalStates;

    #endregion

    public NFA() : this(new Alphabet())
    {}

    public NFA(Alphabet alphabet) : this(alphabet, Enumerable.Empty<Transition>(), Enumerable.Empty<EpsilonTransition>(), Enumerable.Empty<int>(), Enumerable.Empty<int>())
    {}

    private NFA(Alphabet alphabet, IEnumerable<Transition> nonEpsilonTransitions, IEnumerable<EpsilonTransition> epsilonTransitions, IEnumerable<int> initialStates, IEnumerable<int> finalStates)
    {
        this.Alphabet = alphabet;
        this.nonEpsilonTransitions = new SortedSet<Transition>(nonEpsilonTransitions);
        this.nonEpsilonTransitions_ByToState = new SortedSet<Transition>(nonEpsilonTransitions, Transition.CompareByToState());
        this.epsilonTransitions = new SortedSet<EpsilonTransition>(epsilonTransitions); ;
        this.epsilonTransitions_ByToState = new SortedSet<EpsilonTransition>(epsilonTransitions, EpsilonTransition.CompareByToState());
        this.initialStates = new SortedSet<int>(initialStates);
        this.finalStates = new SortedSet<int>(finalStates);
    }


    internal NFA(DFA dfa, bool applyReverseOperation = false) : this(dfa.Alphabet)
    {
        this.Alphabet = dfa.Alphabet;
       
        if (applyReverseOperation)
        {
            AddAll(dfa.Transitions.Select(t => t.Reverse()));
            SetInitial(dfa.FinalStates);
            SetFinal(dfa.InitialState);
        }
        else
        {
            AddAll(dfa.Transitions);
            SetInitial(dfa.InitialState);
            SetFinal(dfa.FinalStates);
        }
       
    }

    public void Add(Transition transition)
    {
        nonEpsilonTransitions.Add(transition);
        nonEpsilonTransitions_ByToState.Add(transition);
    }
    public void Add(EpsilonTransition transition)
    {
        epsilonTransitions.Add(transition);
        epsilonTransitions_ByToState.Add(transition);
    }

    public void AddAll(IEnumerable<Transition> transitions)
    {
        foreach (var transition in transitions)
        {
            nonEpsilonTransitions.Add(transition);
            nonEpsilonTransitions_ByToState.Add(transition);
        }
    }

    public void AddAll(IEnumerable<EpsilonTransition> transitions)
    {
        foreach (var transition in transitions)
        {
            epsilonTransitions.Add(transition);
            epsilonTransitions_ByToState.Add(transition);
        }
    }
    public void Add(IEnumerable<string> sequence)
    {
        int maxState = FindMaxState();

        if (initialStates.Count == 0)
            SetInitial(++maxState);

        int fromState = initialStates.Min;

        foreach (string symbol in sequence)
        {
            Transition transition = new Transition(fromState, Alphabet.GetOrAdd(symbol), ++maxState);
            nonEpsilonTransitions.Add(transition);
            fromState = maxState;
        }
        finalStates.Add(maxState);
    }

    public bool IsInitial(int state) => initialStates.Contains(state);
    public bool IsFinal(int state) => finalStates.Contains(state);

    public void SetInitial(int state, bool initial = true)
    {
        if (initial) 
            initialStates.Add(state);
        else 
            initialStates.Remove(state);
    }

    public void SetFinal(int state, bool final = true)
    {
        if (final) 
            finalStates.Add(state);
        else 
            finalStates.Remove(state);
    }

    public void SetInitial(IEnumerable<int> states, bool initial = true)
    {
        if (initial) 
            initialStates.UnionWith(states);
        else 
            initialStates.ExceptWith(states);
    }

    public void SetFinal(IEnumerable<int> states, bool final = true)
    {
        if (final) 
            finalStates.UnionWith(states);
        else 
            finalStates.ExceptWith(states);
    }

    public IReadOnlySet<Transition> Transitions => nonEpsilonTransitions;

    public IReadOnlySet<EpsilonTransition> EpsilonTransitions => epsilonTransitions;

    public IReadOnlySet<int> InitialStates => initialStates;
    public IReadOnlySet<int> FinalStates => finalStates;

    public bool EpsilonFree => EpsilonTransitions.Count == 0;

    IEnumerable<Transition> IFsa.Transitions => Transitions;

    IEnumerable<EpsilonTransition> IFsa.EpsilonTransitions => EpsilonTransitions;


    public int FindMaxState()
    {
        static int Max(int a, int b) => a > b ? a : b;

        int maxTransition = Max(nonEpsilonTransitions.Max.FromState, nonEpsilonTransitions_ByToState.Max.ToState);
        int maxEpsilonTransition = Max(epsilonTransitions.Max.FromState, epsilonTransitions_ByToState.Max.ToState);
        int maxInitial = initialStates.Count > 0 ? initialStates.Max : -1;
        int maxFinal = finalStates.Count > 0 ? finalStates.Max : -1;
        return Max(Max(maxTransition, maxEpsilonTransition), Max(maxInitial, maxFinal));
      
    }



    /// <summary>
    /// Returns the states reachable from the given state on the given symbol (epsilon transitions are ignored).
    /// </summary>
    private IEnumerable<int> TraverseOnSymbol(int fromState, int symbol) 
        => nonEpsilonTransitions.GetViewBetween(new Transition(fromState, symbol, int.MinValue), new Transition(fromState, symbol, int.MaxValue)).Select(t => t.ToState);

 
    /// <summary>
    /// Returns the states reachable from the given state on a single epsilon transition.
    /// If the input state has an epsilon loop on it self, it will be included in the result.
    /// </summary>
    private IEnumerable<int> TraverseOnEpsilonOneStep(int fromState) 
        => epsilonTransitions.GetViewBetween(new EpsilonTransition(fromState, int.MinValue), new EpsilonTransition(fromState, int.MaxValue)).Select(t => t.ToState);


    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given states.
    /// Epsilon transitions are ignored.
    /// </summary>
    private IntSet GetAvailableSymbols(IEnumerable<int> fromStates) 
        => new IntSet(fromStates.SelectMany(s => nonEpsilonTransitions.GetViewBetween(new Transition(s, int.MinValue, int.MinValue), new Transition(s, int.MaxValue, int.MaxValue)).Select(t => t.Symbol)));
    
    private void ExtendWithEpsilonClosureInplace(HashSet<int> fromStates)
    {
        var queue = new Queue<int>(fromStates);

        while (queue.Count != 0)
        {
            int state = queue.Dequeue();
            var newStates = TraverseOnEpsilonOneStep(state);
            foreach (var newState in newStates)
            {
                if (fromStates.Add(newState))
                    queue.Enqueue(newState);
            }
        }
    }

    private IntSet GetToStates(IEnumerable<int> fromStates, int symbol)
    {
        HashSet<int> intermediateStates = fromStates.ToHashSet();
        ExtendWithEpsilonClosureInplace(intermediateStates);
        HashSet<int> toStates = intermediateStates.SelectMany(s => TraverseOnSymbol(s, symbol)).ToHashSet();
        ExtendWithEpsilonClosureInplace(toStates);
        return new IntSet(toStates);
    }

    public DFA ToDFA() 
    {
        List<Transition> dfaTransitions = new();
        HashSet<int> dfaFinalStates = new();

        int dfaMaxState = -1;
        Dictionary<IntSet, int> stateSetToDfaState = [];
        Queue<IntSet> queue = new();
              
        HashSet<int> initialStates = new HashSet<int>(this.initialStates);
        ExtendWithEpsilonClosureInplace(initialStates);
        int dfaInitialState = GetOrAddState(new IntSet(initialStates)); //adds inital state to dfa
        

        while (queue.Count > 0)
        {
            IntSet fromState = queue.Dequeue();
            IntSet symbols = GetAvailableSymbols(fromState.Members);
            int dfaFromState = GetOrAddState(fromState);
            
            foreach (int symbol in symbols.Members)
            {
                IntSet toState = GetToStates(fromState.Members, symbol);
                int dfaToState = GetOrAddState(toState);
                dfaTransitions.Add(new Transition(dfaFromState, symbol, dfaToState));
            }
        }
        return new DFA(this.Alphabet, dfaTransitions, dfaInitialState, dfaFinalStates);

        int GetOrAddState(IntSet state)
        {
            if (!stateSetToDfaState.TryGetValue(state, out int dfaState))
            {
                dfaState = ++dfaMaxState; //create a new state in DFA
                stateSetToDfaState[state] = dfaState;
                queue.Enqueue(state);
            }
            if (state.Members.Overlaps(finalStates))
                dfaFinalStates.Add(dfaState);
            return dfaState;
        }
    }

    public DFA ToMinimizedDFA() => ToDFA().Minimized();


}
