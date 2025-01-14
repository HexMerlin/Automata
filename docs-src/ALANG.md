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
| :small_blue_diamond:Union            | Difference  (`\|` Difference)*                                 |
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

:small_blue_diamond: Denotes a node type that may be included in the resulting parse tree.

The root rule AlangRegex must cover the entire input, with no residue. 

### Operators
- Operators with higher precedence levels bind more tightly than those with lower levels.
- Operators of the same precedence level are left-associative (left-to-right).


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
- Whitespace denotes any whitespace character (i.e. space, tab, newline, etc.)
- Whitespace is allowed anywhere in the grammar, except within Symbols.
- Whitespace it is never required unless to separate directly adjacent Symbols or operators. 

### Symbols 
**Symbols** have a specific meaning and are defined as:
- User-defined string literals that constitute the *atoms* of Alang expressions.
- Directly equivalent to **alphabet symbols** in the context of finite-state automata. 
- Can contain any characters except reserved operator characters or whitespace.
- Can never be empty. 
- They are not to be confused with characters. 
 
### Wildcard
A Wildcard is a special token denoted by a `.` (dot).

It represents any symbol in the alphabet.

For example:

`. - hello`  represents the language of all symbols except 'hello'.

`(. - hello).*`    represents the language of all sequences, except those starting with 'hello'.

### Empty Language ∅
- The Empty Language (∅) is the language that does not cotain anything. 
- It written in Alang with an empty pair of parentheses `()`.
- Its corresponding grammar rule is `EmptyLang` and the parse tree type is `EmptyLang`.
- Its automata equivalence is an automaton that does not accept anything (not even the empty string).
- In most scenarios, `()` is not required when writing a Alang expressions.
  However, many operations can result in the empty language. For example `a - (a | b)` is equivalent to `()`.
- Please note that `()` is not the same as `ε` (empty string).
  For instance, concatenating `()` with any language results in `()`.

### Operation Definitions
```
Union: L₁ ∪ L₂ = { w | w ∈ L₁ or w ∈ L₂ }
Difference: L₁ - L₂ = { w | w ∈ L₁ and w ∉ L₂ }
Intersection: L₁ ∩ L₂ = { w | w ∈ L₁ and w ∈ L₂ }
Concatenation: L₁ ⋅ L₂ = { w | w = uv, u ∈ L₁, v ∈ L₂ }
Option: L? = L ∪ { ε }
Kleene Star: L* = ⋃ₙ₌₀^∞ Lⁿ, where L⁰ = { ε }, Lⁿ = L ⋅ Lⁿ⁻¹ for n ≥ 1
Kleene Plus: L⁺ = ⋃ₙ₌₁^∞ Lⁿ, where Lⁿ = L ⋅ Lⁿ⁻¹ for n ≥ 1
Complement: ᒾL = Σ* \ L
```