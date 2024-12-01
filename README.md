# Automata - a lightweight finite-state automata library


Automata
========
The core Automata library provides common automata functionality, such as:
  - creating NFAs (non-deterministic automata)
  - determinization to DFA (deterministic automata)
  - minimization of automata - creating automata with minimum number of states

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
 
Automata.Visualization (adds visualization functionality)
=========================================================
The Automata.Visualization library provides functionality for visualizing automata using MSAGL (Microsoft Automatic Graph Library).
The core Automata library is included.
If you want to visualize automata, use the Automata.Visualization library. If you only need the core automata functionality, use the Automata library which is more light-weight, 
and does not have any dependencies on MSAGL.

Example code in C#:

// Creates a console window with support for colored text output and graph visualization
ConsoleWindow consoleWindow = ConsoleWindow.Create(); 

// Write some colored text output to the console window
consoleWindow.WriteLine("Creating graph...", System.Drawing.Color.Blue); 

//create some random sequences of strings (using number-strings in this example, but symbols can be any strings)
var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString())); //create some random sequences

// Create a displayable graph object directly from the sequences
Graph graph = GraphFactory.CreateGraph(sequences, minimize: true); 

// Open a new non-modal window and show the graph
consoleWindow.ShowGraph(graph); 

// Write some more colored text output to the console window
consoleWindow.WriteLine("Graph created.", System.Drawing.Color.Green); 