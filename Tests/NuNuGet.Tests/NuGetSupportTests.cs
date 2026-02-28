namespace NuNuGet.Tests;

using System;
using System.IO;
using Xunit;

public class NuGetSupportTests
{
    [Fact]
    public void ParsePackageNameAndVersion_SimpleThreePartVersion()
    {
        (string name, string version) = NuGetSupport.GetPackageNameAndVersion("MyPackage.1.0.0.nupkg");

        Assert.Equal("MyPackage", name);
        Assert.Equal("1.0.0", version);
    }

    [Fact]
    public void ParsePackageNameAndVersion_LargeVersionNumbers()
    {
        (string name, string version) = NuGetSupport.GetPackageNameAndVersion("MyPackage.10.200.3000.nupkg");

        Assert.Equal("MyPackage", name);
        Assert.Equal("10.200.3000", version);
    }

    [Fact]
    public void ParsePackageNameAndVersion_DottedPackageName()
    {
        (string name, string version) = NuGetSupport.GetPackageNameAndVersion("System.Text.Json.9.0.0.nupkg");

        Assert.Equal("System.Text.Json", name);
        Assert.Equal("9.0.0", version);
    }

    [Fact]
    public void ParsePackageNameAndVersion_PrereleaseVersion()
    {
        (string name, string version) = NuGetSupport.GetPackageNameAndVersion("MyPackage.1.0.0-beta.1.nupkg");

        Assert.Equal("MyPackage", name);
        Assert.Equal("1.0.0-beta.1", version);
    }

    [Fact]
    public void ParsePackageNameAndVersion_DottedNameWithPrerelease()
    {
        (string name, string version) = NuGetSupport.GetPackageNameAndVersion("My.Complex.Package.2.3.4-rc.2.nupkg");

        Assert.Equal("My.Complex.Package", name);
        Assert.Equal("2.3.4-rc.2", version);
    }

    [Fact]
    public void ParsePackageNameAndVersion_MultiSegmentPrerelease()
    {
        (string name, string version) = NuGetSupport.GetPackageNameAndVersion("MyPackage.1.0.0-beta.2.3.nupkg");

        Assert.Equal("MyPackage", name);
        Assert.Equal("1.0.0-beta.2.3", version);
    }

    [Fact]
    public void ParsePackageNameAndVersion_FullPathIsHandled()
    {
        (string name, string version) = NuGetSupport.GetPackageNameAndVersion(Path.Combine("C:", "packages", "Newtonsoft.Json.13.0.3.nupkg"));

        Assert.Equal("Newtonsoft.Json", name);
        Assert.Equal("13.0.3", version);
    }

    [Fact]
    public void ParsePackageNameAndVersion_NoVersionThrows()
    {
        _ = Assert.Throws<InvalidOperationException>(() =>
            NuGetSupport.GetPackageNameAndVersion("NoVersion.nupkg"));
    }

    [Fact]
    public void ParsePackageNameAndVersion_EmptyStringThrows()
    {
        _ = Assert.Throws<InvalidOperationException>(() =>
            NuGetSupport.GetPackageNameAndVersion(".nupkg"));
    }

    [Fact]
    public void GetPackageSha512Hash_ReturnsConsistentHash()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "test content for hashing");

            string hash1 = NuGetSupport.GetPackageSha512Hash(tempFile);
            string hash2 = NuGetSupport.GetPackageSha512Hash(tempFile);

            Assert.NotEmpty(hash1);
            Assert.Equal(hash1, hash2);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void GetPackageSha512Hash_DifferentContentProducesDifferentHash()
    {
        string tempFile1 = Path.GetTempFileName();
        string tempFile2 = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile1, "content A");
            File.WriteAllText(tempFile2, "content B");

            string hash1 = NuGetSupport.GetPackageSha512Hash(tempFile1);
            string hash2 = NuGetSupport.GetPackageSha512Hash(tempFile2);

            Assert.NotEqual(hash1, hash2);
        }
        finally
        {
            File.Delete(tempFile1);
            File.Delete(tempFile2);
        }
    }
}
