namespace Automata.Core.Alang;

/// <summary>
/// Compiler that compiles <c>Alang</c> regular expression to <c>Finite-State Automata</c>.
/// </summary>
internal class AlangCompiler
{
    private readonly Alphabet Alphabet;

    /// <summary>
    /// Compiles the specified <paramref name="regex"/> into a finite-state automaton using the given <paramref name="alphabet"/>.
    /// </summary>
    /// <param name="regex">The <see cref="AlangRegex"/> to compile.</param>
    /// <param name="alphabet">The <see cref="Alphabet"/> to use for the automaton.</param>
    /// <returns>An <see cref="Fsa"/> representing the compiled finite-state automaton.</returns>
    internal static Mfa Compile(AlangRegex regex, Alphabet alphabet)
        => new AlangCompiler(regex, alphabet).Compile(regex);

    private AlangCompiler(AlangRegex regex, Alphabet alphabet)
    {
        this.Alphabet = alphabet;
        alphabet.AddAll(regex.DescendantsAndSelf().OfType<Symbol>().Select(s => s.Value));
    }

    private Mfa Compile(AlangRegex regex) => regex switch
    {
        // Compiles a Union regex into an automaton.
        Union union => Compile(union.Left).AsNfa()
            .UnionWith(Compile(union.Right).AsDeterministic()).AsMfa(),

        // Compiles a Difference regex into an automaton.
        Difference difference => Ops.Difference(
            Compile(difference.Left).AsDeterministic(),
            Compile(difference.Right).AsMfa()).AsMfa(),
        
        Intersection intersection => Ops.Intersection(
            Compile(intersection.Left).AsDeterministic(),
            Compile(intersection.Right).AsDeterministic()).AsMfa(),

        // Compiles a Concatenation regex into an automaton.
        Concatenation concatenation => Compile(concatenation.Left).AsNfa()
            .Append(Compile(concatenation.Right).AsDeterministic()).AsMfa(),

        // Compiles an Option regex into an automaton.
        Option option => Compile(option.Operand).AsNfa().OptionWith().AsMfa(),

        // Compiles a KleeneStar regex into an automaton.
        KleeneStar kleeneStar => Compile(kleeneStar.Operand).AsNfa().KleeneStarWith().AsMfa(),

        // Compiles a KleenePlus regex into an automaton.
        KleenePlus kleenePlus => Compile(kleenePlus.Operand).AsNfa().KleenePlusWith().AsMfa(),

        // Compiles a Complement regex into an automaton.
        Complement complement => Compile(complement.Operand).AsMfa().Complement().AsMfa(),
                
        // Compiles a Symbol regex into an automaton.
        Symbol symbol => new Mfa(symbol.Value, Alphabet),

        // Compiles a Wildcard regex into an automaton.
        Wildcard => Mfa.CreateWildcard(Alphabet),

        EmptyLang emptyLang => Mfa.CreateEmpty(Alphabet),

        // Throws an exception for an unknown regex type. Internal error, should never happen.
        _ => throw new InvalidOperationException($"Unexpected {nameof(AlangRegex)} type {regex.GetType()}")
    };
}
