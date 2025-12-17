using EPiServer;
using EPiServer.Core;
using EPiServer.Web;

namespace Imageshop.Optimizely.Plugin.Import;

public class ForThisPageFolderResolver : IFolderResolver
{
    private readonly IContentLoader _contentRepository;
    private readonly ContentAssetHelper contentAssetHelper;

    public ForThisPageFolderResolver( IContentLoader loader, ContentAssetHelper contentAssetHelper)
    {
        _contentRepository = loader;
        this.contentAssetHelper = contentAssetHelper;
    }

    public ContentReference GetAssetFolder(ImageshopAsset assetInformation, string contentid = null)
    {
        if (!string.IsNullOrEmpty(contentid))
        {
            var folder = new ContentReference(contentid);
            if (_contentRepository.TryGet(folder, out ContentFolder folderObj))//if asset folder, use this one
            {
                return folderObj.ContentLink;
            }

            var assetfolder = contentAssetHelper.GetOrCreateAssetFolder(new ContentReference(contentid))?.ContentLink;

            if (assetfolder != null)//is null if not "for this page"
            {
                return assetfolder;
            }
        }

        //fallback
        return  SiteDefinition.Current.GlobalAssetsRoot;
    }
}