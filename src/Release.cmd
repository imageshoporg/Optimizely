@echo off
setlocal enabledelayedexpansion

echo ========================================
echo Version Check
echo ========================================
echo.

rem -- Read versions using PowerShell script
echo Attempting to read versions...
for /f "tokens=1,2 delims==" %%a in ('powershell -noprofile -ExecutionPolicy Bypass -File "GetVersions.ps1"') do (
    if "%%a" == "assemblyVersion10" set "assemblyVersion10=%%b"
    if "%%a" == "assemblyVersion8" set "assemblyVersion8=%%b"
    if "%%a" == "nuspecVersion" set "nuspecVersion=%%b"
    if "%%a" == "cms12Version" set "cms12Version=%%b"
)

echo Current version numbers:
echo.
echo   AssemblyInfo.cs (NET 10 - CMS 13):  %assemblyVersion10%
echo   Imageshop.Optimizely.Plugin.nuspec: %nuspecVersion%
echo.
echo   AssemblyInfo.cs (NET 8 - CMS 12):   %assemblyVersion8%
echo   Imageshop.Optimizely.Plugin.Cms12.nuspec: %cms12Version%
echo.

if not "%assemblyVersion10%" == "%nuspecVersion%" (
    echo WARNING: AssemblyInfo.cs (NET 10 has different version than Imageshop.Optimizely.Plugin.nuspec
    echo.
)

if not "%assemblyVersion8%" == "%cms12Version%" (
    echo WARNING: AssemblyInfo.cs (NET 8 has different version than Imageshop.Optimizely.Plugin.Cms12.nuspec
    echo.
)

set /p "versionConfirm=Are these version numbers correct? (y/n): "

if /i not "%versionConfirm%" == "y" (
    echo.
    echo Release cancelled. Please update the version numbers first.
    pause
    exit /b 1
)

echo.

CALL CreateZip.cmd release

echo ----------------------
echo Creating CMS 13 package
echo ----------------------

dotnet pack Imageshop.Optimizely.Plugin.csproj -c Release -p:NuspecFile=Imageshop.Optimizely.Plugin.nuspec

echo ----------------------
echo Creating CMS 12 package
echo ----------------------

CALL CreateZip.cmd release cms12

dotnet pack Imageshop.Optimizely.Plugin.csproj -c Release -p:NuspecFile=Imageshop.Optimizely.Plugin.Cms12.nuspec

pause