# :repeat_one: Automata: A lightweight library for Finite-State Automata 

The **Automata** library provides functionality for working with finite-state automata.

:star: Example Features:
  - Create **NFAs** (Non-deterministic Finite Automata) from sequences or other data.
  - Convert **NFAs** to **DFAs** (Deterministic Finite Automata).
  - Minimize **Automata** to reduce states while preserving functionality.

---

## :hammer: Automata - Core Library

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
## :framed_picture: Automata.Visualization: Visualize Your Automata
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
## :package: NuGet Package releases on GitHub

- [Automata on GitHub Packages](https://github.com/HexMerlin/Automata/pkgs/nuget/Automata)
- [Automata.Visualization on GitHub Packages](https://github.com/HexMerlin/Automata/pkgs/nuget/Automata.Visualization)

## :wrench: Usage and Installation


### Using NuGet Package Manager in Visual Studio

### Add GitHub Packages to NuGet Sources
To install from GitHub Packages, ensure your NuGet sources include the following:
1. In Visual Studio: Navigate to **Tools > NuGet Package Manager > Package Sources**.
2. Add a new source:
   - **Name**: Automata GitHub Packages
   - **Source**: `https://nuget.pkg.github.com/HexMerlin/index.json`
3. Use the new source to install the Automata packages using NugGet Package Manager as normal.

### Using .NET CLI

#### For **Automata**:
```bash
dotnet add package Automata --version 1.0.0 --source https://nuget.pkg.github.com/hexmerlin/index.json
```

#### For **Automata.Visualization** (includes the one above):
```bash
dotnet add package Automata.Visualization --version 1.0.0 --source https://nuget.pkg.github.com/hexmerlin/index.json
```