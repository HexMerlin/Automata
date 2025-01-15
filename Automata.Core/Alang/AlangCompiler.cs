using Automata.Core.Operations;

namespace Automata.Core.Alang;

/// <summary>
/// Compiler that compiles <c>Alang</c> regular expression to <c>Finite-State Automata</c>.
/// </summary>
public static class AlangCompiler
{
    /// <summary>
    /// Compiles the specified <paramref name="regex"/> into a finite-state automaton using the given <paramref name="alphabet"/>.
    /// </summary>
    /// <param name="regex">The <see cref="AlangRegex"/> to compile.</param>
    /// <param name="alphabet">The <see cref="Alphabet"/> to use for the automaton.</param>
    /// <returns>An <see cref="IFsa"/> representing the compiled finite-state automaton.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unknown <see cref="AlangRegex"/> type is encountered.</exception>
    public static Mfa Compile(AlangRegex regex, Alphabet alphabet) => regex switch
    {

        // Compiles a Union regex into an automaton.
        Union union => Compile(union.Left, alphabet).AsNfa()
            .UnionWith(Compile(union.Right, alphabet).AsIDfa()).AsMfa(),

        // Compiles a Difference regex into an automaton.
        Difference difference => Ops.Difference(
            Compile(difference.Left, alphabet).AsIDfa(),
            Compile(difference.Right, alphabet).AsMfa()).AsMfa(),
        
        Intersection intersection => Ops.Intersection(
            Compile(intersection.Left, alphabet).AsIDfa(),
            Compile(intersection.Right, alphabet).AsIDfa()).AsMfa(),

        // Compiles a Concatenation regex into an automaton.
        Concatenation concatenation => Compile(concatenation.Left, alphabet).AsNfa()
            .Append(Compile(concatenation.Right, alphabet).AsIDfa()).AsMfa(),

        // Compiles an Option regex into an automaton.
        Option option => Compile(option.Operand, alphabet).AsNfa().OptionWith().AsMfa(),

        // Compiles a KleeneStar regex into an automaton.
        KleeneStar kleeneStar => Compile(kleeneStar.Operand, alphabet).AsNfa().KleeneStarWith().AsMfa(),

        // Compiles a KleenePlus regex into an automaton.
        KleenePlus kleenePlus => Compile(kleenePlus.Operand, alphabet).AsNfa().KleenePlusInWith().AsMfa(),

        // Compiles a Complement regex intoan automaton.
        Complement complement => Compile(complement.Operand, alphabet).AsMfa().Complement().AsMfa(),
                
        // Compiles a Symbol regex into an automaton.
        Symbol symbol => new Mfa(symbol.Value, alphabet),

        // Compiles a Wildcard regex into an automaton.
        Wildcard => new Mfa(Chars.Wildcard.ToString(), alphabet).AsMfa(),

        EmptyLang emptyLang => new Mfa(alphabet),

        // Throws an exception for an unknown regex type.
        _ => throw new InvalidOperationException($"Unexpected {nameof(AlangRegex)} type {regex.GetType()}")
    };
}
