using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.Shell.Configuration;
using EPiServer.Web;
using Imageshop.Optimizely.Plugin.Configuration;
using Imageshop.Optimizely.Plugin.Import;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Imageshop.Optimizely.Plugin.Services;

public class ImageshopAssetService: IImageshopAssetService
{
    private readonly ILogger _logger;
    private readonly IdentityMappingService _identityMappingService;
    private readonly ImageshopEvents _imageshopEvents;
    private readonly IContentLoader _contentLoader;
    private readonly IContentRepository _contentRepository;
    private readonly UIOptions _uiOptions;
    private readonly ContentMediaResolver _contentMediaResolver;
    private readonly IContentTypeRepository _contentTypeRepository;
    private readonly IFolderResolver _folderResolver;
    private readonly IUrlSegmentCreator _urlSegmentCreator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IBlobFactory _blobFactory;
    private readonly ImageshopSettings _imageshopSettings;

    public ImageshopAssetService(
        IdentityMappingService identityMappingService, 
        ImageshopEvents imageshopEvents, 
        IContentLoader contentLoader, 
        IContentRepository contentRepository, 
        UIOptions uiOptions, 
        ContentMediaResolver contentMediaResolver, 
        IContentTypeRepository contentTypeRepository, 
        IFolderResolver folderResolver, 
        IUrlSegmentCreator urlSegmentCreator, 
        IHttpClientFactory httpClientFactory, 
        IBlobFactory blobFactory,
        ILogger<ImageshopAssetService> logger)
    {
        _identityMappingService = identityMappingService;
        _imageshopEvents = imageshopEvents;
        _contentLoader = contentLoader;
        _contentRepository = contentRepository;
        _uiOptions = uiOptions;
        _contentMediaResolver = contentMediaResolver;
        _contentTypeRepository = contentTypeRepository;
        _folderResolver = folderResolver;
        _urlSegmentCreator = urlSegmentCreator;
        _httpClientFactory = httpClientFactory;
        _blobFactory = blobFactory;
        _logger = logger;
        _imageshopSettings = ImageshopConfigurationSection.Settings;
    }

    public async Task<ContentReference> AddOrUpdateAssetAsync(ImageshopAsset asset, string jsonAsset, string parent)
    {
        var customIdentity = asset.ExternalIdentifier();
        var existingMapping = _identityMappingService.Get(customIdentity);
        if (existingMapping == null)
        {
            return await AddAssetAsync(asset, jsonAsset, customIdentity, parent);
        }

        await UpdateAssetAsync(asset, jsonAsset, existingMapping);
        return existingMapping.ContentLink;
    }

    private async Task<ContentReference> AddAssetAsync(ImageshopAsset asset, string jsonAsset, Uri customIdentity, string parent)
    {

        var mediaData = await SaveAssetAsync(asset, jsonAsset, parent);
        _identityMappingService.MapContent(customIdentity, mediaData);

        var args = new ImageshopImageEventArgs
        {
            ContentLink = mediaData.ContentLink,
            ImageUrl = customIdentity.PathAndQuery
        };
        _imageshopEvents.RaiseImageshopEvent(ImageshopEvents.ImageDownloadedEvent, this, args);

        return mediaData.ContentLink;
    }

    private async Task<MediaData> SaveAssetAsync(ImageshopAsset asset, string jsonAsset, string parent)
    {
        var title = asset.documentUrl ?? FindTitle(asset) + ".jpg";
        var fileInfo = new FileInfo(title);
        var mediaTypes = _contentMediaResolver.ListAllMatching(fileInfo.Extension);
        Type? mediaType = null;
        foreach (var type in mediaTypes)
        {
            if (type.GetInterfaces().Contains(typeof(IImageshopMedia)))
            {
                mediaType = type;
                break;
            }
        }
        if (mediaType == null)
        {
            mediaType = mediaTypes.FirstOrDefault();//nevermind, then it will save without metadata json
            //var message = $"Imageshop - Could not find any ContentType associated to {fileInfo.Extension} - check IImageshopMedia implementation";
            //_logger.LogError(message);
            //throw new MissingConfigurationException(message);
        }
        var contentType = _contentTypeRepository.Load(mediaType);
        if (contentType == null)
        {
            var message = $"Imageshop - Could not find any ContentType associated to {fileInfo.Extension} - check IImageshopMedia implementation ";
            _logger.LogError(message);
            throw new MissingConfigurationException(message);
        }
        //asset.filename = ChangeExtension(fileInfo.Name, fileInfo.Extension);

        var assetFolder = _folderResolver.GetAssetFolder(asset, parent);
        var mediaData = _contentRepository.GetDefault<MediaData>(assetFolder, contentType.ID);

        mediaData.Name = fileInfo.Name;
        mediaData.RouteSegment = _urlSegmentCreator.Create(mediaData, null);

        if (mediaData is IImageshopMedia imageshopMedia)
        {
            imageshopMedia.ImageshopUrl = asset.image.file;//track back
            imageshopMedia.ImageshopJson = jsonAsset;//direct from FINDER, so we dont parse away new props added in future
            //var message = $"Imageshop configuration error: Asset of file type '{fileInfo.Extension}' is mapped to {contentType.ModelType.FullName} which do not implement interface {typeof(IImageshopMedia).FullName}";
            //_logger.LogError(message);
            //throw new MissingConfigurationException(message);
        }

        await SaveFileAsync(asset, mediaData);
        //_contentRepository.Save(mediaData, GetSaveAction(), AccessLevel.NoAccess);

        return mediaData;
    }

