using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using Imageshop.Optimizely.Plugin.Attributes;
using Imageshop.Optimizely.Plugin.Configuration;
using Imageshop.Optimizely.Plugin.UrlBuilders;

namespace Imageshop.Optimizely.Plugin.Descriptors
{
    public abstract class ImageshopEditorDescriptorBase : EditorDescriptor
    {
        protected readonly IImageshopDialogUrlBuilder ImageshopDialogUrlBuilder;
        protected virtual bool IsVideoDescriptor { get { return false; } }

        protected ImageshopEditorDescriptorBase() : this(ServiceLocator.Current.GetInstance<IImageshopDialogUrlBuilder>())
        {
        }

        protected ImageshopEditorDescriptorBase(IImageshopDialogUrlBuilder imageshopDialogUrlBuilder)
        {
            ImageshopDialogUrlBuilder = imageshopDialogUrlBuilder;
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            ImageshopSettingsAttribute configurationAttribute = GetConfigurationAttribute(metadata, attributes);
            IEnumerable<ImageshopSizePresetAttribute> sizePresetAttributes = GetSizePresetAttributes(metadata, attributes);

            UriBuilder dialogUrl = ImageshopDialogUrlBuilder.BuildDialogUrl(configurationAttribute!, sizePresetAttributes, IsVideoDescriptor);
            metadata.EditorConfiguration.Add("baseUrl", dialogUrl.ToString());
            metadata.EditorConfiguration.Add("preferredLanguage", MapToImageshopLanguage(ContentLanguage.PreferredCulture));
            metadata.EditorConfiguration.Add("fallbackLanguage", ImageshopConfigurationSection.Settings.FallbackLanguage);
            metadata.EditorConfiguration.Add("isVideo", IsVideoDescriptor);

            if (configurationAttribute != null)
            {
                metadata.EditorConfiguration.Add("cropName", configurationAttribute.CropName);
                metadata.EditorConfiguration.Add("previewCropName", configurationAttribute.PreviewCropName);
            }
        }

        protected virtual string MapToImageshopLanguage(CultureInfo cultureInfo)
        {
            CultureInfo neutralCulture = cultureInfo;

            while (Equals(neutralCulture.Parent, CultureInfo.InvariantCulture) == false)
            {
                neutralCulture = neutralCulture.Parent;
            }

            return neutralCulture.Name.ToLowerInvariant();
        }

        protected virtual ImageshopSettingsAttribute GetConfigurationAttribute(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            return attributes.OfType<ImageshopSettingsAttribute>().FirstOrDefault() ??
                   metadata.ContainerType.GetCustomAttribute<ImageshopSettingsAttribute>(true);
        }

        protected virtual IEnumerable<ImageshopSizePresetAttribute> GetSizePresetAttributes(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            return attributes.OfType<ImageshopSizePresetAttribute>();
        }
    }
}
