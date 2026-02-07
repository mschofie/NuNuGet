namespace NuNuGet.Tests;

using System;

using static NuNuGet.Tests.Helper;

internal sealed class NuGetCli
{
    private const string ExecutableName = "nuget.exe";

    private string ExecutablePath { get; }

    private NuGetCli(string path)
    {
        this.ExecutablePath = path;
    }

    public void Add(string packagePath, string source)
    {
        ProcessResult result = ProcessManagement.RunProcess(this.ExecutablePath, $"add {packagePath} -Source {source}", Environment.CurrentDirectory);
        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException($"Failed to add package: {result.StandardOutput}\n {result.StandardError}\n");
        }
    }

    public static NuGetCli Create()
    {
        string path = FindOnPath(ExecutableName) ?? throw new InvalidOperationException($"Could not find {ExecutableName} on the system PATH.");
        return new NuGetCli(path);
    }
}
