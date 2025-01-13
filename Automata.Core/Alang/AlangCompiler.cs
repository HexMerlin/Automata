using Automata.Core.Operations;

namespace Automata.Core.Alang;

/// <summary>
/// Compiler that compiles <c>Alang</c> regular expression to <c>Finite-State Automata</c>.
/// </summary>
public static class AlangCompiler
{
    public static IFsa Compile(AlangRegex regex, Alphabet alphabet) => regex switch
    {
        Symbol atom => new Mfa(atom.Symbol, alphabet),

        Concatenation concatenation =>
            Compile(concatenation.Left, alphabet).AsNfa()
            .Append(Compile(concatenation.Right, alphabet).AsIDfa()),

        Wildcard => throw new NotImplementedException(),

        Option option => throw new NotImplementedException(),

        KleeneStar kleeneStar => Compile(kleeneStar, alphabet).AsNfa().KleeneStarInPlace(),

        KleenePlus kleenePlus => Compile(kleenePlus, alphabet).AsNfa().KleenePlusInPlace(),
        
        Union union =>
            Compile(union.Left, alphabet).AsNfa()
            .UnionWith(Compile(union.Right, alphabet).AsIDfa()),
        
        Complement complement => throw new NotImplementedException(),
        
        _ => throw new InvalidOperationException("Should never be reached. Only for completeness.")
    };

}
