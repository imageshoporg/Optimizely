using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Imageshop.Optimizely.Plugin.Import;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Linq;
using EPiServer;

namespace Imageshop.Optimizely.Plugin
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Renders an HTML img tag for Imageshop media with proper attributes including width, height, and alt text
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance</param>
        /// <param name="media">The Imageshop media object containing image data and metadata</param>
        /// <param name="postfix">Will add this to Permalink when downloaded into Optimizely. Example if you want it always downloads the image in 1200 with, add "__w=1200_autocrop=true" documentation https://apidocumentation.imageshop.no/start.aspx</param>
        /// <returns>HTML string containing the rendered img tag</returns>
        public static HtmlString Imageshop(this IHtmlHelper htmlHelper, IImageshopMedia media, string postfix = null)
        {
            var imgTag = new TagBuilder("img");

            string url;

            if (!string.IsNullOrEmpty(media.ImageshopUrl))
            {
                url = media.ImageshopUrl;
            }
            else
            {
                var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
                url = urlResolver.GetUrl(media as IContent);
            }

            imgTag.Attributes.Add("src", url + postfix);


            if (!string.IsNullOrEmpty(media.ImageshopJson))
            {
                var asset = JsonSerializer.Deserialize<ImageshopAsset>(media.ImageshopJson);

                if (asset != null)
                {
                    if (!imgTag.Attributes.Keys.Contains("width") && asset.image?.width > 0)
                    {
                        imgTag.Attributes.Add("width", asset.image?.width.ToString());
                    }
                    if (!imgTag.Attributes.Keys.Contains("height") && asset.image?.height > 0)
                    {
                        imgTag.Attributes.Add("height", asset.image?.height.ToString());
                    }
                    if (!imgTag.Attributes.Keys.Contains("alt"))
                    {
                        var altText = GetAltText(asset, Thread.CurrentThread.CurrentCulture);
                        imgTag.Attributes.Add("alt", altText);
                    }
                }
            }


            return new HtmlString(GetString(imgTag.RenderSelfClosingTag()));
        }


        /// <summary>
        /// Renders an HTML img tag for Imageshop media using a content reference
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance</param>
        /// <param name="contentReference">Content reference to the Imageshop media</param>
        /// <param name="postfix">Optional postfix to add to the image URL for transformations, Example if you want it always show the image in 1200 with, add "__w=1200_autocrop=true" documentation https://apidocumentation.imageshop.no/start.aspx</param>
        /// <returns>HTML string containing the rendered img tag, or empty string if content not found</returns>
        public static HtmlString Imageshop(this IHtmlHelper htmlHelper, ContentReference contentReference, string postfix = null)
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            if (contentReference != null && contentLoader.TryGet<IImageshopMedia>(contentReference, out var media))
            {
                return Imageshop(htmlHelper, media, postfix);
            }
            return HtmlString.Empty;
        }

        /// <summary>
        /// Retrieves the Imageshop asset metadata from a content reference
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance</param>
        /// <param name="contentReference">Content reference to the Imageshop media</param>
        /// <returns>Imageshop asset object containing metadata, or null if not found</returns>
        public static ImageshopAsset ImageshopAsset(this IHtmlHelper htmlHelper, ContentReference contentReference)
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            if (contentReference != null && contentLoader.TryGet<IImageshopMedia>(contentReference, out var media))
            {
                if (!string.IsNullOrEmpty(media.ImageshopJson))
                {
                    var asset = JsonSerializer.Deserialize<ImageshopAsset>(media.ImageshopJson);
                    return asset;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the alt text for an Imageshop image based on the current culture
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance</param>
        /// <param name="contentReference">Content reference to the Imageshop media</param>
        /// <returns>Alt text string for the image in the current culture, or null if not found</returns>
        public static string ImageshopAltText(this IHtmlHelper htmlHelper, ContentReference contentReference)
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            if (contentReference != null && contentLoader.TryGet<IImageshopMedia>(contentReference, out var media))
            {
                var asset = ImageshopAsset(htmlHelper, contentReference);
                return GetAltText(asset, Thread.CurrentThread.CurrentCulture);
            }
            return null;
        }

        /// <summary>
        /// Gets the appropriate alt text for an Imageshop asset based on the specified culture
        /// </summary>
        /// <param name="asset">The Imageshop asset containing text metadata</param>
        /// <param name="currentCulture">The culture to use for language-specific alt text</param>
        /// <returns>Alt text string in the specified language, with fallbacks to English and title if needed</returns>
        public static string GetAltText(this ImageshopAsset asset, CultureInfo currentCulture)
        {
            if (asset?.text == null)
                return string.Empty;

            // Get the language code from current culture (e.g., "en", "no", "sv", etc.)
            var languageCode = currentCulture.TwoLetterISOLanguageName.ToLowerInvariant();
            
            // Try to get alt text for the specific language
            var altText = GetAltTextForLanguage(asset.text, languageCode);
            
            // If no alt text found for current language, fallback to English
            if (string.IsNullOrEmpty(altText) && languageCode != "en")
            {
                altText = GetAltTextForLanguage(asset.text, "en");
            }
            
            // If still no alt text, try title as fallback
            if (string.IsNullOrEmpty(altText))
            {
                altText = GetTitleForLanguage(asset.text, languageCode);
                if (string.IsNullOrEmpty(altText) && languageCode != "en")
                {
                    altText = GetTitleForLanguage(asset.text, "en");
                }
            }
            
            return altText ?? string.Empty;
        }

        /// <summary>
        /// Retrieves alt text for a specific language from the asset's text metadata
        /// </summary>
        /// <param name="text">The text metadata from the Imageshop asset</param>
        /// <param name="languageCode">The two-letter language code (e.g., "en", "no", "sv")</param>
        /// <returns>Alt text string for the specified language, or null if not found</returns>
        private static string GetAltTextForLanguage(Text text, string languageCode)
        {
            LangInfo langInfo = GetLanguageInfo(text, languageCode);
            
            if (langInfo == null)
                return null;

            // First try the altText property
            if (!string.IsNullOrEmpty(langInfo.altText))
                return langInfo.altText;

            // Then check documentinfo for "Alternative text"
            if (langInfo.documentinfo != null)
            {
                var altTextDoc = langInfo.documentinfo
                    .FirstOrDefault(d => d.Name?.Equals("Alternative text", StringComparison.OrdinalIgnoreCase) == true);
                
                if (altTextDoc != null && !string.IsNullOrEmpty(altTextDoc.Value))
                    return altTextDoc.Value;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the title for a specific language from the asset's text metadata
        /// </summary>
        /// <param name="text">The text metadata from the Imageshop asset</param>
        /// <param name="languageCode">The two-letter language code (e.g., "en", "no", "sv")</param>
        /// <returns>Title string for the specified language, or null if not found</returns>
        private static string GetTitleForLanguage(Text text, string languageCode)
        {
            LangInfo langInfo = GetLanguageInfo(text, languageCode);
            return langInfo?.title;
        }

        /// <summary>
        /// Gets the language-specific information object from the text metadata
        /// </summary>
        /// <param name="text">The text metadata from the Imageshop asset</param>
        /// <param name="languageCode">The two-letter language code (e.g., "en", "no", "sv")</param>
        /// <returns>Language info object containing localized text data, or null if language not supported</returns>
        private static LangInfo GetLanguageInfo(Text text, string languageCode)
        {
            return languageCode switch
            {
                "en" => text.en,
                "no" => text.no,
                "nb" => text.nb,
                "nn" => text.nn,
                "sv" => text.sv,
                "da" => text.da,
                _ => null
            };
        }

        /// <summary>
        /// Converts HTML content to a string representation
        /// </summary>
        /// <param name="content">The HTML content to convert</param>
        /// <returns>String representation of the HTML content</returns>
        public static string GetString(IHtmlContent content)
        {
            using (var writer = new StringWriter())
            {
                content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }
}
