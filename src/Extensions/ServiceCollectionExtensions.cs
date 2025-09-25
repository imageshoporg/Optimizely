using EPiServer.Cms.TinyMce.Core;
using EPiServer.Shell;
using EPiServer.Shell.Modules;
using Imageshop.Optimizely.Plugin.Configuration;
using Imageshop.Optimizely.Plugin.Helpers;
using Imageshop.Optimizely.Plugin.Import;
using Imageshop.Optimizely.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Imageshop.Optimizely.Plugin.Extensions
{
    /// <summary>
    /// Static class that is called from within the executing assembly. This is used during startup of projects to enable the Imageshop Plugin.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Typically called from the Startup.cs file of the project that has the plugin installed. 
        /// Registers the plugin as a protected module.
        /// </summary>
        public static IServiceCollection AddImageShopRegistration(this IServiceCollection services)
        {
            var executingAssembliy = Assembly.GetExecutingAssembly();

            services.Configure<ProtectedModuleOptions>(o => o.Items.Add(new ModuleDetails { Name = "Imageshop.Optimizely.Plugin" }));


            services.AddSingleton<IImageshopAssetService, ImageshopAssetService>();
            services.AddSingleton<IFolderResolver, ForThisPageFolderResolver>();

            return services.Configure<IMvcBuilder>(x => x.AddApplicationPart(executingAssembliy));
        }

        /// <summary>
        /// Typically called from the Startup.cs file of the project that has the plugin installed.
        /// Enables the Imageshop Plugin for TinyMCE. 
        /// </summary>
        public static IServiceCollection AddImageshopTinyMceConfiguration(this IServiceCollection services)
        {
            if (ImageshopConfigurationSection.Settings.InitializeTinyMCEPlugin)
            {
                services.PostConfigure<TinyMceConfiguration>(config =>
                    config.Default()
                    .AddImageshopToTinyMCE());
            } 
            return services;
        }

        /// <summary>
        /// Enables the Imageshop Plugin for TinyMCE for custom TinyMCEs
        /// </summary>
        /// <param name="tinySettings"></param>
        /// <returns></returns>
        public static TinyMceSettings AddImageshopToTinyMCE(this TinyMceSettings tinySettings, bool addToToolbar = true)
        {
            tinySettings.AddExternalPlugin("imageshopoptimizelyplugin", Paths.ToClientResource(ImageshopPluginHelper.GetPluginAssemblyName(), "ClientResources/tinymce/editor_plugin.js"));

            if (addToToolbar)
            {
                bool exists = false;

                string[] toolbarStrings = (tinySettings["toolbar"] as string[]);

                if (toolbarStrings?.Length > 0)
                {
                    if (toolbarStrings[0].Contains(" image "))
                    {
                        var toolbar = toolbarStrings[0].Replace(" image ", " imageshopoptimizelyplugin imageshopvideo image ");
                        toolbarStrings[0] = toolbar;
                        exists = true;
                    }
                }

                if (!exists)
                {
                    tinySettings.AppendToolbar("imageshopoptimizelyplugin imageshopvideo");
                }
            }

            return tinySettings;
        }
    }
}
