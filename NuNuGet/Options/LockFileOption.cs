namespace NuNuGet.Options;

using System.CommandLine;

internal sealed class LockFileOption : Option<string>
{
    private LockFileOption()
        : base(name: "--lockFile")
    {
        this.Description = "Path to the NuGet lock file to use";
        this.Required = true;
    }

    public static LockFileOption Instance { get; } = new();
}
