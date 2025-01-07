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
PrimaryExpr           ::= '(' [AlangExpr] ')' | Atom
Atom                  ::= AtomChar+

PostfixOp             ::= '?' | '*' | '+' | '~'
AtomChar              ::= ^('|' | '&' | '-' | '?' | '*' | '+' | '~' | '(' | ')' | '\ws')
```

## Operators Ordered by Precedence (Lowest-to-Highest)

| Precedence | Operation       | Operator Character | Position & Arity   |
|------------|-----------------|--------------------|--------------------|
| 1          | Union           | \|                | Infix binary       |
| 2          | Difference      | -                | Infix binary       |
| 3          | Intersection    | &                | Infix binary       |
| 4          | Concatenation   | (implicit)         | Juxtaposition      |
| 5          | Option          | ?                | Postfix unary      |
| 5          | Kleene Star     | *                | Postfix unary      |
| 5          | Kleene Plus     | +                | Postfix unary      |
| 5          | Complement      | ~                | Postfix unary      |
| 6          | Group           | ( )              | Enclosing unary    |
|            | Atom            | string literal     | Atomic leaf        |

## Notes

- Operators with higher precedence levels bind more tightly than those with lower levels.
- Operators of the same precedence level are left-associative.
- Whitespace is allowed anywhere in the grammar, but it is never required unless to separate directly adjacent atoms.

