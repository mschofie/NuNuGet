namespace NuNuGet.Options;

using System.CommandLine;

internal sealed class ListFileOption : Option<FileInfo?>
{
    private ListFileOption()
        : base(name: "--listFile")
    {
        this.Description = "Path to the packages.list.json file to restore";
        this.Required = true;
    }

    public static ListFileOption Instance { get; } = new();
}
