# Package Release Guide

This guide explains how to release a new version of the Imageshop.Optimizely.Plugin package.

## Overview

The release process handles two versions of the plugin:
- **CMS 13** (targeting .NET 10)
- **CMS 12** (targeting .NET 8)

Each version has its own NuGet package with separate version numbers.

## Prerequisites

Before starting the release process, ensure you have:
- .NET SDK installed (supports both .NET 8 and .NET 10)
- PowerShell 5.0 or later
- Git (for committing version changes)
- Access to NuGet package repository

## Step 1: Update Version Numbers

You must update version numbers in THREE files before releasing:

### 1.1 Update `Properties\AssemblyInfo.cs`

```csharp
#if NET10_0_OR_GREATER
[assembly: AssemblyVersion("3.2.0.0")]        // ← Update for CMS 13
[assembly: AssemblyFileVersion("3.2.0.0")]
#else
[assembly: AssemblyVersion("2.2.0.0")]        // ← Update for CMS 12
[assembly: AssemblyFileVersion("2.2.0.0")]
#endif
```

**Important:** 
- Update BOTH lines for each target framework
- Maintain separate versions: 3.x.x.x for CMS 13 (NET 10) and 2.x.x.x for CMS 12 (NET 8)

### 1.2 Update `Imageshop.Optimizely.Plugin.nuspec`

```xml
<version>3.2.0.0</version>  <!-- CMS 13 version, should match AssemblyInfo.cs NET 10 version -->
```

### 1.3 Update `Imageshop.Optimizely.Plugin.Cms12.nuspec`

```xml
<version>2.2.0.0</version>  <!-- CMS 12 version, should match AssemblyInfo.cs NET 8 version -->
```

## Step 2: Run the Release Script

Open PowerShell or Command Prompt in the `src` directory and run:

```batch
.\Release.cmd
```

## Step 3: Review and Confirm Versions

The script will display all version numbers:

```
========================================
Version Check
========================================

Current version numbers:

  AssemblyInfo.cs (NET 10 - CMS 13):  3.2.0.0
  Imageshop.Optimizely.Plugin.nuspec: 3.2.0.0

  AssemblyInfo.cs (NET 8 - CMS 12):   2.2.0.0
  Imageshop.Optimizely.Plugin.Cms12.nuspec: 2.2.0.0
```

**Important:**
- NET 10 version (CMS 13) must match in AssemblyInfo.cs and Imageshop.Optimizely.Plugin.nuspec
- NET 8 version (CMS 12) must match in AssemblyInfo.cs and Imageshop.Optimizely.Plugin.Cms12.nuspec
- If versions don't match, you will see warnings

Type `y` to proceed with the release or `n` to cancel.

## Step 4: What the Release Script Does

When you confirm, the Release script automatically:

1. **Creates CMS 13 package:**
   - Calls `CreateZip.cmd release` (creates zip with CMS 13 content)
   - Runs `dotnet pack` with CMS 13 nuspec file

2. **Creates CMS 12 package:**
   - Calls `CreateZip.cmd release cms12` (creates zip with CMS 12 content)
   - Runs `dotnet pack` with CMS 12 nuspec file

## Understanding the Release Scripts

### Release.cmd
- Main orchestration script
- Validates all version numbers before proceeding
- Calls CreateZip.cmd twice (once for each CMS version)
- Runs dotnet pack twice (once for each nuspec file)

### CreateZip.cmd
- Packages plugin files into zip archives
- Syntax: `CreateZip.cmd release [cms12]`
  - `CreateZip.cmd release` → Creates CMS 13 zip
  - `CreateZip.cmd release cms12` → Creates CMS 12 zip

### GetVersions.ps1
- PowerShell helper script used by Release.cmd
- Reads and extracts version numbers from source files
- Returns versions in key=value format for batch script parsing

## Version Control Recommendations

After successful release:

```bash
# Commit version changes
git add Properties/AssemblyInfo.cs
git add Imageshop.Optimizely.Plugin.nuspec
git add Imageshop.Optimizely.Plugin.Cms12.nuspec
git commit -m "Release version 3.2.0.0 (CMS 13) and 2.2.0.0 (CMS 12)"

# Tag the release
git tag -a v3.2.0.0-cms13 -m "Release CMS 13 version 3.2.0.0"
git tag -a v2.2.0.0-cms12 -m "Release CMS 12 version 2.2.0.0"
```

## Output Files

After successful release, you will find:

- `src/bin/Release/net10.0/Imageshop.Optimizely.Plugin.nupkg` (CMS 13 package)
- `src/bin/Release/net8.0/Imageshop.Optimizely.Plugin.nupkg` (CMS 12 package)
- `src/ZippedFiles/Imageshop.Optimizely.Plugin.zip` (Latest zip archive)


## Version Numbering Strategy

### CMS 13 (NET 10) - Version 3.x.x.x
- Starts from 3.0.0.0
- Increment minor for features: 3.2.0.0 → 3.3.0.0
- Increment patch for bugfixes: 3.2.0.0 → 3.2.1.0
- Increment revision for patches: 3.2.0.0 → 3.2.0.1

### CMS 12 (NET 8) - Version 2.x.x.x
- Starts from 2.0.0.0
- Uses same pattern as CMS 13 but in 2.x range
- Only updated when backporting bugfixes to CMS 12

## Support

For issues or questions about the release process:
1. Check this README and the Troubleshooting section
2. Review the script contents (Release.cmd, CreateZip.cmd, GetVersions.ps1)
3. Consult the main project documentation
