using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using Imageshop.Optimizely.Plugin.Configuration;
using Imageshop.Optimizely.Plugin.UrlBuilders;
using Microsoft.Extensions.Configuration;

namespace Imageshop.Optimizely.Plugin
{
    public class AssetPaneShellModule : ShellModule
    {
#if NET10_0_OR_GREATER
    public AssetPaneShellModule(Microsoft.Extensions.Logging.ILogger<ShellModule> logger)
        : base(logger)
    {
    }
#else
        public AssetPaneShellModule(string name, string routeBasePath, string resourceBasePath)
            : base(name, routeBasePath, resourceBasePath)
        {
        }
#endif

        /// <inheritdoc />
        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
        {
            return new ImageDownloadModuleViewModel(this, clientResourceService);
        }
    }

    public class ImageDownloadModuleViewModel : ModuleViewModel
    {
        public ImageDownloadModuleViewModel(ShellModule shellModule, IClientResourceService clientResourceService) :
            base(shellModule, clientResourceService)
        {

            IConfiguration configuration = ServiceLocator.Current.GetInstance<IConfiguration>();
            IImageshopDialogUrlBuilder ImageshopDialogUrlBuilder = ServiceLocator.Current.GetInstance<IImageshopDialogUrlBuilder>();
            pluginenableassetpane = ImageshopConfigurationSection.Settings.EnableDownload;
            pluginenabledocuments = ImageshopConfigurationSection.Settings.EnableDownloadDocuments;
            baseUrl = ImageshopDialogUrlBuilder.BuildDialogUrl(null, null, false).ToString();

        }
        public bool pluginenabledocuments { get; set; }
        public bool pluginenableassetpane { get; set; }
        public string baseUrl { get; set; }
    }

}
