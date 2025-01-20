# Alang (Automata Language)

**Alang** is a formal language for defining finite-state automata using human-readable regular expressions. 
It supports many operations, such as union, intersection, complement and set difference, 
enabling expressions like "(a? (b | c)* - (b b))+". 
Alang's syntax is defined by the *Alang Grammar* which is an LL(1) context-free grammar. 
The Alang parser is optimized for fast parsing of very large inputs.
The parser validates syntactic correctness and generates detailed error messages for invalid inputs. 

## Alang Grammar Specification

| Grammar Rule                         | Expansion                                                     |
|--------------------------------------|---------------------------------------------------------------|
| AlangRegex (root)                    | Union                                                         |
| :small_blue_diamond:Union            | Difference  (`|` Difference)*                                 |
| :small_blue_diamond:Difference       | Intersection (`-` Intersection)*                              |
| :small_blue_diamond:Intersection     | Concatenation (`&` Concatenation)*                            |
| :small_blue_diamond:Concatenation    | UnaryRegex+                                                   |
| UnaryRegex           | PrimaryRegex<br> (Option <br>┃ KleeneStar <br>┃ KleenePlus <br>┃ Complement)* |
| :small_blue_diamond:Option           | PrimaryRegex `?`                                              |
| :small_blue_diamond:KleeneStar       | PrimaryRegex `*`                                              |
| :small_blue_diamond:KleenePlus       | PrimaryRegex `+`                                              |
| :small_blue_diamond:Complement       | PrimaryRegex `~`                                                   |
| PrimaryRegex          | `(` AlangRegex `)` <br>┃ Symbol <br>┃  Wildcard <br>┃ EmptyLang              |
| :small_blue_diamond:Symbol           | SymbolChar+                                                   |
| :small_blue_diamond:Wildcard         | `.`                                                           |
| :small_blue_diamond:EmptyLang        | `()`                                                          |
| SymbolChar                           | *any character except operator characters and whitespace*     |

:small_blue_diamond: Denotes an actual node type in the resulting AST (abstract syntax tree) outputed by the parser.

Note to developers: All types marked with a :small_blue_diamond: have corresponding classes with the exact same names in the namespace **Automata.Core.Alang**.

For an input to be valid, the root rule AlangRegex must cover the entire input, with no residue. 

### Operators
- Operators with higher precedence levels bind more tightly than those with lower levels.
- Operators of the same precedence level are left-associative (left-to-right).
- All *unary* operators are *postfix operators* and all *binary* operators are *infix* operators.

| Precedence | Operation/Unit  | Operator Character | Position & Arity   |
|------------|-----------------|--------------------|--------------------|
| 1          | Union           | L₁ `|` L₂          | Infix Binary       | 
| 2          | Difference      | L₁ `-` L₂          | Infix Binary       |
| 3          | Intersection    | L₁ `&` L₂          | Infix Binary       | 
| 4          | Concatenation   | L₁ L₂              | Infix Implicit     | 
| 5          | Option          | L `?`              | Postfix Unary      | 
| 5          | Kleene Star     | L`*`               | Postfix Unary      | 
| 5          | Kleene Plus     | L`+`               | Postfix Unary      |
| 5          | Complement      | L`~`               | Postfix Unary      |
| 6          | Group           | `(` L `)`          | Enclosing Unary    |
| 7          | EmptyLang       | `()`               | Empty parentheses  |
| 7          | Wildcard        | `.`                | Terminal           |
| 7          | Symbol          | string literal     | Terminal           |


### Whitespace
- Multiple Whitespace is allowed anywhere in the grammar, except within Symbols.
- Whitespace is never required anywhere - except for separating *directly* adjacent Symbols or operators. 
   Thus, the parser resolves all reserved tokens as delimiters: The following are correcly delimited: `hello+world` or `hello(world)`.
- Whitespace denotes any whitespace character (i.e. space, tab, newline, etc.).
- The formal whitespace definition is equivalent to .NET's `char.IsWhiteSpace(char c)`.

### Symbols 
**Symbols** have a specific meaning - as formally defined by automata theory:
- User-defined string literals that constitute the *atoms* of Alang expressions.
- It is equivalent to **symbols** in finite-state automata. 
- Can contain any characters except reserved operator characters or whitespace.
- They can never be empty. 
- Symbols are *strings* and are not to be confused with characters, 
 
### Wildcard
A Wildcard is a special token denoted by a `.` (dot).

It represents any symbol in the alphabet.

For example:

`. - hello`  represents the language of all symbols except 'hello'.

`(. - hello).*`    represents the language of all sequences, except those containing 'hello'.

### The Empty Language ∅ and The Language containing only epsilon {ε}
- The Empty Language ∅ is the language that does not cotain anything.
    - It is written in Alang using empty parentheses `()`.  
    - Its corresponding grammar rule is `EmptyLang` and the parse tree type is `EmptyLang`.
    - Its automata equivalence is an automaton that does not accept anything (not even the empty string).
    - In most scenarios, `()` is not required when writing a Alang expressions.
        However, many operations can result in the empty language. For example `a - (a | b)` is equivalent to `()`.

- The language containing only the empty string {ε}
    - It is written in Alang as `()?`, since the Option operator `?` unites the operand with {ε}:  **L? = L ∪ { ε }**
    - Its automata equivalence is an automaton that only accepts ε.
- Note that `()` ≠ `{ε}`. For instance:
    - Concatenating any language `L` with `()` => `()`.
    - Concatenating any language `L` with `{ε}` => `L`.

### Alang expression examples
`(a? (b | c) )+` : All sequences from the set {a, b, c} where any 'a' must be followed by 'b' or 'c'.

`a+~ b`          : Complement of 'a+' - all sequences that are not 1 or more 'a's, followed by a 'b'

`(x1 | x2 | x3)* - (x1 x2 x3)+` : All sequences constaining {x1, x2, x3}, except repetitions of "x1 x2 x3".

`()`              : The empty language that does not accept anything. For example, it is the result from `hello - hello` and from `hello & world`.

### Operation Definitions
```
### Operation Definitions

Union: L₁ ∪ L₂ = { w | w ∈ L₁ or w ∈ L₂ }  
Difference: L₁ - L₂ = { w | w ∈ L₁ and w ∉ L₂ }  
Intersection: L₁ ∩ L₂ = { w | w ∈ L₁ and w ∈ L₂ }  
Concatenation: L₁ ⋅ L₂ = { w | w = uv, u ∈ L₁, v ∈ L₂ }  
Option: L? = L ∪ { ε }  
Kleene Star: L* = ⋃ₙ₌₀ⁿ Lⁿ, where L⁰ = { ε }, Lⁿ = L ⋅ Lⁿ⁻¹ for n ≥ 1  
Kleene Plus: L⁺ = ⋃ₙ₌₁ⁿ Lⁿ, where Lⁿ = L ⋅ Lⁿ⁻¹ for n ≥ 1  
Complement: ᒾL = Σ* \ L  

```

## C# API
The Alang parser and FSA compiler is provided by the namespace **Automata.Core.Alang**.

Key class: **AlangRegex**

Example usage:
```csharp
 AlangRegex regex = AlangRegex.Parse("(a? (b | c) )+");  // Create an Alang regex

 Mfa fsa = regex.Compile(); // Compile the regex to a minimal finite-state automaton
```
For more information, see the [Automata documentation](index.md)


