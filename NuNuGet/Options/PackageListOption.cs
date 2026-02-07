namespace NuNuGet.Options;

using System.CommandLine;

internal sealed class PackageListOption : Option<FileInfo?>
{
    private PackageListOption()
        : base(name: "--packageList")
    {
        this.Description = "Path to the package.list.json file to restore";
        this.Required = true;
    }

    public static PackageListOption Instance { get; } = new();
}
