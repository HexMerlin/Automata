using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core.Operations;
public static partial class Ops
{
    /// <summary>
    /// Indicates whether the languages of two deterministic finite automata overlap.
    /// The resulting value is <see langword="true"/> <c>iff</c> there exists at least one string accepted by both input automata.
    /// </summary>
    /// <param name="fsa">The first finite automaton.</param>
    /// <param name="other">The second finite automaton.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> there exists at least one string accepted by both input automata.
    /// </returns>
    public static bool Overlaps(this FsaDet fsa, FsaDet other)
        => !Intersection(fsa, other).IsEmptyLanguage;
}
