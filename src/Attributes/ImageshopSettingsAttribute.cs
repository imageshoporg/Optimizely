using System;
using System.ComponentModel;

namespace Imageshop.Optimizely.Plugin.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class ImageshopSettingsAttribute : Attribute
    {
        private bool? _showSizeDialog;
        private bool? _showCropDialog;
        private bool? _freeCrop;
        private bool? _includeCategories;

        public ImageshopSettingsAttribute()
        {
            PreviewCropName = string.Empty;
            CropName = string.Empty;
        }

        [Description("Name of the default interface (if any).")]
        public string InterfaceName { get; set; }

        [Description("Standard document code prefix used when uploading images.")]
        public string DocumentPrefix { get; set; }

        [Obsolete("This is not in use, it will use the page context language dynamically")]
        [Description("Culture (en-US and nb-NO supported). Default is nb-NO.")]
        public string Culture { get; set; }

        [Description("The id of the Imageshop profile to use.")]
        public string ProfileID { get; set; }

        [Description("Show size dialog. When not explicitly set, uses global configuration. Default is true.")]
        public bool ShowSizeDialog
        {
            get => _showSizeDialog ?? true;
            set => _showSizeDialog = value;
        }

        [Description("Show crop dialog. When not explicitly set, uses global configuration. Default is true.")]
        public bool ShowCropDialog
        {
            get => _showCropDialog ?? true;
            set => _showCropDialog = value;
        }

        [Description("Free cropping is enabled when selecting an image. When not explicitly set, uses global configuration. Default is true.")]
        public bool FreeCrop
        {
            get => _freeCrop ?? true;
            set => _freeCrop = value;
        }

        [Description("Name of the crop to use for preview when editing. Default is empty string.")]
        public string PreviewCropName { get; set; }

        [Description("Name of the crop to use for the image. Default is empty string.")]
        public string CropName { get; set; }

        [Description("Show categories in JSON format. When not explicitly set, uses global configuration. Default is false.")]
        public bool IncludeCategories
        {
            get => _includeCategories ?? false;
            set => _includeCategories = value;
        }

        /// <summary>
        /// Determines whether a specific Boolean property was explicitly set on this attribute instance.
        /// </summary>
        /// <param name="propertyName">The name of the property (e.g., "ShowSizeDialog", "ShowCropDialog", "FreeCrop", "IncludeCategories")</param>
        /// <returns>True if the property was explicitly assigned; false if it uses the default value.</returns>
        public bool IsPropertyExplicitlySet(string propertyName)
        {
            return propertyName switch
            {
                nameof(ShowSizeDialog) => _showSizeDialog.HasValue,
                nameof(ShowCropDialog) => _showCropDialog.HasValue,
                nameof(FreeCrop) => _freeCrop.HasValue,
                nameof(IncludeCategories) => _includeCategories.HasValue,
                _ => false
            };
        }
    }
}
