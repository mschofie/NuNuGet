namespace NuNuGet.Options;

using System.CommandLine;

internal sealed class VerboseOption : Option<bool>
{
    private VerboseOption()
        : base(name: "--verbose")
    {
        this.Description = "Enable verbose output";
        this.Required = false;
    }

    public static VerboseOption Instance { get; } = new();
}
