using Automata.Core;
using Automata.Core.Alang;

namespace Automata.ConsoleOnly;

/// <summary>
/// Project including only the core library without any visualization dependencies.
/// </summary>
internal class Program
{
    /// <summary>
    /// A simple program that demonstrates creating an FSA from a regex and displaying it.
    /// This program is a pure console application without any Visualization dependencies.
    /// </summary>
    public static void Main()
    {
        string regex = "(a? (b | c) )+ (d e)+"; // Define a regex pattern
        Console.WriteLine($"Creating a minimal FSA for regex: {regex}");

        Mfa fsa = AlangRegex.Compile(regex);  // Create an FSA from a regex
        
        Console.WriteLine("--- FSA stats ---");
        Console.WriteLine($"States: {fsa.StateCount}");
        Console.WriteLine($"Transitions: {fsa.TransitionCount}");


    }
}
