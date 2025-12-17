using EPiServer.Core;

namespace Imageshop.Optimizely.Plugin.Import;

public interface IImageshopMedia: IContentData
{

    /// <summary>
    /// Imageshop CDN url
    /// </summary>
    string ImageshopUrl { get; set; } //https://v.imgi.no/28gm2sxycf

    /// <summary>
    /// Imageshop JSON metadata
    /// </summary>
    string ImageshopJson { get; set; }
}