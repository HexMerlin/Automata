# Alang (Automata Language)

**Alang** is a language for defining finite-state automata.

## Alang Grammar Specification

```ebnf
AlangExpr             ::= UnionExpr
UnionExpr             ::= DifferenceExpr ('|' DifferenceExpr)*
DifferenceExpr        ::= IntersectionExpr ('-' IntersectionExpr)*
IntersectionExpr      ::= ConcatenationExpr ('&' ConcatenationExpr)*
ConcatenationExpr     ::= PostfixExpr PostfixExpr*
PostfixExpr           ::= PrimaryExpr PostfixOp*
PrimaryExpr           ::= '(' AlangExpr ')'
                        | EmptySetExpr
                        | Wildcard
                        | Atom

EmptySetExpr          ::= '(' ')'
Atom                  ::= AtomChar+

PostfixOp             ::= '?' | '*' | '+' | '~'
AtomChar              ::= ^('|' | '&' | '-' | '?' | '*' | '+' | '~' | '(' | ')' | '\ws')
```

## Operators Ordered by Precedence (Lowest-to-Highest)

| Precedence | Operation       | Operator Character | Position & Arity   |
|------------|-----------------|--------------------|--------------------|
| 1          | Union           | x \| x             | Infix binary       |
| 2          | Difference      | x - x              | Infix binary       |
| 3          | Intersection    | x & x              | Infix binary       |
| 4          | Concatenation   | x x                | Infix Implicit     |
| 5          | Option          | x?                 | Postfix unary      |
| 5          | Kleene Star     | x*                 | Postfix unary      |
| 5          | Kleene Plus     | x+                 | Postfix unary      |
| 5          | Complement      | x~                 | Postfix unary      |
| 6          | Group           | ( x )              | Enclosing unary    |
| 7          | Empty set       | ()                 | Empty parentheses  |
| 7          | Wildcard        | .                  | Atomic leaf        |
| 7          | Atom            | string literal     | Atomic leaf        |

## Notes

- Operators with higher precedence levels bind more tightly than those with lower levels.
- Operators of the same precedence level are left-associative.
- '\ws' denote any whitespace character (i.e. space, tab, newline, etc.)
- Atoms are user defined and can contain any characters except for the operator characters and whitespace.
- Wildcard '.' matches exactly one “Atom” in the alphabet.
- Whitespace is allowed anywhere in the grammar, but it is never required unless to separate directly adjacent atoms.

