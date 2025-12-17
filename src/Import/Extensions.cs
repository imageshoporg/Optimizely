using EPiServer.DataAbstraction;
using System;

namespace Imageshop.Optimizely.Plugin.Import
{
    public static class Extensions
    {
        public static Uri ExternalIdentifier(this ImageshopAsset asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            var externalIdentifier = MappedIdentity.ConstructExternalIdentifier("Imageshop", asset.documentUrl ?? asset.image.file );
            return externalIdentifier;
        }
    }
}
