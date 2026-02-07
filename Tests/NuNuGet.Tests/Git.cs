namespace NuNuGet.Tests;

using System;

public static class Git
{
    /// <summary>
    /// Gets the repository root.
    /// </summary>
    /// <param name="fromPath">The path to retrieve the repository root from.</param>
    /// <returns>The path to the root of the repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the repository root cannot be determined.</exception>
    public static string GetRepositoryRoot(string fromPath)
    {
        ProcessResult result = ProcessManagement.RunProcess("git", $"-C {fromPath} rev-parse --show-toplevel", fromPath);

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException($"Unable to determine the repository root: {result.StandardOutput}\n {result.StandardError}\n");
        }

        return result.StandardOutput.TrimEnd('\r', '\n');
    }
}
