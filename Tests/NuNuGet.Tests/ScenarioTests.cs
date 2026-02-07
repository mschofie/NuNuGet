namespace NuNuGet.Tests;

using NuNuGet.Models;
using System.IO;
using Xunit;

using static NuNuGet.Tests.Helper;

public class ScenarioTests
{
    private string ReferenceFolder { get; }

    private string OutputFolder { get; }

    private string NuNuGetPath { get; }

    private string RepositoryRoot { get; }

    private string BuiltPackagesFolder { get; }

    private string ConfigFilePath { get; }

    public ScenarioTests()
    {
        this.OutputFolder = Path.GetDirectoryName(typeof(ScenarioTests).Assembly.Location)!;
        this.RepositoryRoot = Git.GetRepositoryRoot(this.OutputFolder);
        this.ReferenceFolder = Path.Combine(this.RepositoryRoot, "Tests", "NuNuGet.Tests", "Reference");
        this.NuNuGetPath = Path.Combine(this.OutputFolder, "NuNuGet.exe");
        this.BuiltPackagesFolder = Path.Combine(this.RepositoryRoot, "__artifacts", "package", Build.GetConfiguration().ToLowerInvariant());

        this.ConfigFilePath = Path.Combine(this.ReferenceFolder, "nuget.config");

        Assert.True(File.Exists(this.NuNuGetPath), $"Expected NuNuGet.exe to be present in the working folder: {this.OutputFolder}");
    }

    private ProcessResult RunNuNuGet(params string[] args)
    {
        return ProcessManagement.RunProcess(this.NuNuGetPath, string.Join(' ', args), this.OutputFolder);
    }

    [Fact]
    public void InvocationErrorsWithNoParameters()
    {
        ProcessResult processResult = this.RunNuNuGet();

        Assert.NotEqual(0, processResult.ExitCode);
    }

    [Fact]
    public void InvocationSucceedsWithHelp()
    {
        ProcessResult processResult = this.RunNuNuGet("--help");

        Assert.Equal(0, processResult.ExitCode);
    }

    [Fact]
    public void InvocationErrorsWithOnlyInstall()
    {
        ProcessResult processResult = this.RunNuNuGet("install");

        Assert.NotEqual(0, processResult.ExitCode);
    }

    [Fact]
    public void InvocationErrorsWithConfigFile()
    {
        ProcessResult processResult = this.RunNuNuGet("install", "--configFile", this.ConfigFilePath);

        Assert.NotEqual(0, processResult.ExitCode);
    }

    [Fact]
    public void EndToEndScenario()
    {
        NuGetCli nuGetCli = NuGetCli.Create();

        string nugetConfigPath = Path.Combine(this.ReferenceFolder, "nuget.config");
        string packagesListPath = Path.Combine(this.ReferenceFolder, "packages.list.json");
        string packagesLockPath = Path.Combine(this.ReferenceFolder, "packages.lock.json");
        string packageSourcePath = Path.Combine(this.ReferenceFolder, "__packages");

        string package050 = Path.Combine(this.BuiltPackagesFolder, "NuNuGet.Reference.0.5.0.nupkg");
        string package060 = Path.Combine(this.BuiltPackagesFolder, "NuNuGet.Reference.0.6.0.nupkg");

        // Clean state:
        //  - Delete the 'packages.lock.json'
        //  - Create the 'nuget.config' file, and add the local package source to it.
        //  - Create the 'packages.list.json' file, and add NuNuGet.Reference.0.5 to it.
        //  - Delete the 'packageSourcePath', recreate it, and add NuNuGet.Reference.0.5 to it.
        DeleteFile(packagesLockPath);
        WriteFile(nugetConfigPath, $$"""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <packageSources>
                    <clear />
                    <add key="store" value="{{packageSourcePath}}" />
                </packageSources>
            </configuration>
            """);
        WriteObject(packagesListPath, new PackageList
        {
            TargetFramework = "net10.0",
            Packages =
            [
                new PackageEntry { Id = "NuNuGet.Reference", Version = "0.5.0" }
            ]
        });
        RemoveFolder(packageSourcePath);
        CreateFolder(packageSourcePath);
        nuGetCli.Add(package050, packageSourcePath);

        // Act: Run NuNuGet.exe to generate the lock file.
        {
            ProcessResult processResult = this.RunNuNuGet("install", "--configFile", nugetConfigPath, "--lockFile", packagesLockPath, "--packageList", packagesListPath);

            Assert.Equal(0, processResult.ExitCode);
            Assert.Contains("NuNuGet.Reference/0.5.0", processResult.StandardOutput);
        }

        // Re-run with the same parameters, check for success and the same output.
        {
            ProcessResult processResult = this.RunNuNuGet("install", "--configFile", nugetConfigPath, "--lockFile", packagesLockPath, "--packageList", packagesListPath);

            Assert.Equal(0, processResult.ExitCode);
            Assert.Contains("NuNuGet.Reference/0.5.0", processResult.StandardOutput);
        }

        // Add the 0.6.0 package to the package source, the 0.5.0 package should still be used as it's already in the lock file.
        {
            nuGetCli.Add(package060, packageSourcePath);

            ProcessResult processResult = this.RunNuNuGet("install", "--configFile", nugetConfigPath, "--lockFile", packagesLockPath, "--packageList", packagesListPath);

            Assert.Equal(0, processResult.ExitCode);
            Assert.Contains("NuNuGet.Reference/0.5.0", processResult.StandardOutput);
        }

        // Touch the packages.list.json, the 0.5.0 package should still be used as it's already in the lock file.
        {
            Touch(packagesListPath);

            ProcessResult processResult = this.RunNuNuGet("install", "--configFile", nugetConfigPath, "--lockFile", packagesLockPath, "--packageList", packagesListPath);

            Assert.Equal(0, processResult.ExitCode);
            Assert.Contains("NuNuGet.Reference/0.5.0", processResult.StandardOutput);
        }

        // Update the packages.list.json to reference 0.6.0, 'install' should fail.
        {
            WriteObject(packagesListPath, new PackageList
            {
                TargetFramework = "net10.0",
                Packages =
                [
                    new PackageEntry { Id = "NuNuGet.Reference", Version = "0.6.0" }
                ]
            });

            ProcessResult processResult = this.RunNuNuGet("install", "--configFile", nugetConfigPath, "--lockFile", packagesLockPath, "--packageList", packagesListPath);

            Assert.Equal(100, processResult.ExitCode);
        }

        // Delete the lock file, re-run 'install' and 0.6.0 should be picked-up.
        {
            DeleteFile(packagesLockPath);

            ProcessResult processResult = this.RunNuNuGet("install", "--configFile", nugetConfigPath, "--lockFile", packagesLockPath, "--packageList", packagesListPath);

            Assert.Equal(0, processResult.ExitCode);
            Assert.Contains("NuNuGet.Reference/0.6.0", processResult.StandardOutput);
        }
    }
}
