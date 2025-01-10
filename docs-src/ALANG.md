# Alang (Automata Language)

**Alang** is a language for defining finite-state automata.

## Alang Grammar Specification

| Rule                 | Expansion                                                         |
|----------------------|-------------------------------------------------------------------|
| AlangExpr                 | UnionExpr                                                         |
| :small_blue_diamond:UnionExpr            | Difference  ('\|' Difference)*         |
| :small_blue_diamond:Difference           | Intersection ('-' Intersection)*      |
| :small_blue_diamond:Intersectionr     | Concatenation ('&' Concatenation)*    |
| :small_blue_diamond:Concatenation    | PostfixExpr Postfix*                      |
| :small_blue_diamond:Option           | Primary '?'                               |
| :small_blue_diamond:KleenStar        | Primary '*'                               |
| :small_blue_diamond:KleenPlus       | Primary '+'                               |
| :small_blue_diamond:Complement       | Primary '~'                               |
| PostfixExpr          | PrimaryExpr PostfixOp*                                            |
| PrimaryExpr          | '(' AlangExpr ')' <br>┃ EmptySetExpr <br>┃  Wildcard <br>┃ Atom   |
| :small_blue_diamond:EmptySetExpr         | '(' ')'                                       |
| :small_blue_diamond:Atom                 | AtomChar+                                     |
| :small_blue_diamond:Wildcard             | '.'                                           |
| PostfixOp            | '?' <br> ┃ '*' <br> ┃ '+' <br> ┃ '~'                              |
| AtomChar             | *any character except operator characters and whitespace*  |


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
- Atoms are user defined (alphabet symbols) and can contain any characters except for the operator characters and whitespace.
- Wildcard '.' matches exactly one “Atom” in the alphabet.
- Whitespace is allowed anywhere in the grammar, but it is never required unless to separate directly adjacent atoms.

