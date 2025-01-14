using Automata.Core.Operations;

namespace Automata.Core.Alang;

/// <summary>
/// Compiler that compiles <c>Alang</c> regular expression to <c>Finite-State Automata</c>.
/// </summary>
public static class AlangCompiler
{
    public static IFsa Compile(AlangRegex regex, Alphabet alphabet) => regex switch
    {
        Symbol symbol => new Mfa(symbol.Value, alphabet),

        Concatenation concatenation => Compile(concatenation.Left, alphabet).AsNfa()
            .Append(Compile(concatenation.Right, alphabet).AsIDfa()),

        Wildcard => new Mfa(Chars.Wildcard.ToString(), alphabet), //TODO: Wildcard should be replaced by union over Alphabet

        Option option => throw new NotImplementedException(),

        KleeneStar kleeneStar => Compile(kleeneStar.Operand, alphabet).AsNfa().KleeneStarInPlace(),

        KleenePlus kleenePlus => Compile(kleenePlus.Operand, alphabet).AsNfa().KleenePlusInPlace(),
        
        Union union => Compile(union.Left, alphabet).AsNfa()
            .UnionWith(Compile(union.Right, alphabet).AsIDfa()),
        
        Complement complement => Compile(complement.Operand, alphabet).AsMfa().Complement(),

        _ => throw new InvalidOperationException("Should never be reached. Only for completeness.")
    };

}
