using Mono.Options;

namespace PlasticChangelogTool.Utils
{
    internal class CommandLineOptions
    {
        internal string WorkingDir = ".";
        internal string CheckinQueryAppendix = "";
        internal string? Branch = null;
        internal bool LogsDebug;
        internal bool LogsVerbose;
        internal bool PrintHelp = false;
        internal bool PrintToConsole = false;

        internal static CommandLineOptions ParseCommandLineArgs()
        {
            var options = new CommandLineOptions();

            var set = new OptionSet
            {
                {
                    "h", "Print help", _ => options.PrintHelp = true
                },

                {
                    "dir=", "Working directory override, current folder by default",
                    v => options.WorkingDir = v
                },

                {
                    "b|branch=", "Override branch, current branch by default",
                    v => options.Branch = v
                },

                {
                    "c", "Do not use clipboard and print to console instead.",
                    _ => options.PrintToConsole = true
                },

                {
                    "d", "Enable debug logs",
                    _ => options.LogsDebug = true
                },

                {
                    "v", "Enable verbose logs",
                    _ => options.LogsVerbose = true
                },
            };

            var appendixes = set.Parse(Environment.GetCommandLineArgs());
            options.CheckinQueryAppendix = "";
            foreach (string? appendix in appendixes.Skip(1))
            {
                options.CheckinQueryAppendix += " " + appendix;
            }

            if (options.PrintHelp)
            {
                Console.WriteLine($"plasticChangelogTool [ADDITIONAL QUERY CLAUSES] [OPTIONS].\n" +
                    $"You can specify additional query clauses that will be added after 'where' part in the 'cm find' query.\n" +
                    $"For example, you can only process checkins after a specific date or changeset by adding \"date > '2021-01-01'\" or \"changesetid > 1000\" to the arguments.\n");
                set.WriteOptionDescriptions(Console.Out);
            }

            return options;
        }
    }
}
