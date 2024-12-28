using Automata.Core;
using Automata.Core.Alphabets;
using Microsoft.Msagl.Drawing;

namespace Automata.Visualization;

///<summary>
/// Static class for creating displayable graphs from finite state automata.
///</summary>
public static class GraphFactory
{
    /// <summary>
    /// Helper method that creates a displayable graph from a collection of sequences.
    /// </summary>
    /// <param name="sequences">The collection of sequences to create the graph from.</param>
    /// <param name="minimize">Indicates whether to minimize the DFA.</param>
    /// <returns>A graph representing the finite state automaton.</returns>
    public static Graph CreateGraph(this IEnumerable<IEnumerable<string>> sequences, bool minimize = true)
    {
        Nfa nfa = new Nfa(sequences); //create an NFA from the sequences

        var dfa = nfa.ToDfa(); //determinize to DFA
        if (minimize)
            dfa = dfa.Minimized(); //minimize the DFA

        return CreateGraph(dfa);
    }

    /// <summary>
    /// Creates a displayable graph from a finite state automaton (FSA).
    /// </summary>
    /// <param name="fsa">The finite state automaton to represent as a graph.</param>
    /// <param name="layerDirection">The layout direction of the graph (default: left-to-right).</param>
    /// <param name="directed">Indicates whether the graph is directed (default: true).</param>
    /// <returns>A graph representing the finite state automaton.</returns>
    public static Graph CreateGraph(this IFsa fsa, bool displayStateIDs = false, LayerDirection layerDirection = LayerDirection.LR, bool directed = true)
    {
        Graph graph = new Graph();
        graph.Attr.LayerDirection = layerDirection;
        IAlphabet alphabet = fsa.Alphabet;

        graph.Directed = directed;
        
        void AddEdge(int fromState, string label, int toState)
        {
            Edge edge = graph.AddEdge(fromState.ToString(), label, toState.ToString());
            edge.Attr.ArrowheadAtTarget = directed ? ArrowStyle.Normal : ArrowStyle.None;
        }

        foreach (Transition transition in fsa.SymbolicTransitions())
            AddEdge(transition.FromState, alphabet[transition.Symbol], transition.ToState);
        
        if (!fsa.IsEpsilonFree)
        {
            foreach (EpsilonTransition transition in fsa.EpsilonTransitions())
                AddEdge(transition.FromState, EpsilonTransition.Epsilon, transition.ToState);
        }

        graph.ConfigNodes(fsa, displayStateIDs); //configure the appearance of the states

        return graph;
    }

    /// <summary>
    /// Configures the appearance of a node in the graph.
    /// </summary>
    /// <param name="node">The node to configure.</param>
    /// <param name="isInitial">Indicates whether the node represents an initial state.</param>
    /// <param name="isFinal">Indicates whether the node represents a final state.</param>
    private static void ConfigNodes(this Graph graph, IFsa fsa, bool displayStateIDs)
    {
        foreach (Node? node in graph.Nodes)
        {
            int state = int.Parse(node.Id);
            bool isInitial = fsa.IsInitial(state);
            bool isFinal = fsa.IsFinal(state);

            node.Attr.Shape = Shape.Circle;
            node.Label.Text = displayStateIDs ? state.ToString() : "";

            if (isInitial)
            {
                node.Attr.Shape = Shape.Circle;
                node.Attr.LineWidth = 3;
                //node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue; //uncomment for color
            }
            if (isFinal)
            {
                node.Attr.Shape = Shape.DoubleCircle;
                node.Attr.LineWidth = 1;
            }
        }
    }

    ///// <summary>
    ///// Configures the appearance of a node in the graph.
    ///// </summary>
    ///// <param name="node">The node to configure.</param>
    ///// <param name="isInitial">Indicates whether the node represents an initial state.</param>
    ///// <param name="isFinal">Indicates whether the node represents a final state.</param>
    //public static void ConfigNode(this Node node, bool isInitial, bool isFinal, int stateId = -1)
    //{
    //    node.Attr.Shape = Shape.Circle;
    //    node.Label.Text = stateId >= 0 ? stateId.ToString() : ""; 

    //    if (isInitial)
    //    {
    //        node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
    //    }
    //    if (isFinal)
    //    {
    //        node.Attr.Shape = Shape.DoubleCircle;
    //        node.Label.Text = " ";
    //    }
    //}
}
