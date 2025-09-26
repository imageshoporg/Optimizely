using System.Collections.Generic;

namespace Imageshop.Optimizely.Plugin.Configuration
{
    public class ImageshopSettings
    {
        public string BaseUrl { get; set; }// = "http://client.imageshop.no/InsertImage2.aspx";

        public string Token { get; set; }

        public bool ShowSizeDialog { get; set; }
        public bool ShowCropDialog { get; set; }
        public bool FreeCrop { get; set; }

        public string InterfaceName { get; set; }
        public string DocumentPrefix { get; set; }

        public string Culture { get; set; }

        public string ProfileID { get; set; }

        //[ConfigurationProperty("webServiceUrl", DefaultValue = "http://imageshop.no/ws/v4.asmx", IsRequired = false)]
        public string WebServiceUrl { get; set; }// = "https://webservices.imageshop.no/v4.asmx";

        public bool InitializeTinyMCEPlugin { get; set; }

        public List<ImageshopSizePresetElement> SizePresets { get; set; }

        public string FormattedSizePresets
        {
            get
            {
                if (SizePresets == null || SizePresets.Count == 0)
                {
                    return string.Empty;
                }

                var sizePresets = new List<string>();

                foreach (ImageshopSizePresetElement sizePreset in SizePresets)
                {
                    sizePresets.Add(string.Format("{0};{1}x{2}", sizePreset.Name, sizePreset.Width, sizePreset.Height));
                }

                return string.Join(":", sizePresets);
            }
        }

        public bool EnableDownload { get; set; }
        /// <summary>
        /// Will add this to Permalink when downloaded into Optimizely. Example if you want it always downloads the image in 1200 with, add "__w=1200_autocrop=true" documentation https://apidocumentation.imageshop.no/start.aspx
        /// </summary>
        public string ImageDownloadAppend { get; set; }
        public bool EnableDownloadDocuments { get; set; }

        public static class UIHint
        {
            public const string ImageshopImage = "ImageshopImage";
            public const string ImageshopImageCollection = "ImageshopImageCollection";
            public const string ImageshopVideo = "ImageshopVideo";
            public const string ImageshopVideoCollection = "ImageshopVideoCollection";
        }

        public static class TinyMCEButtons
        {
            public const string ImageshopImage = "getaepiimageshop";
        }
    }
}
