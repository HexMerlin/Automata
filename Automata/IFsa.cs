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
