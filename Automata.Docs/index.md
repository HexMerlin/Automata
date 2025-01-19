---
title: Automata API Documentation
layout: landing
---

# Automata documentation

Welcome to the documentation for **Automata**.

---


## API Documentation

- [Automata.Core](xref:Automata.Core)  
    - [Automata.Core.Alang](xref:Automata.Core.Alang)
    - [Automata.Core.Operations](xref:Automata.Core.Operations)
- [Automata.Visualization](xref:Automata.Visualization)  


---
[Automata source code repo at GitHub](https://github.com/HexMerlin/Automata)

---

## ALANG - A language for definining finite-state automata. 

**Alang** is a formal language for defining finite-state automata using human-readable regular expressions. 
It supports many operations, such as union, intersection, complement and set difference, 
enabling expressions like `(a? (b | c)* - (b b))+`. 

[Alang documentation](ALANG.md)

---
## Automaton types

The automaton types in the Automata library are:

| C# class | Automaton type        | Mutable | Internal Representation  | Transition/state access | Memory Footprint  |
|----------------------------------|---------|---------|--------------- |-------------------------|-------------------|
| `Nfa`    | Non-deterministic FSA | Yes     | Dual Red-Black Trees     | O(log n)                | Medium            |
| `Dfa`    | Deterministic FSA     | Yes     | Red-Black Tree           | O(log n)                | Low               |
| `Mfa`    | Minimal Canonical FSA | No      | Contiguous memory block  | O(log n)                | Very low          |

---
## C# Examples: Create and Operate on Automata

The following examples creates FSAs from regular expressions in `Alang`. 

### :page_facing_up: Example: Console application using `Automata.Core`
```csharp
using Automata.Core;
using Automata.Core.Alang;
using Automata.Core.Operations;

namespace Automata.App;

public static class Program
{
    public static void Main()
    {
        // Compile a regex to a FSA (all sequences of {a, b, c} where any 'a' must be followed by 'b' or 'c')
        var fsa = AlangRegex.Compile("(a? (b | c) )+");

        // Compile two other automata
        var test1 = AlangRegex.Compile("a b b c b");
        var test2 = AlangRegex.Compile("a b a a b");

        // Test the language overlap of the FSA with the two test regexes
        Console.WriteLine(fsa.Overlaps(test1)); //output: true
        Console.WriteLine(fsa.Overlaps(test2)); //output: false
    }
}

```
---

### :page_facing_up: Example: Console application using `Automata.Visualization`

The application will display the following automaton in a new window:

![Example image](images/automaton_example_1.svg)

```csharp
using Automata.Core;
using Automata.Core.Alang;
using Automata.Core.Operations;
using Automata.Visualization;

namespace Automata.App;
public static class Program
{
    public static void Main()
    {
        var fsa = AlangRegex.Compile("(a? (b | c) )+");  // Create an FSA from a regex

        Console.WriteLine("Creating a minimal FSA and displaying it."); // Write some info to the console

        var graph = fsa.CreateGraph(displayStateIDs: true); // Create a graph object (FSA with layout) 

        GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal window that displays the graph

        Console.WriteLine("FSA is displayed."); // Write some info to the console
    }
}

```
---
