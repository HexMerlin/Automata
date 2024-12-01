# Automata - a lightweight finite-state automata library

Automata provides common automata functionality, such as:
  creating NFAs (non-deterministic automata)
  determinization to DFA (deterministic automata)
  minimization of automata - creating automata with minimum number of states

Example code in C#:

//create some random sequences of strings (using number-strings in this example, but symbols can be any strings)

var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString())); 

//create an empty NFA

NFA nfa = new NFA();

//add all sequences to the NFA

nfa.AddAll(sequences);

//determinize the NFA to a DFA

DFA dfa = nfa.ToDFA();

//minimize the DFA

DFA minDFA = dfa.Minimized();
 
