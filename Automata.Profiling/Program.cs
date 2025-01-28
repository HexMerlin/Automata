using System.Diagnostics;
using System.Text;
using Automata.Core;
using Automata.Core.Alang;
using Automata.Core.Operations;

namespace Automata.Profiling;

internal class Program
{
    static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();
        string regexString = CreateLargeAlangRegex();
        AlangRegex alangRegex = AlangRegex.Parse(regexString);
      

        Mfa mfa = alangRegex.Compile();
        sw.Stop();
        Console.WriteLine($"Compiled regex in {sw.Elapsed.ToString()} ");
      
    }

    public static string CreateLargeAlangRegex()
    {
        Random random = new Random(11);

        var symbols = Enumerable.Range('a', 'h' - 'a' + 1).Select(c => ((char)c).ToString()).ToArray();

        StringBuilder sb = new StringBuilder();
        void Add(ReadOnlySpan<string> seq)
        {
            sb.Append('(');
            _ = sb.AppendJoin(" ", seq);
            var x = random.Next(5) switch
            {
                0 => ")*|",
                1 => ")+|",
                _ => ")|"
            };
            sb.Append(x);

        }

        //org: 400
        for (int seqIndex = 0; seqIndex < 80; seqIndex++)
        {
            int seqLength = random.Next(3, 12);
            string[] seqArr = Enumerable.Range(1, seqLength).Select(i => symbols[random.Next(symbols.Length)]).ToArray();
            Add(seqArr);
            Add(seqArr[..(seqLength / 3)]);
            Add(seqArr[^(seqLength / 3)..]);
        }
        sb.Remove(sb.Length - 1, 1); // remove last '|'
        return sb.ToString();
    }
}
