# Alang (Automata Language)

**Alang** is a formal language for defining finite-state automata using human-readable regular expressions. 
It supports many operations, such as union, intersection, complement and set difference, 
enabling expressions like "(a (b | c)* - (b b))?". 
Alang's syntax is defined by the *Alang Grammar* which is an LL(1) context-free grammar. 
The Alang parser is optimized for fast parsing of very large inputs.
The parser validates syntactic correctness and generates detailed error messages for invalid inputs. 

## Alang Grammar Specification

| Rule                             | Expansion                                                     |
|----------------------------------|---------------------------------------------------------------|
| AlangExpr (root)                     | Union                                                     |
| :small_blue_diamond:Union            | Difference  ('\|' Difference)*                            |
| :small_blue_diamond:Difference       | Intersection ('-' Intersection)*                          |
| :small_blue_diamond:Intersectionr    | Concatenation ('&' Concatenation)*                        |
| :small_blue_diamond:Concatenation    | UnaryExpr+                                                |
| UnaryExpr           | PrimaryExpr<br>┃ Option <br>┃ KleeneStar <br>┃ KleenePlus <br>┃ Complement |
| :small_blue_diamond:Option           | PrimaryExpr '?'                                           |
| :small_blue_diamond:KleenStar        | PrimaryExpr '*'                                           |
| :small_blue_diamond:KleenPlus        | PrimaryExpr '+'                                           |
| :small_blue_diamond:Complement       | Primary '~'                                               |
| PrimaryExpr          | '(' AlangExpr ')' <br>┃ Atom <br>┃  Wildcard <br>┃ EmptySet               |
| :small_blue_diamond:Atom                 | AtomChar+                                             |
| :small_blue_diamond:Wildcard             | '.'                                                   |
| :small_blue_diamond:EmptySet          | '(' ')'                                                  |
| AtomChar             | *any character except operator characters and whitespace*                 |

:small_blue_diamond: Denotes a node-type that can be included in the resulting parse tree.

## Operators Ordered by Precedence (Lowest-to-Highest)

| Precedence | Operation       | Operator Character | Position & Arity   |
|------------|-----------------|--------------------|--------------------|
| 1          | Union           | x \| x             | Infix Binary       |
| 2          | Difference      | x - x              | Infix Binary       |
| 3          | Intersection    | x & x              | Infix Binary       |
| 4          | Concatenation   | x x                | Infix Implicit     |
| 5          | Option          | x?                 | Postfix Unary      |
| 5          | Kleene Star     | x*                 | Postfix Unary      |
| 5          | Kleene Plus     | x+                 | Postfix Unary      |
| 5          | Complement      | x~                 | Postfix Unary      |
| 6          | Group           | ( x )              | Enclosing Unary    |
| 7          | Empty set       | ()                 | Empty parentheses  |
| 7          | Wildcard        | .                  | Atomic leaf        |
| 7          | Atom            | string literal     | Atomic leaf        |

## Notes

- Operators with higher precedence levels bind more tightly than those with lower levels.
- Operators of the same precedence level are left-associative (left-to-right).
- *Whitespace* denotes any whitespace character (i.e. space, tab, newline, etc.)
- Atoms are user defined (alphabet symbols) and can contain any characters except for the operator characters and whitespace.
- Wildcard '.' matches exactly one “Atom” in the alphabet.
- Whitespace is allowed anywhere in the grammar, but it is never required unless to separate directly adjacent atoms.

