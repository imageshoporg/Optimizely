using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Imageshop.Optimizely.Plugin.Configuration
{
    /// <summary>
    /// Class for receiving settings read from the appsettings.json file.
    /// </summary>
    public static class ImageshopConfigurationSection
    {
        private static ImageshopSettings _settings;
        public static ImageshopSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = GetSettingsSection();
                return _settings;
            }
        }

        private static ImageshopSettings GetSettingsSection()
        {
            var environment = GetEnvironmentName();
            var builder = new ConfigurationBuilder();

            if (!string.IsNullOrEmpty(environment))
            {
                //Supports both appSettings and appsettings (case sensitive on linux):
                builder.AddJsonFile($"appSettings.{environment}.json", optional: true);
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
            }

            builder.AddJsonFile("appSettings.json", optional: true);
            builder.AddJsonFile("appsettings.json", optional: true);
            builder.AddEnvironmentVariables();

            var configuration = builder.Build();
            var settingsSection = configuration.GetSection("ImageshopOptimizelyPlugin:Settings");

            try
            {
                var settings = new ImageshopSettings()
                {
                    BaseUrl = settingsSection["baseUrl"] ?? "https://client.imageshop.no/InsertImage2.aspx",
                    Token = settingsSection["token"],
                    ShowSizeDialog = bool.Parse(settingsSection["showSizeDialog"]),
                    ShowCropDialog = bool.Parse(settingsSection["showCropDialog"]),
                    EnableDownload = bool.Parse(settingsSection["enableDownload"]),
                    EnableDownloadDocuments = bool.Parse(settingsSection["enableDownloadDocuments"]),
                    FreeCrop = bool.Parse(settingsSection["freeCrop"]),
                    InterfaceName = settingsSection["interfaceName"],
                    ImageDownloadAppend = settingsSection["imageDownloadAppend"],
                    DocumentPrefix = settingsSection["documentPrefix"],
                    Culture = settingsSection["culture"],
                    ProfileID = settingsSection["profileID"],
                    WebServiceUrl = settingsSection["webServiceUrl"] ?? "https://webservices.imageshop.no/v4.asmx",
                    InitializeTinyMCEPlugin = bool.Parse(settingsSection["initializeTinyMCEPlugin"]),
                    SizePresets = GetListOfSizePresets(settingsSection["sizePresets"] ?? string.Empty)
                };

                return settings;
            }
            catch (Exception)
            {
                string filesChecked = "appSettings.json, appsettings.json";
                if (!string.IsNullOrEmpty(environment))
                {
                    filesChecked = $"appSettings.{environment}.json, appsettings.{environment}.json, " + filesChecked;
                }

                throw new Exception($"Error: Could not find the ImageshopOptimizelyPlugin settings section. Tried in the following order: {filesChecked}, please check your configuration.");
            }
        }

        private static List<ImageshopSizePresetElement> GetListOfSizePresets(string sizePresets)
        {
            var listOfSizeElements = new List<ImageshopSizePresetElement>();

            foreach (var sizeString in sizePresets.Split(':'))
            {
                if (string.IsNullOrEmpty(sizeString)) continue;

                var sizeStringParts = sizeString.Split(';');
                if (sizeStringParts.Length != 2) continue;

                var sizeName = sizeStringParts[0];
                if (string.IsNullOrEmpty(sizeName.Trim())) continue;

                var sizesPartsString = sizeStringParts[1].ToLower().Split('x');

                if (sizesPartsString.Length != 2) continue;

                int width = 0;
                int height = 0;

                try
                {
                    width = int.Parse(sizesPartsString[0]);
                    height = int.Parse(sizesPartsString[1]);
                }
                catch (Exception)
                {
                    continue;
                }

                listOfSizeElements.Add(new ImageshopSizePresetElement
                {
                    Name = sizeName,
                    Width = width,
                    Height = height
                });
            }

            return listOfSizeElements;
        }

        private static string GetEnvironmentName()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
    }
}