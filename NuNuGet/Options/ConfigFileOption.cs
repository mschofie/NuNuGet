namespace NuNuGet.Options;

using System.CommandLine;

internal sealed class ConfigFileOption : Option<FileInfo?>
{
    private ConfigFileOption()
        : base(name: "--configFile")
    {
        this.Description = "Path to the NuGet configuration file to use";
        this.Required = true;
    }

    public static ConfigFileOption Instance { get; } = new();
}
