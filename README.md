[![NuGet Version](https://img.shields.io/nuget/v/Automata.Core)](https://www.nuget.org/packages/Automata.Core)
[![NuGet Version](https://img.shields.io/nuget/v/Automata.Visualization)](https://www.nuget.org/packages/Automata.Visualization)

# :repeat_one: Automata: A lightweight library for Finite-State Automata 

The **Automata** library provides functionality for working with finite-state automata.

:star: Example Features:
  - Create **NFAs** (Non-deterministic Finite Automata) from sequences or other data.
  - Convert **NFAs** to **DFAs** (Deterministic Finite Automata).
  - Minimize **Automata** to reduce states while preserving functionality.

---

## :hammer: Automata.Core - Core Library

The core library provides essential tools for finite-state automata operations.
It offers a lightweight and clean solution without visualization features.

### :bulb: C# Example: Create and Manipulate Automata
```csharp
// Generate random symbol sequences (numbers as symbols in this example)
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

### :bulb: C# Full example program: Automata Visualization

```csharp
using Automata.Visualization;

// Creates the main console window.
ConsoleWindow consoleWindow = ConsoleWindow.Create();

// Write some colored text output to the console window
consoleWindow.WriteLine("Creating graph...", System.Drawing.Color.Blue);

//Create some random sequences
var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString()));

// Create and display a minimized graph object from the sequences
var graph = GraphFactory.CreateGraph(sequences, minimize: true);

// Open a new non-modal window that displays the graph in it. 
consoleWindow.ShowGraph(graph);

// Write some more colored text output to the console window
consoleWindow.WriteLine("Graph created.", System.Drawing.Color.Green); 
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
  - [Microsoft.MSAGL](https://github.com/microsoft/automatic-graph-layout)

## :scroll: License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).