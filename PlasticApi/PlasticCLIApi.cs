using PlasticChangelogTool.Utils;
using System.Diagnostics;
using Tokens;

namespace PlasticChangelogTool.PlasticApi
{
    internal sealed class PlasticCliApi
    {
        private string _workingFolder;
        private string _permanentArgs;

        internal PlasticCliApi(string workingFolder)
        {
            _workingFolder = workingFolder;
            _permanentArgs = "";
        }

        internal string GetCurrentCS()
        {
            string output = RunCommand("status --head --machinereadable");
            var result = new Tokenizer().Tokenize("STATUS {cs} {repo} {server}", output);
            return result.First<string>("cs");
        }

        internal string GetCurrentBranch()
        {
            string cs = GetCurrentCS();
            string branch = RunCommand($"log {cs} --csformat={{branch}}");
            return branch.Trim();
        }

        internal IReadOnlyList<string> GetCommentsFromBranch(string? branch)
        {
            var tokenizer = new Tokenizer();
            string comments = RunCommand($"find changeset \"where branch = '{branch}'\" --format=<<<{{comment}}");
            var parsedResult = tokenizer.Tokenize("<<<{cs*}\n\nTotal: {total}\n", comments);
            var result = new List<string>(48);

            int total = int.Parse(parsedResult.FirstOrDefault<string>("total"));
            Log.Debug($"Total from plastic: {total}.");

            foreach (object? cs in parsedResult.All("cs"))
            {
                string? value = cs.ToString();
                Log.Verbose($"Matched comment: '{value}'.");
                result.Add(value);
            }

            if (result.Count != total)
            {
                Log.Warning($"Mismatch between Plastic total {total} and parsed total {result.Count}.");
            }

            return result;
        }

        private string RunCommand(string args)
        {
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "cm",
                    WorkingDirectory = _workingFolder,
                    Arguments = args + _permanentArgs,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                }
            };

            Log.Debug($"Running: cm {args}.");

            p.Start();
            p.WaitForExit();

            string error = p.StandardError.ReadToEnd();
            if (error.Length > 0)
            {
                throw new Exception($"{error} (command 'cm {args}')");
            }

            string output = p.StandardOutput.ReadToEnd();
            Log.Verbose($"Result: '{output}'.");
            return output;
        }
    }
}