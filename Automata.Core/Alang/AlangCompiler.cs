namespace Automata.Core.Alang;

/// <summary>
/// Compiler that compiles <c>Alang</c> regular expression to <c>Finite-State Automata</c>.
/// </summary>
public static class AlangCompiler
{
    public static IFsa Compile(AlangRegex regex)
    {
        return regex switch
        {
            //Atom atom => new AtomFsa(atom.Symbol),
            //Wildcard => new WildcardFsa(),
            //Option option => new OptionFsa(Compile(option.Operand)),
            //KleeneStar kleeneStar => new KleeneStarFsa(Compile(kleeneStar.Operand)),
            //KleenePlus kleenePlus => new KleenePlusFsa(Compile(kleenePlus.Operand)),
            //Concatenation concatenation => new ConcatenationFsa(Compile(concatenation.Left), Compile(concatenation.Right)),
            //Union union => new UnionFsa(Compile(union.Left), Compile(union.Right)),
            //Complement complement => new ComplementFsa(Compile(complement.Operand)),
            _ => throw new InvalidOperationException("Should never be reached. Only for completeness.")
        };
    }

}
