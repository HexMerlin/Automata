namespace Automata.Core.Alang;


/// <summary>
/// Represents an expression in the <c>Alang</c> (Automata language) used for defining finite-state automata. 
/// <see cref="AlangRegex"/> has a one-to-one correspondence with Finite State Automata.
/// </summary>
/// <remarks>
/// The <c>Alang</c> language is a domain-specific language designed for defining and working with finite-state automata.
/// This class serves as the starting point for parsing <c>Alang</c> expressions with <see cref="AlangRegex.Parse(string)"/>.
/// For more information about the <c>Alang</c> language, see the <see href="https://hexmerlin.github.io/Automata/ALANG.html">Alang Grammar Specification</see>.
/// </remarks>
public abstract class AlangRegex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlangRegex"/> class.
    /// </summary>
    protected AlangRegex() { }

    /// <summary>
    /// Precedence level of this expression according to the Alang grammar specification.
    /// </summary>
    /// <value>
    /// An integer representing the precedence level of the expression.
    /// </value>
    public abstract int Precedence { get; }

    /// <summary>
    /// String representation of this expression in valid Alang language syntax.
    /// </summary>
    /// <value>
    /// A string representing the expression in Alang syntax.
    /// </value>
    public abstract string AlangExpressionString { get; }

    /// <summary>
    /// Indicates whether this expression is an empty string.
    /// An empty string is not a valid expression in Alang.
    /// Used internally by the Parser to handle empty strings.
    /// </summary>
    /// <value>
    /// <see langword="true"/> <c>iff</c> this expression is an empty string; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsEmptyString => this is Symbol symbol && symbol.Value.Length == 0;

