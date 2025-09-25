using EPiServer.Core;

namespace Imageshop.Optimizely.Plugin.Import;

public interface IFolderResolver
{
    ContentReference GetAssetFolder(ImageshopAsset asset, string contentid = null);
}