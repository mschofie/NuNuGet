namespace NuNuGet.Tests;

using NuGet.LibraryModel;
using NuGet.Versioning;
using NuNuGet.Commands;
using NuNuGet.Models;
using Xunit;

public class PackageEntryExtensionsTests
{
    [Fact]
    public void ToLibraryDependency_WithExactVersion_CreatesCorrectLibraryDependency()
    {
        PackageEntry package = new()
        {
            Id = "Newtonsoft.Json",
            Version = "[13.0.3]"
        };

        LibraryDependency result = package.ToLibraryDependency();

        Assert.NotNull(result);
        Assert.NotNull(result.LibraryRange);
        Assert.NotNull(result.LibraryRange.VersionRange);

        Assert.Equal("Newtonsoft.Json", result.LibraryRange.Name);
        Assert.Equal(LibraryDependencyTarget.Package, result.LibraryRange.TypeConstraint);

        Assert.False(result.LibraryRange.VersionRange.IsFloating);
        Assert.True(result.LibraryRange.VersionRange.HasLowerBound);
        Assert.True(result.LibraryRange.VersionRange.HasUpperBound);
        Assert.True(result.LibraryRange.VersionRange.IsMinInclusive);
        Assert.True(result.LibraryRange.VersionRange.IsMaxInclusive);
    }

    [Fact]
    public void ToLibraryDependency_WithMinimumVersion_CreatesCorrectVersionRange()
    {
        PackageEntry package = new()
        {
            Id = "AutoMapper",
            Version = "12.0.1"
        };

        LibraryDependency result = package.ToLibraryDependency();

        Assert.NotNull(result);
        Assert.NotNull(result.LibraryRange);
        Assert.NotNull(result.LibraryRange.VersionRange);

        Assert.Equal("AutoMapper", result.LibraryRange.Name);
        Assert.Equal(LibraryDependencyTarget.Package, result.LibraryRange.TypeConstraint);

        Assert.False(result.LibraryRange.VersionRange.IsFloating);
        Assert.True(result.LibraryRange.VersionRange.HasLowerBound);
        Assert.False(result.LibraryRange.VersionRange.HasUpperBound);
        Assert.True(result.LibraryRange.VersionRange.IsMinInclusive);
        Assert.False(result.LibraryRange.VersionRange.IsMaxInclusive);
    }

    [Fact]
    public void ToLibraryDependency_WithFloatingMajorVersion_CreatesCorrectVersionRange()
    {
        PackageEntry package = new()
        {
            Id = "Microsoft.Extensions.Logging",
            Version = "1.*"
        };

        LibraryDependency result = package.ToLibraryDependency();

        Assert.NotNull(result);
        Assert.NotNull(result.LibraryRange);
        Assert.NotNull(result.LibraryRange.VersionRange);

        Assert.True(result.LibraryRange.VersionRange.IsFloating);
        Assert.True(result.LibraryRange.VersionRange.HasLowerBound);
        Assert.False(result.LibraryRange.VersionRange.HasUpperBound);
        Assert.True(result.LibraryRange.VersionRange.IsMinInclusive);
        Assert.False(result.LibraryRange.VersionRange.IsMaxInclusive);
    }

    [Fact]
    public void ToLibraryDependency_WithFloatingMinorVersion_CreatesCorrectVersionRange()
    {
        PackageEntry package = new()
        {
            Id = "System.Text.Json",
            Version = "8.0.*"
        };

        LibraryDependency result = package.ToLibraryDependency();

        Assert.NotNull(result);
        Assert.NotNull(result.LibraryRange);
        Assert.NotNull(result.LibraryRange.VersionRange);

        Assert.True(result.LibraryRange.VersionRange.IsFloating);
        Assert.True(result.LibraryRange.VersionRange.HasLowerBound);
        Assert.False(result.LibraryRange.VersionRange.HasUpperBound);
        Assert.True(result.LibraryRange.VersionRange.IsMinInclusive);
        Assert.False(result.LibraryRange.VersionRange.IsMaxInclusive);
    }

    [Fact]
    public void ToLibraryDependency_WithVersionRangeInclusiveBounds_CreatesCorrectVersionRange()
    {
        PackageEntry package = new()
        {
            Id = "NuGet.Protocol",
            Version = "[6.0.0,7.0.0]"
        };

        LibraryDependency result = package.ToLibraryDependency();

        Assert.NotNull(result);
        Assert.NotNull(result.LibraryRange);
        Assert.NotNull(result.LibraryRange.VersionRange);

        Assert.False(result.LibraryRange.VersionRange.IsFloating);
        Assert.True(result.LibraryRange.VersionRange.HasLowerBound);
        Assert.True(result.LibraryRange.VersionRange.HasUpperBound);
        Assert.True(result.LibraryRange.VersionRange.IsMinInclusive);
        Assert.True(result.LibraryRange.VersionRange.IsMaxInclusive);
    }

    [Fact]
    public void ToLibraryDependency_WithVersionRangeExclusiveUpperBound_CreatesCorrectVersionRange()
    {
        PackageEntry package = new()
        {
            Id = "Castle.Core",
            Version = "[1.0.0,2.0.0)"
        };

        LibraryDependency result = package.ToLibraryDependency();

        Assert.NotNull(result);
        Assert.NotNull(result.LibraryRange);
        Assert.NotNull(result.LibraryRange.VersionRange);

        Assert.False(result.LibraryRange.VersionRange.IsFloating);
        Assert.True(result.LibraryRange.VersionRange.HasLowerBound);
        Assert.True(result.LibraryRange.VersionRange.HasUpperBound);
        Assert.True(result.LibraryRange.VersionRange.IsMinInclusive);
        Assert.False(result.LibraryRange.VersionRange.IsMaxInclusive);
    }

    [Fact]
    public void ToLibraryDependency_WithPrereleaseVersion_CreatesCorrectVersionRange()
    {
        PackageEntry package = new()
        {
            Id = "Experimental.Package",
            Version = "1.0.0-beta.1"
        };

        LibraryDependency result = package.ToLibraryDependency();

        Assert.NotNull(result);
        Assert.NotNull(result.LibraryRange);
        Assert.Equal("Experimental.Package", result.LibraryRange.Name);
        Assert.Equal(VersionRange.Parse("1.0.0-beta.1"), result.LibraryRange.VersionRange);
        Assert.Equal(LibraryDependencyTarget.Package, result.LibraryRange.TypeConstraint);
    }

    [Fact]
    public void ToLibraryDependency_WithInvalidVersionString_ThrowsException()
    {
        PackageEntry package = new()
        {
            Id = "InvalidPackage",
            Version = "[13.0.0"
        };

        _ = Assert.Throws<ArgumentException>(package.ToLibraryDependency);
    }
}