    private string FindTitle(ImageshopAsset asset)
    {
        // Fix: Use an array and iterate, instead of using a foreach with multiple variables separated by commas
        var titles = new[] { asset.text.en.title, asset.text.no.title, asset.text.da.title, asset.text.sv.title, asset.text.nb.title, asset.text.nn.title };
        foreach (var lang in titles)
        {
            if (!string.IsNullOrWhiteSpace(lang))
            {
                return lang;
            }
        }
        return asset.code;
    }

    private async Task SaveFileAsync(ImageshopAsset asset, MediaData mediaData)
    {
        var args = new ImageshopImageEventArgs { ImageUrl = GetDownloadUrl(asset), ImageshopAsset = asset };
        if (!_imageshopEvents.RaiseImageshopEvent(ImageshopEvents.ImageDownloadingEvent, this, args))
        {
            _logger.LogWarning($"AddAssetAsync: OperationCanceled by ImageDownloadingEvent {args.CancelReason}");
            throw new OperationCanceledException(args.CancelReason);
        }

        var blob = _blobFactory.CreateBlob(mediaData.BinaryDataContainer, Path.GetExtension(mediaData.Name));

        // Use download url for files and generic url for images
        var downloadUrl = GetDownloadUrl(asset);
        var stream = await GetMediaDataStreamAsync(downloadUrl);
        blob.Write(stream);
        mediaData.BinaryData = blob;

        _contentRepository.Save(mediaData, GetSaveAction(), AccessLevel.NoAccess);

    }

    private string GetDownloadUrl(ImageshopAsset asset)
    {

        if (asset.image?.file != null)
            return asset.image.file + _imageshopSettings.ImageDownloadAppend;
                
        return  asset.documentUrl;
    }
    
    private async Task<Stream> GetMediaDataStreamAsync(string url)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
        catch
        {
        }
        return null;
    }

    //private static string ChangeExtension(string filename, string extension)
    //{
    //    if (filename.EndsWith(extension)) return filename;
    //    var fi = new FileInfo(filename);
    //    return filename.Replace(fi.Extension, extension);
    //}

    private async Task UpdateAssetAsync(ImageshopAsset asset, string jsonAsset, MappedIdentity mappedIdentity)
    {
        if (_contentLoader.TryGet<MediaData>(mappedIdentity.ContentLink, out var mediaData))
        {
            var mediaDataClone = (MediaData)mediaData.CreateWritableClone();
            if (mediaDataClone is not IImageshopMedia imageshopMedia) return;

            //should be lastmodified date
            //if (asset.createdAt > imageshopMedia.ImageshopModifiedDate)
            //{
                //meanwhile always download the latest
                await SaveFileAsync(asset, mediaDataClone);
            //}

            imageshopMedia.ImageshopJson = jsonAsset;//direct from FINDER, so we dont parse away new props added in future

            _contentRepository.Save(mediaDataClone, GetSaveAction(), AccessLevel.NoAccess);
        }
    }

    private SaveAction GetSaveAction()
    {
        return _uiOptions.AutoPublishMediaOnUpload ? SaveAction.Publish : SaveAction.Save;
    }
}

public interface IImageshopAssetService
{
    /// <summary>
    /// Add or update Imageshop asset in Optimizely
    /// </summary>
    /// <param name="asset">Imageshop asset</param>
    /// <param name="jsonAsset">The json representation from Finder</param>
    /// <returns>Content Reference</returns>
    Task<ContentReference> AddOrUpdateAssetAsync(ImageshopAsset asset, string jsonAsset, string parent);
}