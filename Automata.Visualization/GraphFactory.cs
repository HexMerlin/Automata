using Automata.Core;
using Microsoft.Msagl.Drawing;

namespace Automata.Visualization;

///<summary>
/// Static class for creating displayable graphs from finite state automata.
///</summary>
public static class GraphFactory
{
    /// <summary>
    /// Creates a displayable graph from a collection of sequences.
    /// </summary>
    /// <param name="sequences">Collection of sequences to create the graph from.</param>
    /// <param name="minimize">Indicates whether to minimize the DFA.</param>
    /// <returns>A graph representing the finite state automaton.</returns>
    public static Graph CreateGraph(this IEnumerable<IEnumerable<string>> sequences, bool minimize = true)
    {
        Fsa fsa = new Nfa(sequences); // create an NFA from the sequences

        fsa = fsa.AsDeterministic(); // determinize to DFA
        if (minimize)
            fsa = fsa.AsMfa(); // minimize the DFA

        return CreateGraph(fsa);
    }

    /// <summary>
    /// Creates a displayable graph from a finite state automaton (FSA).
    /// </summary>
    /// <param name="fsa">Finite state automaton to represent as a graph.</param>
    /// <param name="displayStateIDs">Indicates whether to display state IDs.</param>
    /// <param name="layerDirection">Layout direction of the graph (default: left-to-right).</param>
    /// <param name="directed">Indicates whether the graph is directed (default: true).</param>
    /// <returns>A graph representing the finite state automaton.</returns>
    public static Graph CreateGraph(this Fsa fsa, bool displayStateIDs = false, LayerDirection layerDirection = LayerDirection.LR, bool directed = true)
    {
        Graph graph = new Graph();
        graph.Attr.LayerDirection = layerDirection;
        Alphabet alphabet = fsa.Alphabet;

        graph.Directed = directed;

        void AddEdge(int fromState, string label, int toState)
        {
            Edge edge = graph.AddEdge(fromState.ToString(), label, toState.ToString());
            edge.Attr.ArrowheadAtTarget = directed ? ArrowStyle.Normal : ArrowStyle.None;
        }

        foreach (Transition transition in fsa.Transitions())
            AddEdge(transition.FromState, alphabet[transition.Symbol], transition.ToState);

        if (fsa is Nfa nfa)
        {
            foreach (EpsilonTransition transition in nfa.EpsilonTransitions())
                AddEdge(transition.FromState, EpsilonTransition.Epsilon, transition.ToState);
            foreach (var s in nfa.InitialStates)  //ensure any disconnected initial states are included
                _ = graph.AddNode(s.ToString());
        }
        else if (fsa is Dfa dfa)
        {
            if (dfa.InitialState != Constants.InvalidState) //ensure the initial states is included, if disconnected
                graph.AddNode(dfa.InitialState.ToString());
        }

        foreach (var s in fsa.FinalStates)  //ensure any disconnected final states are included
           _ = graph.AddNode(s.ToString());

        graph.ConfigNodes(fsa, displayStateIDs); // configure the appearance of the states

        return graph;
    }

    /// <summary>
    /// Configures the appearance of nodes in the graph.
    /// </summary>
    /// <param name="graph">Graph to configure.</param>
    /// <param name="fsa">Finite state automaton represented by the graph.</param>
    /// <param name="displayStateIDs">Indicates whether to display state IDs.</param>
    private static void ConfigNodes(this Graph graph, Fsa fsa, bool displayStateIDs)
    {
        foreach (Node? node in graph.Nodes)
        {
            int state = int.Parse(node.Id);
            bool isInitial = fsa.IsInitial(state);
            bool isFinal = fsa.IsFinal(state);

            node.Attr.Shape = Shape.Circle;
            node.Label.Text = displayStateIDs ? state.ToString() : "";

            //note: a state can be both initial and final
            //a state that is both will be double-circled and colored
            if (isFinal)
            {
                node.Attr.Shape = Shape.DoubleCircle;
                node.Attr.LineWidth = 1;
            }
            if (isInitial)
            {
                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue; 
            }
           
        }
    }
}
