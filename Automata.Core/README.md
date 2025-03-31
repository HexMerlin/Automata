
The Automata.Core library provides functionality for working with finite-state automata.
The library is highly optimized and ideal for very large automata.

Example Features:
  - Create automata from regular expressions, sequences, or other data.
  - Convert **NFAs** to **DFAs**.
  - Minimize **Automata** to their optimal minimal representation.
  - Provides **Alang** (Automata Language) as an option for defining and creating automata.
  - Perform many operations on automata (union, intersection, difference, complement, etc.).


### C# Example: Create and Operate on Automata

The following example utilizes regular expressions written in `Alang`. 
`Alang` (**Automata Language**) is a language for defining automata using regular operations.
The implementation of Alang is contained in the namespace `Automata.Core.Alang`
Read more about `Alang` in the [Alang documentation](https://hexmerlin.github.io/Automata/ALANG.html).

```csharp
// Compile a regex to a FSA (all sequences of {a, b, c} where any 'a' must be followed by 'b' or 'c')
var fsa = AlangRegex.Compile("(a? (b | c) )+");

// Compile two other automata
var test1 = AlangRegex.Compile("a b b c b");
var test2 = AlangRegex.Compile("a b a a b");

// Test the language overlap of the FSA with the two test regexes
bool overlaps1 = fsa.Overlaps(test1); //true

bool overlaps2 = fsa.Overlaps(test2); //false

```


