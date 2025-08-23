# Imageshop.Optimizely.Plugin

[![Platform](https://img.shields.io/badge/Platform-.NET%206.0-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/Optimizely-%2012.8.0-blue.svg?style=flat)](https://www.optimizely.com/products/content)
Github repository: https://github.com/screentek/Optimizely

[Imageshop](http://www.imageshop.org) is an online-based Digital Asset Management (DAM) software. This module integrates Imageshop in the [Optimizely 12](http://www.optimizely.com) CMS User Interface. It contains a custom property and a TinyMCE plugin that launches the Imageshop image selection interface in a dialogue.

## How to get started - Installation

### Summary

The installation process currently requires some manual work. We are working to improve this process.

#### 1. Installing Nuget Package:

Start by installing NuGet package (use [the Nuget.org feed](https://www.nuget.org/packages/Imageshop.Optimizely.Plugin/) or [the Optimizely feed](https://nuget.optimizely.com/package/?id=Imageshop.Optimizely.Plugin))

    Install-Package Imageshop.Optimizely.Plugin

> **Note**: **Exception:** On build: ImageshopImage.cshtml and ImageshopVideo.cshtml will only be overwritten if their modified dates from the nuget package are newer than the modified dates of these two files in your project. If you wish to keep your own ImageshopImage.cshtml and ImageshopVideo.cshtml files, modify the files before updating the plugin.

#### 2. Adding two lines to your Startup.cs:

Add the following lines to your startup.cs file.

First line to be added at the top (before the CMS is added):

    services.AddImageShopRegistration()

Second line can be added later on:

    services.AddImageshopTinyMceConfiguration()

Or in if you rather add this to your TinyMCE config (can be used for custom tinyMCE configs): 

     services.PostConfigure<TinyMceConfiguration>(config =>
            config.Default()
             .AddImageshopToTinyMCE()); <---

![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/installation1.png)

If you have problems with routes that causes the client to not load, you can add this to your Configure method, app.UseEndpoints, in startup.cs:

    endpoints.MapControllers();

![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/mapcontrollers.png)

#### 3. Configure access token:

After the package is successfully installed you need to add your access token to configuration section ImageshopOptimizelyPlugin in your [appsettings.json](https://github.com/screentek/Optimizely/tree/master/ModifyTheseFiles) located in root directory:

```
    "ImageshopOptimizelyPlugin": {
        "Settings": {
            "token": "<Token here>",
            "interfaceName": "",
            "documentPrefix": "Warning",
            "profileId": "",
            "showSizeDialog": "true",
            "showCropDialog": "true",
            "freeCrop": "true",
            "initializeTinyMCEPlugin": "true"
        }
    }
```

> **Note**: The plugin will first look for settings based on your current environment. If it's set to "Development" it will first look into appSettings.Development.json, if still not found it will look for settings inside appSettings.json (settings for your current environment will override settings in appSettings.json). The settings file can also be called appsettings.json (with a lowercase s).

![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/config.png)

#### 4. Publishing your project

Before publishing your project, ensure that you mark the module zip file named Imageshop.Optimizely.Plugin.zip with the "Copy to output directory" set to "Copy if newer." 
This can be done using Visual Studio, or you can add additional code to your .csproj file.

To add the necessary code to your .csproj file, follow these steps:
1. Open your .csproj file.
2. Add the following code at the bottom of the file, just before the closing </Project> tag:
```
<ItemGroup>
    <None Update="modules\_protected\Imageshop.Optimizely.Plugin\Imageshop.Optimizely.Plugin.zip">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
```

![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/csproj_addition.png)

**Or** you can perform this action directly in Visual Studio, which will automatically add the code mentioned above to your .csproj file Select the Imageshop.Optimizely.Plugin.zip file and choose the option "Copy if never":
![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/mark_zipfile.png)

Build your project before publishing, and the module files will be included.

## Basics

### Add an Imageshop image property to your content model:

    [BackingType(typeof(PropertyImageshopImage))]
    [UIHint(ImageshopSettings.UIHint.ImageshopImage)]
    [ImageshopSettings(InterfaceName = "", DocumentPrefix = "", ProfileID = "", Culture = "nb-NO")]
    [ImageshopSizePreset("Main image (1280x720)", 1280, 720)]
    [ImageshopSizePreset("Thumbnail image (400x300)", 400, 300)]
    public virtual ImageshopImage MainImage { get; set; }

### Minimal Imageshop image property example:

    [BackingType(typeof(PropertyImageshopImage))]
    public virtual ImageshopImage MainImage { get; set; }

### Render the image property in a view:

    @Html.PropertyFor(m => m.CurrentPage.MainImage)

### Image collection property:

    [Display(Name = "Bilder")]
    [BackingType(typeof(PropertyImageshopImageCollection))]
    [UIHint(ImageshopSettings.UIHint.ImageshopImageCollection)]
    [ImageshopSettings(ProfileID = "CAROUSEL", ShowCropDialog = false, ShowSizeDialog = false)]
    public virtual IEnumerable<ImageshopImage> Images { get; set; }

### Imageshop video property:

    [BackingType(typeof(PropertyImageshopVideo))]
    public virtual ImageshopVideo MainVideo { get; set; }

### Render the video property in a view:

    @Html.PropertyFor(m => m.CurrentPage.MainVideo)

### Imageshop video collection property:

    [BackingType(typeof(PropertyImageshopVideoCollection))]
    [UIHint(ImageshopSettings.UIHint.ImageshopVideoCollection)]
    public virtual IEnumerable<ImageshopVideo> Videos { get; set; }

## TinyMCE

A TinyMCE plugin is included for browsing Imageshop images to add to your XhtmlString properties. It's located in the "media" group.

for custom TinyMCE configurations use .AddImageshopToTinyMCE() to TinyMceSettings and customize placement of "imageshopoptimizelyplugin" and "imageshopvideo" into a toolbar

     services.PostConfigure<TinyMceConfiguration>(config =>
            config.Default()
             .AddImageshopToTinyMCE()); <---

## Configuration

| Parameter               | Type       | Description                                                                                                                          |
| ----------------------- | ---------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| baseUrl                 | string     | Base URL to Imageshop client. Default is https://client.imageshop.no/InsertImage2.aspx                                               |
| token                   | string     | Token identifying the user.                                                                                                          |
| interfaceName           | string     | Standard interface used when searching images.                                                                                       |
| documentPrefix          | string     | Standard document code prefix used when uploading images.                                                                            |
| culture                 | string     | Language for the client. Supports en-US and nb-NO. Norwegian is default (nb-NO).                                                     |
| showSizeDialog          | true/false | Indicates whether the size dialogue should be shown. Default is true.                                                                |
| showCropDialog          | true/false | Indicates whether the crop dialogue should be show. Default is true.                                                                 |
| initializeTinyMCEPlugin | true/false | Indicates whether the tinymce plugin should be initialized.                                                                          |
| sizePresets             | string     | String in the format <Size name 1>;<width1>x<height1>:<Size name 2>;<width2>x<height2>... Predefined sizes the user can choose from. |

See configuration section ImageshopOptimizelyPlugin in appSettings.json for examples.

## Support

If you encounter any bugs or have any feature requests, please feel free to create a new issue here on GitHub. For additional support, please reach out to us at support@imageshop.no.

## Screenshots

![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/imageshop-epi-dialogue.png)

![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/imageshop-selection.png)

![ScreenShot](https://raw.githubusercontent.com/screentek/Optimizely/master/docs/imageshop-tinymce-plugin.png)

## Changelog
- **v1.3.1.0** _(08.02.25)_: Add FocalPoint
- **v1.3.0.0** _(02.02.25)_: Add TinyMCE Import Video, back track permalink, bugfix listener, localization
- **v1.2.6.2** _(10.12.24)_: bugfixes + new latest jquery version 
- **v1.2.6** _(31.10.24)_: Localization in Picker, it now uses the context language of import of AltText and other props + new logo in tinymce 
- **v1.2.5** _(23.10.24)_: Added AltText before Description when display image on site + AddEnvironmentVariables (overrides appsettings in Azure)
- **v1.2.2** _(10.12.23)_: Additional code documentation and a bugfix for linux servers where first character of filename had to be uppercase. Renamed 'imageSelector' to 'ImageSelector'.
- **v1.2.1** _(26.10.23)_: Refactoring: We've relocated module files from the "modules" directory to "modules/\_protected." Additionally, we've resolved a bug that was causing the inclusion of unwanted files from other modules in the project. Identified the problem that prevented module files from being included during project publishing (to azure, local folder etc), resulting in missing file errors. You'll find an updated guide in the readme file with a solution to this issue.
- **v1.2.0** _(26.06.23)_: Refactoring: The majority of the plugin files have been relocated to the modules/Imageshop.Optimizely.Plugin directory, instead of directly being copied into your project structure. Module.config file is also located inside the modules folder, so you are no longer needed to manually create this file. Please note that the settings section inside your appsettings.json has been renamed to ImageshopOptimizelyPlugin. If you encounter any issues, please refer to the updated installation guide on GitHub for assistance.
- **v1.1.1** _(16.06.23)_: Solved bug where the files from the plugin were not being copied over if the EPiServer.Forms.UI package was also installed.
- **v1.1.0** _(15.06.23)_: Images inserted to TinyMCE now contains additional data. Popup window for TinyMCE no longer automatically closing after a few seconds.
- **v1.0.41** _(08.06.23)_: Bugfixes: Settings can now be read from appsettings.<environment>.json files with fallback on appsettings.json, In tinymce we have added increased support for linux systems (case sensitivity for files), added additional close button to tinymce popup window, fixed bug where the source of the popup window sometimes was undefined.
- **v1.0.40** _(22.03.23)_: Bugfixes: Images now display alt-text correctly and added some useful comments on view files for models.
- **v1.0.39** _(17.03.23)_: Updated plugin to support TinyMCE v5 and newer. Bugfix: Missing images are now included when building your project.
- **v1.0.38** _(23.01.23)_: ImageshopImage model has now been updated to contain InterfaceList for the selected image.
- **v1.0.37** _(16.01.23)_: ImageshopImage model has now been updated to contain profiles for the selected image.
- **v1.0.36** _(12.01.23)_: On install: Files will be copied if modified (date + filesize), viewfiles will be copied if newer modified date (filesize ignored). + Bugfixes
