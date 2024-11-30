using System.Linq;

namespace Automata;

public class DFA : IFsa
{
    #region Instance
    public Alphabet Alphabet { get; }
   
    private readonly SortedDictionary<long, int> transitions;

    public int InitialState { get; private set; } = -1; 

    private readonly SortedSet<int> finalStates;

    #endregion

    public bool IsInitial(int state) => state == InitialState;
    public bool IsFinal(int state) => finalStates.Contains(state);  

    public IReadOnlySet<int> FinalStates => finalStates;
 
    public DFA() : this(new Alphabet())
    {
        
    }

    public DFA(Alphabet alphabet, IEnumerable<Transition> transitions, int initialState, IEnumerable<int> finalStates)
    {
        this.Alphabet = alphabet;
        this.transitions = new();
        SetInitial(initialState);
        this.finalStates = new(finalStates);
        foreach (Transition transition in transitions)
        {
            AddTransition(transition);
        }
    }   
  

    public DFA(Alphabet alphabet)
    {
        this.Alphabet = alphabet;
        this.transitions = [];
        this.finalStates = [];
    }

    public void SetInitial(int state) => InitialState = state;
    public void SetFinal(int state, bool final = true)
    {
        if (final)
            finalStates.Add(state);
        else
            finalStates.Remove(state);
    }

    public IEnumerable<Transition> Transitions 
        => transitions.Select(kvp => new Transition(Split(kvp.Key).Item1, Split(kvp.Key).Item2, kvp.Value));

    public bool EpsilonFree => true;

    public IEnumerable<EpsilonTransition> EpsilonTransitions => Enumerable.Empty<EpsilonTransition>();

    public void AddTransition(Transition transition)
    {
        long key = Merge(transition.FromState, transition.Symbol);
        transitions.Add(key, transition.ToState);
    }

    public DFA Minimized()
    {
        DFA reverseDfa = Reversed().ToDFA();
        return reverseDfa.Reversed().ToDFA();
    }

    public NFA Reversed() => new(this, applyReverseOperation: true);

    public NFA ToNFA() => new(this);

   
    private static long Merge(int a, int b) => (long)a << 32 | (uint)b;

    private static (int, int) Split(long value) => ((int)(value >> 32), (int)value);

   
}
