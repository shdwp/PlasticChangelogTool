namespace PlasticChangelogTool.Utils
{
    internal static class Log
    {
        internal static bool enableDebug = true;
        internal static bool enableVerbose = true;

        internal static void Error(string line)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[!] " + line);
            Console.ResetColor();
        }

        internal static void Warning(string line)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[w] " + line);
            Console.ResetColor();
        }

        internal static void Output(string line)
        {
            Console.WriteLine(line);
        }

        internal static void Debug(string line)
        {
            if (!enableDebug)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("[d] " + line);
            Console.ResetColor();
        }

        internal static void Verbose(string line)
        {
            if (!enableVerbose)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[v] " + Sanitize(line));
            Console.ResetColor();
        }

        private static string Sanitize(string input)
        {
            return input.Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
}