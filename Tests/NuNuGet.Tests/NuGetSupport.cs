namespace NuNuGet.Tests;

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

internal static partial class NuGetSupport
{
    [GeneratedRegex(@"^(?<name>.+)\.(?<version>\d+\.\d+\.\d+(?:\.\d+)?(?:-[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*)?)$")]
    private static partial Regex PackageNameVersionRegex();

    /// <summary>
    /// Gets the package name and version from a <c>.nupkg</c> file path.
    /// </summary>
    /// <remarks>
    /// This method extracts the package name and version from the filename of the provided <c>.nupkg</c> file path,
    /// without reading the package contents. A more rigorous implementation might read the <c>.nupkg</c> file as a
    /// ZIP archive and extract the <c>.nuspec</c> file to read the package metadata, but this regex-based approach
    /// is sufficient for the test scenarios in this project and avoids unnecessary file I/O.
    /// </remarks>
    /// <param name="packagePath">The file path to a <c>.nupkg</c> file.</param>
    /// <returns>A tuple of the package name and version string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the filename does not match the expected NuGet package naming pattern.</exception>
    public static (string name, string version) GetPackageNameAndVersion(string packagePath)
    {
        string packageFileName = Path.GetFileNameWithoutExtension(packagePath);
        Match match = PackageNameVersionRegex().Match(packageFileName);
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not parse package name and version from '{packagePath}'.");
        }

        return (match.Groups["name"].Value, match.Groups["version"].Value);
    }

    /// <summary>
    /// Computes the Base64-encoded SHA-512 hash of the specified NuGet package file.
    /// </summary>
    /// <param name="packagePath">The file path to the <c>.nupkg</c> file to hash.</param>
    /// <returns>The Base64-encoded SHA-512 hash of the file contents.</returns>
    public static string GetPackageSha512Hash(string packagePath)
    {
        using FileStream stream = File.OpenRead(packagePath);
        byte[] hash = SHA512.HashData(stream);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Extracts the <c>.nuspec</c> file from the specified <c>.nupkg</c> file and writes it to the target directory
    /// with the name '&lt;lower case packagename&gt;.nuspec'.
    /// </summary>
    /// <param name="packagePath">The file path to the <c>.nupkg</c> file.</param>
    /// <param name="targetPath">The directory where the <c>.nuspec</c> file should be written.</param>
    /// <param name="packageName">The name of the package.</param>
    /// <exception cref="InvalidOperationException">Thrown when the <c>.nuspec</c> file cannot be found in the package.</exception>
    public static void ExtractNuspec(string packagePath, string targetPath, string packageName)
    {
        using ZipArchive archive = ZipFile.OpenRead(packagePath);
        ZipArchiveEntry? nuspecEntry = archive.GetEntry($"{packageName}.nuspec") ?? throw new InvalidOperationException($"Could not find {packageName}.nuspec in {packagePath}.");
        string lowerCasePackageName = packageName.ToLowerInvariant();
        string nuspecDestination = Path.Combine(targetPath, $"{lowerCasePackageName}.nuspec");
        nuspecEntry.ExtractToFile(nuspecDestination, overwrite: true);
    }
}
