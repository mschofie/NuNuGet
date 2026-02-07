namespace NuNuGet.Tests;

internal static class Build
{
    public static string GetRootPath()
    {
        return Git.GetRepositoryRoot(Path.GetDirectoryName(typeof(ScenarioTests).Assembly.Location)!);
    }

    public static string GetConfiguration()
    {
#if DEBUG
        return "Debug";
#else
        return "Release";
#endif
    }
}
