# :repeat_one: Automata: A lightweight library for Finite-State Automata 

The **Automata** library provides functionality finite-state automata. 

   🌟 Example Features:
   - Create NFAs (Non-Deterministic Finite Automata)** from sequences.
   - Convert NFAs to DFAs** (Deterministic Finite Automata).
   - Minimize Automata** to reduce states while preserving functionality.

---

## :hammer: Automata - Core Library

The core library provides essential functionality for finite-state automata operations.
It is a lightweight and clean option for automata operations without visualization.

### :bulb: C# Example: Create and Manipulate Automata
```csharp
// Create random sequences of strings (symbols can be any strings; using number-strings here)
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
## :package: NuGet Package releases on GitHub

- [Automata on GitHub Packages](https://github.com/HexMerlin/Automata/pkgs/nuget/Automata)
- [Automata.Visualization on GitHub Packages](https://github.com/HexMerlin/Automata/pkgs/nuget/Automata.Visualization)

## :wrench: Usage and Installation


### Using NuGet Package Manager in Visual Studio

1. Open your project in Visual Studio.
2. Go to **Tools > NuGet Package Manager > Manage NuGet Packages for Solution...**.
3. Add the GitHub Packages feed to your NuGet package sources:
   - URL: `https://nuget.pkg.github.com/HexMerlin/index.json`
4. Search for the package and click **Install**.

### Using .NET CLI

#### For **Automata**:
```bash
dotnet add package Automata --version 1.0.0
```

#### For **Automata.Visualization**:
```bash
dotnet add package Automata.Visualization --version 1.0.0
```