#region Static Methods
    /// <summary>
    /// Parses the specified regex string into an <see cref="AlangRegex"/>.
    /// </summary>
    /// <param name="regexString">A regex string on Alang format.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the regex string is in invalid format.</exception>
    public static AlangRegex Parse(string regexString)
    {
        AlangCursor cursor = new(regexString);
        cursor.ShouldNotBeEmpty();
        AlangRegex regex = ParseAlangRegex(ref cursor);
        cursor.ShouldNotBeRightParen();
        return regex;
    }

    /// <summary>
    /// Compiles the specified regex string directly into a finite-state automaton.
    /// A new <see cref="Alphabet"/> for the automaton is created, containing all symbols in the regex string.
    /// </summary>
    /// <param name="regexString">A regex string on Alang format.</param>
    /// <remarks>
    /// When possible, consider using a shared single alphabet for multiple automata, for added performance.
    /// </remarks>
    /// <returns>An <see cref="Mfa"/> representing the compiled automaton.</returns>
    /// <exception cref="AlangFormatException">Thrown when the regex string is in invalid format.</exception>
    public static Mfa Compile(string regexString)
        => Parse(regexString).Compile();

    /// <summary>
    /// Compiles the specified regex string directly into a finite-state automaton.
    /// A new <see cref="Alphabet"/> for the automaton is created, containing all symbols in the regex string and the specified additional symbols.
    /// </summary>
    /// <param name="regexString">A regex string on Alang format.</param>
    /// <param name="addSymbols">Additional symbols to include in the alphabet.</param>
    /// <remarks>All alphabet symbols will be included in generic constructs, such as '<c>.</c>' (Wildcard) and '<c>~</c>' (Complement)
    /// <para>When possible, consider using a shared single alphabet for multiple automata, for added performance.</para>
    /// </remarks>
    /// <returns>An <see cref="Mfa"/> representing the compiled automaton.</returns>
    /// <exception cref="AlangFormatException">Thrown when the regex string is in invalid format.</exception>
    public static Mfa Compile(string regexString, params string[] addSymbols)
        => Parse(regexString).Compile(addSymbols);
    
    #endregion Static Methods

    /// <summary>
    /// Compiles this <see cref="AlangRegex"/> into an automaton using the specified <paramref name="alphabet"/>.
    /// </summary>
    /// <param name="alphabet">The alphabet to use for compilation.</param>
    /// <remarks>The alphabet is extended with any symbols not currently in it.</remarks>
    /// <returns>An <see cref="Mfa"/> representing the compiled finite state automaton.</returns>
    public Mfa Compile(Alphabet alphabet) => AlangCompiler.Compile(this, alphabet);

    /// <summary>
    /// Compiles this <see cref="AlangRegex"/> into an automaton.
    /// A new <see cref="Alphabet"/> for the automaton is created containing all referenced symbols.
    /// </summary>
    /// <remarks>
    /// This method creates a new <see cref="Alphabet"/>.
    /// <para>When possible, consider using a shared single alphabet for multiple automata, for added performance.</para>
    /// </remarks>
    /// <returns>An <see cref="Mfa"/> representing the compiled finite state automaton.</returns>
    public Mfa Compile() => Compile(new Alphabet());

    /// <summary>
    /// Compiles this <see cref="AlangRegex"/> into an automaton.
    /// A new <see cref="Alphabet"/> for the automaton is created, containing all referenced symbols and the specified additional symbols.
    /// </summary>
    /// <param name="addSymbols">Additional symbols to include in the alphabet.</param>
    /// <remarks>All alphabet symbols will be included in generic constructs, such as '<c>.</c>' (Wildcard) and '<c>~</c>' (Complement)
    /// <para>When possible, consider using a shared single alphabet for multiple automata, for added performance.</para>
    /// </remarks>
    /// <returns>An <see cref="Mfa"/> representing the compiled finite state automaton.</returns>
    public Mfa Compile(params string[] addSymbols) => Compile(new Alphabet(addSymbols));

    /// <summary>
    /// Parses the rule <c>AlangRegex</c> in the Alang grammar specification.
    /// </summary>
    /// <remarks>
    /// Parses an expression from the cursor, starting at the lowest precedence operator.
    /// </remarks>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    private static AlangRegex ParseAlangRegex(ref AlangCursor cursor)
    {
        AlangRegex regex = Union.Parse(ref cursor);
        return regex;
    }

    /// <summary>
    /// Parses the rule <c>PrimaryRegex</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed primary expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when parentheses are unmatched in the input.</exception>
    internal static AlangRegex ParsePrimaryRegex(ref AlangCursor cursor)
    {
        if (cursor.TryConsume(Chars.LeftParen))
        {
            cursor.ShouldNotBeOperator();
            AlangRegex regex = ParseAlangRegex(ref cursor); // Parse inner expression
            cursor.ShouldBeRightParen();
            _ = cursor.TryConsume(Chars.RightParen);

            if (regex.IsEmptyString)
                return new EmptyLang(); // Captured empty parentheses '()' => EmptyLang

            return regex;
        }

        if (cursor.TryConsume(Chars.Wildcard))
            return new Wildcard();

        return cursor.ConsumeSymbol();
    }

    /// <summary>
    /// Returns the string of the given expression, enclosed in parentheses only if necessary based on operator precedence.
    /// </summary>
    /// <remarks>
    /// Parentheses are added if the precedence of the given expression is lower than the precedence of the parent.
    /// </remarks>
    /// <param name="expr">The expression to get the string representation of.</param>
    /// <param name="parent">The parent expression of <paramref name="expr"/>.</param>
    /// <returns>The expression string, potentially enclosed in parentheses.</returns>
    protected static string Param(AlangRegex expr, AlangRegex parent)
        => expr.Precedence >= parent.Precedence
            ? expr.AlangExpressionString
            : $"({expr.AlangExpressionString})";

    /// <summary>
    /// String that represents the current expression in Alang format.
    /// </summary>
    /// <returns>The expression string of this expression in Alang format.</returns>
    public override string ToString() => AlangExpressionString;

    /// <summary>
    /// Returns this expression and all its descendant expressions in a depth-first order.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AlangRegex"/> representing this expression and all its descendants.</returns>
    public IEnumerable<AlangRegex> DescendantsAndSelf()
    {
        yield return this;
        if (this is BinaryRegex binary)
        {
            foreach (AlangRegex child in binary.Left.DescendantsAndSelf())
                yield return child;
            foreach (AlangRegex child in binary.Right.DescendantsAndSelf())
                yield return child;
        }
        else if (this is UnaryRegex unaryRegex)
        {
            foreach (AlangRegex child in unaryRegex.Operand.DescendantsAndSelf())
                yield return child;
        }
    }
}


