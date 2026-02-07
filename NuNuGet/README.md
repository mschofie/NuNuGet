# NuNuGet

NuNuGet is a small command-line NuGet client that installs packages listed in a simple JSON `packages.list` file into the NuGet global packages folder. It uses a NuGet lock file to ensure repeatable restores and writes out the resolved package graph.

## Features

- **Install from JSON**: Install packages described in a `package.list.json` file.
- **Repeatable restores**: Uses a NuGet lock file (`packages.lock.json`) to ensure deterministic restores.
- **Output metadata**: Writes the `GlobalPackagesPath` and a reverse-topological install order to standard output.
- **Verbose logging**: Optional `--verbose` flag to enable detailed logs.

## Usage

```powershell
NuNuGet -- install \
        --configFile ./nuget.config \
        --lockFile ./packages.lock.json \
        --packageList ./packages.list.json [--verbose]
```

Command-line options:

- `--configFile`: Path to the NuGet configuration file to use (required).
- `--lockFile`: Path to the NuGet lock file to use (required).
- `--packageList`: Path to the `package.list.json` file describing packages to restore (required).
- `--verbose`: Enable verbose output (optional).

## Example `package.list.json`

```json
{
    "targetFramework": "net8.0",
    "packages": [
        { "id": "Newtonsoft.Json", "version": "13.0.3" },
        { "id": "Humanizer", "version": "2.14.0" }
    ]
}
```

The `PackageList` schema has two properties: `targetFramework` (string) and `packages` (array of objects with `id` and `version`). Versions may be floating or range specifiers as accepted by NuGet.

## Output and exit codes

- **0**: Success. Writes `GlobalPackagesPath:` and `Package: id/version` lines to stdout.
- **1**: General error (exceptions or missing arguments).
- **100**: Restore failed due to a mismatch between the package list and the lock file (the tool suggests deleting the lock file to force a rebuild).
