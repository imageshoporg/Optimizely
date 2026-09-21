$ErrorActionPreference = "Stop"

try {
    $assemblyFile = "Properties\AssemblyInfo.cs"
    $nuspecFile = "Imageshop.Optimizely.Plugin.nuspec"
    $cms12File = "Imageshop.Optimizely.Plugin.Cms12.nuspec"

    # Get versions from AssemblyInfo.cs
    $content = Get-Content $assemblyFile -Raw
    $match3 = [regex]::Match($content, '\[assembly: AssemblyVersion\("(3\.\d+\.\d+\.\d+)"\)\]')
    $match2 = [regex]::Match($content, '\[assembly: AssemblyVersion\("(2\.\d+\.\d+\.\d+)"\)\]')

    $assemblyVersion10 = if ($match3.Success) { $match3.Groups[1].Value } else { "ERROR" }
    $assemblyVersion8 = if ($match2.Success) { $match2.Groups[1].Value } else { "ERROR" }

    # Get version from nuspec files
    $nuspec = [xml](Get-Content $nuspecFile)
    $nuspecVersion = $nuspec.package.metadata.version

    $cms12 = [xml](Get-Content $cms12File)
    $cms12Version = $cms12.package.metadata.version

    Write-Host "assemblyVersion10=$assemblyVersion10"
    Write-Host "assemblyVersion8=$assemblyVersion8"
    Write-Host "nuspecVersion=$nuspecVersion"
    Write-Host "cms12Version=$cms12Version"
}
catch {
    Write-Host "ERROR: $_"
    exit 1
}
