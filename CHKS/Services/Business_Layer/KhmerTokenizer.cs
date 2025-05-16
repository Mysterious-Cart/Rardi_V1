// Purpose: Tokenize Khmer text and calculate similarity between two strings.
using Python.Runtime;

namespace CHKS.Services
{
    public class KhmerTokenizer : IDisposable
    {
        private static dynamic InstantiateTokenizer()
        {
            if (!PythonEngine.IsInitialized) PythonEngine.Initialize();
            using (Py.GIL())
            {
                dynamic tokenizer = Py.Import("khmernltk");
                return tokenizer;
            }
        }

        public List<string> Tokenize(string input, CancellationToken token = default)
        {
            if (!PythonEngine.IsInitialized)
            {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
            }
            List<string> tokenizedText = [];
            using (Py.GIL())
            {
                dynamic tokenizer = InstantiateTokenizer();
                PyList result = tokenizer.word_tokenize(CleanString(input));
                foreach(PyObject i in result)
                {
                    tokenizedText.Add(i.ToString());
                }
            }
            return tokenizedText;
        }
        public async Task<double> CalculateTokenSimilarity(string a, string b)
        {
            a = CleanString(a);
            b = CleanString(b);

            if (a == b) return 1.0;
            if (a.Length == 0 || b.Length == 0) return 0.0;

            int maxLen = Math.Max(a.Length, b.Length);

            // Get the similarity between two strings
            int distance = LevenshteinDistance(a, b);
            return 1.0 - (double)distance / maxLen;
        }
        private static int LevenshteinDistance(string s, string t)
        {
            
            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int j = 1; j <= t.Length; j++)
                for (int i = 1; i <= s.Length; i++)
                    d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,
                        d[i, j - 1] + 1),
                        d[i - 1, j - 1] + (s[i - 1] == t[j - 1] ? 0 : 1));

            return d[s.Length, t.Length];
        }

        private static string CleanString(string text)
        {
            string clean_string = text
                .Normalize()
                .Trim()
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("\t", "")
                .Replace("\v", "")
                .Replace("\f", "")
                .Replace("\b", "")
                .Replace("\a", "")
                .Replace("\0", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("/", "")
                .Replace("\"", "")
                .Replace('-', ' ')
                .Replace("\u0000", "")
                .Replace("\u0001", "")
                .Replace("\u0002", "")
                .Replace("\u0003", "")
                .Replace("\u0004", "")
                .Replace("\u0005", "")
                .Replace("\u0006", "")
                .Replace("\u0007", "")
                .Replace("\u0008", "")
                .Replace("\u0009", "")
                .Replace("\u000A", "")
                .Replace("\u000B", "")
                .Replace("\u000C", "")
                .Replace("\u000D", "")
                .Replace("\u000E", "")
                .Replace("\u000F", "")
                .Replace("\u0010", "")
                .Replace("\u0011", "")
                .Replace("\u0012", "")
                .Replace("\u0013", "")
                .Replace("\u0014", "")
                .Replace("\u0015", "")
                .Replace("\u0016", "")
                .Replace("\u0017", "")
                .Replace("\u0018", "")
                .Replace("\u0019", "")
                .Replace("\u001A", "")
                .Replace("\u001B", "")
                .Replace("\u001C", "")
                .Replace("\u001D", "")
                .Replace("\u001E", "")
                .Replace("\u001F", "")
                .ToLowerInvariant()
                .Replace(" ", "");

            return clean_string;
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing");
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        // Additional methods and properties can be added here
    }
}