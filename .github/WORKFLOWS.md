Workflows
=========

Prefer `pwsh` for shell steps in GitHub Actions workflows for this repository.

Why:

- `pwsh` (PowerShell Core) runs consistently on GitHub-hosted Windows, macOS, and Linux runners.
- Using `pwsh` avoids Windows-specific differences when a step is written in PowerShell, and reduces surprises when running the same workflow on multiple platforms.

Guidance:

- For PowerShell scripts use `shell: pwsh` and PowerShell syntax in the `run` block.
- Only use `bash` when you need POSIX shell tools or syntax that isn't available in PowerShell.
- If you choose a minimal runner (e.g. a custom `ubuntu-slim`), ensure `gh` or other required CLI tools are installed before use.

Example:

- name: Run PowerShell script
  shell: pwsh
  run: |
    Write-Host "Hello from pwsh"

Avoid embedding build logic in workflows
---------------------------------------

Where possible, keep build, packaging, and publishing logic inside the project/repository (e.g. MSBuild targets, dotnet CLI commands, or scripts versioned with the repo) rather than embedding that logic in GitHub Actions workflows.

Why:

- Easier to test locally and in CI (build logic lives with the code it builds).
- One source of truth: build scripts and targets are reusable across CI providers and developer machines.
- Simpler workflows: workflows should orchestrate (checkout, run build, upload artifacts), not duplicate complex build mechanics.

When to use workflow logic:

- Use workflows for orchestration, environment setup, and steps that are CI-specific (e.g., publishing to GitHub Packages with repo secrets).
- If a small, portable helper is required only in CI, document it in this file and keep it minimal.
