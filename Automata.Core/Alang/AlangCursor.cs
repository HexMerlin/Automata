
namespace Automata.Core.Alang;

/// <summary>
/// A cursor for parsing Alang expressions from an input string.
/// </summary>
public ref struct AlangCursor
{
    private ReadOnlySpan<char> cursor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlangCursor"/> struct with the specified input string.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    public AlangCursor(string input)
    {
        this.cursor = input.AsSpan().Trim();
    }

    /// <summary>
    /// Gets a value indicating whether the cursor has reached the end of the input.
    /// </summary>
    public readonly bool IsEmpty => cursor.IsEmpty;

    /// <summary>
    /// Gets the first character in the remaining input, or <see cref="Chars.EOI"/> if the input is empty.
    /// </summary>
    public readonly char First
        => this.cursor.IsEmpty ? Chars.EOI : this.cursor[0];

    /// <summary>
    /// Gets a value indicating whether the current character indicates the start of an expression.
    /// </summary>
    public readonly bool IsExpressionStart
          => Chars.IsExpressionStart(First);

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
            _ = TryConsume(first); // Will always return true
            return first;
        }
        return Chars.Invalid;
    }

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
    /// Consumes an atom from the input.
    /// </summary>
    /// <returns>An <see cref="Atom"/> representing the consumed atom.</returns>
    /// <exception cref="FormatException">Thrown when the current character is not a valid start of an atom.</exception>
    public Atom ConsumeAtom()
    {
        if (!IsExpressionStart)
            throw new FormatException($"Unexpected token: {First}");

        int pos = 0;
        while (pos < this.cursor.Length && Chars.IsAtomChar(this.cursor[pos]))
            pos++;

        Atom atom = new Atom(this.cursor.Slice(0, pos).ToString());

        this.cursor = this.cursor.Slice(atom.Symbol.Length).TrimStart();
        return atom;
    }

    /// <summary>
    /// Returns a string representation of the remaining input.
    /// </summary>
    /// <returns>A string that represents the remaining input.</returns>
    public override readonly string ToString() => $"'{string.Join("", cursor.ToArray())}'";
}
