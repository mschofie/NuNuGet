namespace NuNuGet.Tests;

using NuGet.Packaging.Core;
using NuGet.ProjectModel;
using NuGet.Versioning;
using System.Collections.Generic;
using Xunit;

public class TraversalTests
{
    [Fact]
    public void Empty()
    {
        IEnumerable<(string, NuGetVersion)> result = Traversal.ReverseTopological(new PackagesLockFile
        {
            Targets = [new PackagesLockFileTarget
            {
                Dependencies = []
            }]
        });

        Assert.Empty(result);
    }

    [Fact]
    public void SingleElement()
    {
        IEnumerable<(string, NuGetVersion)> result = Traversal.ReverseTopological(new PackagesLockFile
        {
            Targets = [
                new PackagesLockFileTarget
                {
                    Dependencies =
                    [
                        new LockFileDependency
                        {
                            Id = "A",
                            ResolvedVersion = NuGetVersion.Parse("1.0.0"),
                        }
                    ]
                }]
        });

        Assert.Equal(result, [("A", NuGetVersion.Parse("1.0.0"))]);
    }

    [Fact]
    public void TwoDependentElements()
    {
        IEnumerable<(string, NuGetVersion)> result = Traversal.ReverseTopological(new PackagesLockFile
        {
            Targets = [
                new PackagesLockFileTarget
                {
                    Dependencies =
                    [
                        new LockFileDependency
                        {
                            Id = "A",
                            ResolvedVersion = NuGetVersion.Parse("1.0.0"),
                            Dependencies =
                            [
                                new PackageDependency("B", VersionRange.Parse("2.0.0"))
                            ]
                        },
                        new LockFileDependency
                        {
                            Id = "B",
                            ResolvedVersion = NuGetVersion.Parse("2.0.0")
                        },

                    ]
                }]
        });

        Assert.Equal(result, [
            ("B", NuGetVersion.Parse("2.0.0")),
            ("A", NuGetVersion.Parse("1.0.0")),
        ]);
    }

    [Fact]
    public void TwoIndependentElements()
    {
        IEnumerable<(string, NuGetVersion)> result = Traversal.ReverseTopological(new PackagesLockFile
        {
            Targets = [new PackagesLockFileTarget
            {
                Dependencies =
                [
                    new LockFileDependency
                    {
                        Id = "A",
                        ResolvedVersion = NuGetVersion.Parse("1.0.0"),
                    },
                    new LockFileDependency
                    {
                        Id = "B",
                        ResolvedVersion = NuGetVersion.Parse("2.0.0")
                    },

                ]
            }]
        });

        Assert.Equivalent(result, new List<(string, NuGetVersion)>
        {
            ("B", NuGetVersion.Parse("2.0.0")),
            ("A", NuGetVersion.Parse("1.0.0")),
        });
    }

    [Fact]
    public void ModerateGraph()
    {
        IEnumerable<(string, NuGetVersion)> result = Traversal.ReverseTopological(new PackagesLockFile
        {
            Targets = [new PackagesLockFileTarget
            {
                Dependencies =
                [
                    new LockFileDependency
                    {
                        Id = "B",
                        ResolvedVersion = NuGetVersion.Parse("2.0.0")
                    },
                    new LockFileDependency
                    {
                        Id = "A",
                        ResolvedVersion = NuGetVersion.Parse("1.0.0"),
                        Dependencies =
                        [
                            new PackageDependency("B", VersionRange.Parse("2.0.0")),
                            new PackageDependency("C", VersionRange.Parse("3.0.0")),
                        ]
                    },
                    new LockFileDependency
                    {
                        Id = "C",
                        ResolvedVersion = NuGetVersion.Parse("3.0.0"),
                        Dependencies =
                        [
                            new PackageDependency("D", VersionRange.Parse("4.0.0"))
                        ]
                    },
                    new LockFileDependency
                    {
                        Id = "D",
                        ResolvedVersion = NuGetVersion.Parse("4.0.0")
                    },
                ]
            }]
        });

        List<(string, NuGetVersion)> resultList = [.. result];

        int indexOfA = resultList.IndexOf(("A", NuGetVersion.Parse("1.0.0")));
        int indexOfB = resultList.IndexOf(("B", NuGetVersion.Parse("2.0.0")));
        int indexOfC = resultList.IndexOf(("C", NuGetVersion.Parse("3.0.0")));
        int indexOfD = resultList.IndexOf(("D", NuGetVersion.Parse("4.0.0")));

        Assert.True(indexOfD < indexOfC, "D should come before C");
        Assert.True(indexOfC < indexOfA, "C should come before A");
        Assert.True(indexOfB < indexOfA, "B should come before A");
    }

    [Fact]
    public void CaseInsensitivePackageIds()
    {
        // Dependency references "b" in lowercase, but the package is declared as "B".
        IEnumerable<(string, NuGetVersion)> result = Traversal.ReverseTopological(new PackagesLockFile
        {
            Targets = [
                new PackagesLockFileTarget
                {
                    Dependencies =
                    [
                        new LockFileDependency
                        {
                            Id = "A",
                            ResolvedVersion = NuGetVersion.Parse("1.0.0"),
                            Dependencies =
                            [
                                new PackageDependency("b", VersionRange.Parse("2.0.0"))
                            ]
                        },
                        new LockFileDependency
                        {
                            Id = "B",
                            ResolvedVersion = NuGetVersion.Parse("2.0.0")
                        },
                    ]
                }]
        });

        Assert.Equal(result, [
            ("B", NuGetVersion.Parse("2.0.0")),
            ("A", NuGetVersion.Parse("1.0.0")),
        ]);
    }
}
