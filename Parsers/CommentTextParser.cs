using PlasticChangelogTool.Utils;

namespace PlasticChangelogTool.Parsers
{
    internal static class CommentTextParser
    {
        internal struct ParsedComment
        {
            internal string Comment;
            internal IReadOnlyDictionary<string, List<string>> Logs;
        }

        private struct ParserState
        {
            internal bool BOL;
            internal bool EOL;

            internal bool InHeader;
            internal bool InSection;
            internal string CurrentHeaderValue;
            internal string CurrentValue;

            public ParserState()
            {
                BOL = true;
                EOL = false;
                InHeader = false;
                InSection = false;
                CurrentHeaderValue = "";
                CurrentValue = "";
            }

            public override string ToString()
            {
                return base.ToString() +
                    $" BOL: {BOL} EOL: {EOL} InHeader: {InHeader} InSection: {InSection} CurrentValue: {CurrentValue}";
            }
        }

        internal static bool Parse(string text, out ParsedComment result)
        {
            Log.Verbose($"CommentTextParser parsing: '{text}'");
            var logs = new Dictionary<string, List<string>>();
            string comment = "";

            using var reader = new StringReader(text);
            var state = new ParserState();
            int ord;
            while ((ord = reader.Read()) != -1)
            {
                char current = (char)ord;
                if (current == '\n')
                {
                    state.BOL = true;
                    continue;
                }

                state.EOL = false;
                int next = reader.Peek();
                if (next == '\n' || next == -1)
                {
                    state.EOL = true;
                }

                if (current == '[' && state.BOL)
                {
                    state.InHeader = true;
                    continue;
                }

                if (current == ']' && state.InHeader)
                {
                    state.InHeader = false;
                    state.InSection = true;
                    state.CurrentHeaderValue = state.CurrentValue.Trim();
                    state.CurrentValue = "";
                    Log.Verbose($"CommentTextParser HEADER: {state.CurrentHeaderValue}");
                    continue;
                }

                state.CurrentValue += current;

                if (current == ']' || state.EOL && state.CurrentValue.Trim().Length > 0)
                {
                    if (state.InSection && !state.InHeader)
                    {
                        string key = state.CurrentHeaderValue;
                        var list = logs.GetValueOrDefault(key) ?? new();
                        list.Add(state.CurrentValue.Trim());
                        logs[key] = list;

                        Log.Verbose($"CommentTextParser LOG: {key} {state.CurrentValue.Trim()}");
                        state.CurrentValue = "";
                    }
                    else if (!state.InHeader)
                    {
                        comment = state.CurrentValue.Trim();
                        Log.Verbose($"CommentTextParses COMMENT: {comment}");
                        state.CurrentValue = "";
                    }
                    else if (state.InHeader)
                    {
                        state.CurrentHeaderValue = state.CurrentValue.Trim();
                        state.InHeader = false;
                        state.CurrentValue = "";
                    }
                }

                state.BOL = false;
            }

            result.Comment = comment;
            result.Logs = logs;
            return true;
        }
    }
}
