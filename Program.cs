using PlasticChangelogTool.Parsers;
using PlasticChangelogTool.PlasticApi;
using PlasticChangelogTool.Utils;
using System.Diagnostics;

try
{
    var opts = CommandLineOptions.ParseCommandLineArgs();
    Log.enableDebug = opts.LogsDebug;
    Log.enableVerbose = opts.LogsVerbose;

    if (opts.PrintHelp)
    {
        return;
    }

    var cli = new PlasticCliApi(opts.WorkingDir);

    string? branch = opts.Branch;
    if (branch == null)
    {
        branch = cli.GetCurrentBranch();
    }

    var stringComments = cli.GetCommentsFromBranch(branch, opts.CheckinQueryAppendix);
    var parsedComments = new List<CommentTextParser.ParsedComment>(stringComments.Count);

    foreach (string comment in stringComments)
    {
        if (CommentTextParser.Parse(comment, out var result))
        {
            parsedComments.Add(result);
        }
        else
        {
            Log.Error($"Failed to parse comment: '{comment}'");
        }
    }

    string text = ChangelogParser.Parse(parsedComments);
    if (!opts.PrintToConsole)
    {
        TextCopy.ClipboardService.SetText(text);
        Log.Output($"Processed total: {parsedComments.Count}, changelog copied to clipboard.");
    }
    else
    {
        Log.Output(text);
    }
}
catch (Exception e)
{
    Log.Error(e.Message);

    if (Debugger.IsAttached)
    {
        throw;
    }
}
