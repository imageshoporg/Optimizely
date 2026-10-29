using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using EPiServer.ServiceLocation;
using Imageshop.Optimizely.Plugin.Configuration;
using Imageshop.Optimizely.Plugin.Extensions;
using Imageshop.Optimizely.Plugin.Attributes;

namespace Imageshop.Optimizely.Plugin.UrlBuilders
{
    [ServiceConfiguration(typeof(IImageshopDialogUrlBuilder), Lifecycle = ServiceInstanceScope.Singleton)]
    public class DefaulImageshopDialogUrlBuilder : IImageshopDialogUrlBuilder
    {
        public virtual UriBuilder BuildDialogUrl(ImageshopSettingsAttribute configurationAttribute, IEnumerable<ImageshopSizePresetAttribute> sizePresetAttributes, bool addVideoParameter)
        {
            string baseUrl = ImageshopConfigurationSection.Settings.BaseUrl!;
            NameValueCollection query = BuildDialogQuery(configurationAttribute, sizePresetAttributes, addVideoParameter);

            var uri = new UriBuilder(baseUrl)
            {
                Query = ConstructQueryString(query)
            };

            return uri;
        }

        public string ConstructQueryString(NameValueCollection parameters)
        {
            var items = new List<string>();

            foreach (string name in parameters)
            {
                items.Add(string.Concat(name, "=", HttpUtility.UrlEncode(parameters[name])));
            }

            return string.Join("&", items.ToArray());
        }

        public virtual NameValueCollection BuildDialogQuery(ImageshopSettingsAttribute configurationAttribute, IEnumerable<ImageshopSizePresetAttribute> sizePresetAttributes, bool addVideoParameter)
        {
            string token = ImageshopConfigurationSection.Settings.Token!;
            string interfaceName = ImageshopConfigurationSection.Settings.InterfaceName!;
            string documentPrefix = ImageshopConfigurationSection.Settings.DocumentPrefix!;
            string profileId = ImageshopConfigurationSection.Settings.ProfileID!;
            bool showSizeDialog = ImageshopConfigurationSection.Settings.ShowSizeDialog;
            bool IncludeCategories = ImageshopConfigurationSection.Settings.IncludeCategories;
            bool showCropDialog = ImageshopConfigurationSection.Settings.ShowCropDialog;
            bool freeCropDialog = ImageshopConfigurationSection.Settings.FreeCrop;
            string sizePresets = ImageshopConfigurationSection.Settings.FormattedSizePresets;

            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("IFRAMEINSERT", "true");
            query.Add("IMAGESHOPTOKEN", token);

            // Read settings from attribute.
            if (configurationAttribute != null)
            {
                if (string.IsNullOrWhiteSpace(configurationAttribute.InterfaceName) == false)
                {
                    interfaceName = configurationAttribute.InterfaceName;
                }

                if (string.IsNullOrWhiteSpace(configurationAttribute.DocumentPrefix) == false)
                {
                    documentPrefix = configurationAttribute.DocumentPrefix;
                }

                if (string.IsNullOrWhiteSpace(configurationAttribute.ProfileID) == false)
                {
                    profileId = configurationAttribute.ProfileID;
                }

                IncludeCategories = configurationAttribute.IncludeCategories;
                showCropDialog = configurationAttribute.ShowCropDialog;
                freeCropDialog = configurationAttribute.FreeCrop;
            }

            query.Add("SHOWCATEGORIESINJSON", IncludeCategories.ToString().ToLowerInvariant());
            query.Add("SHOWSIZEDIALOGUE", showSizeDialog.ToString().ToLowerInvariant());
            query.Add("SHOWCROPDIALOGUE", showCropDialog.ToString().ToLowerInvariant());
            query.Add("FREECROP", freeCropDialog.ToString().ToLowerInvariant());
            query.AddIfNotNull("IMAGESHOPINTERFACENAME", interfaceName);
            query.AddIfNotNull("IMAGESHOPDOCUMENTPREFIX", documentPrefix);
            query.AddIfNotNull("PROFILEID", profileId);

            // Read and apply size preset attributes
            if (sizePresetAttributes != null)
            {
                sizePresets = "";
                int index = 0;

                foreach (var preset in sizePresetAttributes)
                {
                    if (index++ > 0)
                    {
                        sizePresets += ":";
                    }

                    var presetString = string.Format("{0};{1}x{2}", preset.Name, preset.Width, preset.Height);
                    sizePresets += presetString;
                }
            }

            query.AddIfNotNull("IMAGESHOPSIZES", sizePresets);
            query.Add("FORMAT", "json");

            if (addVideoParameter)
            {
                query.Add("SHOWVIDEO", "true");
            }

            return query;
        }
    }
}