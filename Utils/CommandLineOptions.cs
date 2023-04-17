using Mono.Options;

namespace PlasticChangelogTool.Utils
{
    internal class CommandLineOptions
    {
        internal string  WorkingDir = ".";
        internal string? Branch     = null;
        internal bool    LogsDebug;
        internal bool    LogsVerbose;
        internal bool    UseClipboard = true;

        internal static CommandLineOptions ParseCommandLineArgs()
        {
            var options = new CommandLineOptions();

            var set = new OptionSet
            {
                {
                    "dir=", "Working directory override, current folder by default", 
                    v => options.WorkingDir = v
                },

                {
                    "b|branch=", "Override branch, current branch by default",
                    v => options.Branch = v
                },

                {
                    "c", "Use clipboard instead of printing to the console. True by default.",
                    (bool v) => options.UseClipboard = v
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

            set.Parse(Environment.GetCommandLineArgs());
            return options;
        }
    }
}