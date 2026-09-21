@echo off
setlocal enabledelayedexpansion

set "action=%1"
set "cms12=%2"

if not "%action%" == "release" (
    echo ---------------------------------
    echo Please run 'Release.cmd' instead.
    echo ---------------------------------
    pause
    exit /b 1
)

rem -- Get the version of the plugin from the nuspec file --
if "%cms12%" == "cms12" (
    set "nuspecFile=%~dp0Imageshop.Optimizely.Plugin.Cms12.nuspec"
) else (
    set "nuspecFile=%~dp0Imageshop.Optimizely.Plugin.nuspec"
)

set "version="
for /f "usebackq tokens=3 delims=<>" %%a in (`findstr /R /C:"<version>" "%nuspecFile%"`) do (
    set "version=%%a"
)

echo Version: %version%

echo ---------------------------------------------------
echo -- Packing Imageshop.Optimizely.Plugin into .zip --
echo --           Plugin version %version%           --
echo ---------------------------------------------------

set "CD=%~dp0"  rem Current directory
set "DD=%CD%\ZippedFiles"  rem Destination directory
set "zipFileName=Imageshop.Optimizely.Plugin.zip"  rem Name of the zip file

rem Delete the existing ZippedFiles directory if it exists:
if exist "%DD%" (
    rmdir /s /q "%DD%"
)

rem -- Copying files to the Zippedfiles directory. --

rem -- Copying tinymce editor files --
echo Copying tinymce editor files:
set "destinationPath=%DD%\content\%version%\ClientResources\tinymce\"
xcopy "%CD%\TinyMce\ClientResources\editor\*" "%destinationPath%\" /S /I /Y

rem -- Copying widget files. --
echo Copying widget files:
set "destinationPath=%DD%\content\%version%\ClientResources\"
xcopy "%CD%\Widgets\*" "%destinationPath%\" /S /I /Y

echo Copying tinymce view files:
set "destinationPath=%DD%\content\%version%\Views\Imageshop\"
xcopy "%CD%\TinyMce\ClientResources\ViewFiles\*" "%destinationPath%\" /S /I /Y

echo Copying property view files:
set "destinationPath=%DD%\content\%version%\Views\Shared\DisplayTemplates\"
xcopy "%CD%\PropertyViewFiles\*" "%destinationPath%\" /S /I /Y

echo Copying scripts:
set "destinationPath=%DD%\content\%version%\scripts"
xcopy "%CD%\scripts\*" "%destinationPath%\" /S /I /Y

rem -- Update module.config with actual version number --
set "sourceModuleConfig=%CD%\module.config"
set "tempModuleConfig=%CD%\temp_module.config"

rem Copy module.config to a temporary file
echo Copying module.config to a temporary file
copy "%sourceModuleConfig%" "%tempModuleConfig%" /Y

rem Update the version number in the temporary module.config file
echo Updating version number of temporary module.config file
powershell -Command "(gc '%tempModuleConfig%') -replace '\$version\$', '%version%' | Set-Content -Path '%tempModuleConfig%'"

rem Copy the updated temporary module.config to the ZippedFiles folder
echo Copying module.config:
set "destinationPath=%DD%\content\"
copy "%tempModuleConfig%" "%destinationPath%\module.config" /Y

rem Delete the existing temp_module.config file:
echo Deleting temp_module.config file
del "%tempModuleConfig%"

rem Create the zip file
set "zipFile=%DD%\%zipFileName%"
powershell -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [System.IO.Compression.ZipFile]::CreateFromDirectory('%DD%\content', '%zipFile%'); }"

echo Zip file created: %zipFile%

endlocal