using System.Threading.Tasks.Sources;

namespace VirtualFolders.Core.Extensions;

internal sealed class VisualStudioResolver : IProgramResolver
{
    private static readonly Dictionary<string, string> versionTable = new()
    {
        { "18", "2026" },
        { "17", "2022" },
        { "16", "2019" },
        { "15", "2017" },
        { "14", "2015" },
        { "12", "2013" },
        { "11", "2012" },
        { "10", "2010" },
        { "9",  "2008" },
        { "8",  "2005" }
    };

    public IEnumerable<ProgramInformation> Resolve(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            var codePath = Path.Combine(path, "code.cmd");

            if (File.Exists(codePath))
            {
                yield return new ProgramInformation
                {
                    ProgramPath = codePath,
                    DisplayName = "Visual Studio Code"
                };
            }
        }

        var vsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Visual Studio");
        if (Directory.Exists(vsFolder))
        {
            foreach (var version in Directory.GetDirectories(vsFolder))
            {
                foreach (var edition in Directory.GetDirectories(version))
                {
                    var devenvPath = Path.Combine(edition, "Common7", "IDE", "devenv.exe");
                    if (File.Exists(devenvPath))
                    {
                        if (!versionTable.TryGetValue(Path.GetFileName(version), out var displayVersion))
                        {
                            displayVersion = Path.GetFileName(version);
                        }

                        yield return new ProgramInformation
                        {
                            ProgramPath = devenvPath,
                            DisplayName = $"Visual Studio {Path.GetFileName(displayVersion)} {Path.GetFileName(edition)}"
                        };
                    }
                }
            }
        }
    }
}
