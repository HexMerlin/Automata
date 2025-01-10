
namespace Automata.Core.Alang;

/// <summary>
/// A cursor for parsing Alang expressions from an input string.
/// </summary>
/// <remarks>
/// A cursor is a lightweight struct that consumes characters from the left of a string input.
/// The input string is never modified. Instead the cursor maintains a lightweight span of the remaining input.
/// The parser available in namespace <see cref="Automata.Core.Alang"/> only need to create a single cursor instance that is passed along during the parse process.
/// The contract of the <see cref="AlangCursor"/> is that it always point to a non-white-space character or <see cref="Chars.EOI"/> if the input is empty.
/// <para>Consequently:</para>
/// <para>- All methods in <see cref="AlangCursor"/> that moves the cursor, must ensure on exit that leading whitespace is trimmed.</para>
/// <para>- All methods in <see cref="AlangCursor"/> can assume on entry that the input is trimmed of leading whitespace.</para>
/// </remarks>
public ref struct AlangCursor
{
    private readonly int OriginalInputLength;

    private ReadOnlySpan<char> cursor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlangCursor"/> struct with the specified input string.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    public AlangCursor(string input)
    {
        this.OriginalInputLength = input.Length;
        this.cursor = input.AsSpan().TrimStart();
    }

    /// <summary>
    /// Gets a value indicating whether the cursor has reached the end of the input.
    /// </summary>
    public readonly bool IsEmpty => cursor.IsEmpty;

    /// <summary>
    /// Gets the first character in the remaining input, or <see cref="Chars.EOI"/> if the input is empty.
    /// </summary>
    private readonly char First
        => this.cursor.IsEmpty ? Chars.EOI : this.cursor[0];

    /// <summary>
    /// Gets a value indicating whether the current character indicates the start of an expression.
    /// </summary>
    public readonly bool IsExpressionStart
          => Chars.IsExpressionStart(First);

    /// <summary>
    /// Indicates whether the first character in the remaining input is the specified character.
    /// </summary>
    public readonly bool Is(char c) => First == c;
        

    /// <summary>
    /// Tries to consume the specified character from the input and advances the cursor if successful.
    /// </summary>
    /// <param name="c">The character to attempt to consume.</param>
    /// <returns><c>true</c> if the character was successfully consumed; otherwise, <c>false</c>.</returns>
    public bool TryConsume(char c)
    {
        if (First != c)
            return false;

        this.cursor = this.cursor.IsEmpty ? this.cursor : this.cursor.Slice(1).TrimStart(); // Consume the character and trim whitespace
        return true;
    }

    /// <summary>
    /// Tries to consume one of the specified characters from the input and advances the cursor if successful.
    /// </summary>
    /// <param name="chars">An array of characters to attempt to consume.</param>
    /// <remarks>This method supports inclusion of <see cref="Chars.EOI"/> to also match against End-Of-Input.</remarks>
    /// <returns>The character that was consumed if successful; otherwise, <see cref="Chars.Invalid"/>.</returns>
    public char TryConsumeAny(params char[] chars)
    {
        char first = First;
        if (chars.Contains(first))
        {
            _ = TryConsume(first); // Will always consume and return true
            return first;
        }
        return Chars.Invalid;
    }

    /// <summary>
    /// Tries to consume an <see cref="Atom"/> from the input.
    /// </summary>
    /// <param name="atom">Contains the consumed <see cref="Atom"/> if successful, otherwise <see langword="null"/>.</param>
    /// <returns>
    /// <c>true</c> if an <see cref="Atom"/> was successfully consumed; otherwise, <c>false</c>.
    /// </returns>
    public bool TryConsumeAtom(out Atom? atom)
    {
        int pos = 0;

        while (pos < this.cursor.Length && Chars.IsAtomChar(this.cursor[pos]))
            pos++;

        if (pos == 0)
        {
            atom = null;
            return false;
        }

        atom = new Atom(this.cursor.Slice(0, pos).ToString());

        this.cursor = this.cursor.Slice(pos).TrimStart();
        return true;
    }

    /// <summary>
    /// Gets the next character in the remaining input as a human-readable string.
    /// </summary>
    public readonly string NextAsString => cursor.IsEmpty ? "End-Of-Input" : cursor[0].ToString();

    /// <summary>
    /// The index of the cursor in relation to the original input string.
    /// </summary>  
    public readonly int CursorIndex => OriginalInputLength - cursor.Length;

    /// <summary>
    /// Returns a string representation of the remaining input.
    /// </summary>
    /// <returns>A string that represents the remaining input.</returns>
    public override readonly string ToString() => cursor.IsEmpty ? "<EMPTY>" : $"'{string.Join("", cursor.ToArray())}'";
}
