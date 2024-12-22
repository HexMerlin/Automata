[![NuGet Version](https://img.shields.io/nuget/v/Automata.Core)](https://www.nuget.org/packages/Automata.Core)  
**Automata.Core** - Core library (if you do not need visualization).

[![NuGet Version](https://img.shields.io/nuget/v/Automata.Visualization)](https://www.nuget.org/packages/Automata.Visualization)  
**Automata.Visualization** - Full library that also includes visualization and rendering of automata.

# :repeat_one: Automata: A lightweight library for Finite-State Automata 

The **Automata** library provides functionality for working with finite-state automata.

:star: Example Features:
  - Create **NFAs** (Non-deterministic Finite Automata) from sequences or other data.
  - Convert **NFAs** to **DFAs** (Deterministic Finite Automata).
  - Minimize **Automata** to reduce states while preserving functionality.
---

## :green_book: API Documentation 

- Get it here:
 [Automata API Documentation](https://hexmerlin.github.io/Automata/index.html)

---
## :memo: Source Code

Find the source code on GitHub:  
[Automata GitHub Repository](https://hexmerlin.github.io/Automata)

---
![Example image](Automaton.png)
---

## :hammer: Automata.Core - Core Library

The core library provides essential tools for finite-state automata operations.
It offers a lightweight and clean solution without visualization features.

### :bulb: C# Example: Create and Manipulate Automata
```csharp
//Create some random sequences of strings
var sequences = Enumerable.Range(0, 10)
    .Select(_ => Enumerable.Range(0, 8)
    .Select(_ => Random.Shared.Next(4).ToString()));

// Create an empty NFA.
NFA nfa = new NFA();  

// Add all sequences to the NFA
nfa.AddAll(sequences);

// Determinize the NFA to a DFA
DFA dfa = nfa.ToDFA();

// Minimize the DFA
DFA minDFA = dfa.Minimized();
```
---
## :framed_picture: Automata.Visualization: Automata.Core + Visualization
The Automata.Visualization library extends the core Automata functionality with visualization capabilities, powered by MSAGL (Microsoft Automatic Graph Library).

:key: Key Features:
- Visualize automata as graphs.
- Includes all core Automata functionality.

### :bulb: C# Full example program: Create an automaton and display it from a Console app

```csharp
using Automata.Visualization;

Console.WriteLine("Creating graph."); // Write some text output to the console window

var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString())); //Create some random sequences of strings

IFsa fsa = new NFA(sequences).ToDFA().Minimized();
        
Graph graph = GraphFactory.CreateGraph(fsa); // Create a graph object from the automaton

//Graph graph = GraphFactory.CreateGraph(sequences); //Alternatively you can use this command, to replace the 2 lines above

GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal interactive window that displays the graph in it

Console.WriteLine("Graph is displayed."); // Write some text output to the console window

```
---
## :package: NuGet Package releases on Nuget.org

## :wrench:  NuGet Installation

Install the packages via the .NET CLI or Package Manager in Visual Studio.

### Automata.Core
```bash
dotnet add package Automata.Core
```
### Automata.Visualization

```bash
dotnet add package Automata.Visualization
```
## :computer: Target Framework Compatibility

- **Automata.Core**: .NET 9.0 and later  
- **Automata.Visualization**: .NET 9.0 and later  

## :link: Dependencies

- **Automata.Core**:
  - None

- **Automata.Visualization**:
  - [**Automata.Core**](https://www.nuget.org/packages/Automata.Core)
  - [Microsoft.MSAGL](https://github.com/microsoft/automatic-graph-layout)

    These dependencies will be automatically installed when you install `Automata.Visualization` via NuGet.

## :scroll: License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).