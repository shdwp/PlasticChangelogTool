using PlasticChangelogTool.Utils;
using System.Text;

namespace PlasticChangelogTool.Parsers
{
    internal static class ChangelogParser
    {
        internal static string Parse(IReadOnlyList<CommentTextParser.ParsedComment> comments)
        {
            var logs = new Dictionary<string, StringBuilder>();

            var fuzzySectionTerms = new Dictionary<string, string>()
            {
                { "releaselog", "ReleaseLog" },
                { "release", "ReleaseLog" },
                { "devlog", "DevLog" },
                { "debug", "DevLog" },
            };

            var ignoredComments = new List<string>()
            {
                "Merged main",
                "Merge from main",
                "Main",
            };

            foreach (var comment in comments)
            {
                if (!comment.Logs.Any())
                {
                    var successfullyIgnored = false;

                    foreach (var ignoredComment in ignoredComments)
                    {
                        if (LevenshteinDistance.Calculate(comment.Comment.ToLowerInvariant(), ignoredComment.ToLowerInvariant()) < 3)
                        {
                            successfullyIgnored = true;
                            break;
                        }
                    }

                    if (!successfullyIgnored)
                    {
                        Log.Output($"Omitting checkin with following comment: '{comment.Comment}'.");
                    }
                }
                else
                {
                    foreach (var kv in comment.Logs.SelectMany(kv => kv.Value, (kv, v) => (kv.Key, v)))
                    {
                        string section = kv.Key;
                        string sectionQuery = kv.Key.ToLowerInvariant().Trim();
                        foreach (var termKv in fuzzySectionTerms)
                        {
                            if (LevenshteinDistance.Calculate(termKv.Key, sectionQuery) < 3)
                            {
                                section = termKv.Value;
                                break;
                            }
                        }

                        if (logs.TryGetValue(section, out var sb))
                        {
                            sb.Append("\n" + kv.v.Trim());
                        }
                        else
                        {
                            logs.Add(section, new StringBuilder(kv.v.Trim()));
                        }
                    }
                }
            }

            var commentBuilder = new StringBuilder();
            for (int i = 0; i < logs.Count; i++)
            {
                string key = logs.Keys.ElementAt(i);
                var value = logs[key];

                if (i > 0)
                {
                    commentBuilder.Append('\n');
                }

                commentBuilder.AppendLine($"[{key}]:");

                if (i < logs.Count - 1)
                {
                    commentBuilder.AppendLine(value.ToString());
                }
                else
                {
                    commentBuilder.Append(value);
                }
            }

            return commentBuilder.ToString();
        }
    }
}
