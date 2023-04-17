using PlasticChangelogTool.Parsers;
using PlasticChangelogTool.PlasticApi;
using PlasticChangelogTool.Utils;

try
{
    // string branch = cli.GetCurrentBranch();

    var cli = new PlasticCliApi(@".");
    string branch = "/main/Milestone4";
    var stringComments = cli.GetCommentsFromBranch(branch);
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
    Console.WriteLine($"'{text}'");

    TextCopy.ClipboardService.SetText(text);
    Log.Output("Changelog copied to clipboard.");
}
catch (Exception e)
{
    Log.Error(e.Message);
    throw;
}
