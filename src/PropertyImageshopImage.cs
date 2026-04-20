using EPiServer.DataAnnotations;
using EPiServer.PlugIn;
using EPiServer.Shell.Modules;

namespace Imageshop.Optimizely.Plugin
{

#if NET10_0_OR_GREATER

    [PropertyDefinitionType(DisplayName = "ImageshopImage")]
#else

    [PropertyDefinitionTypePlugIn(DisplayName = "Imageshop Image")]
#endif
    public class PropertyImageshopImage : PropertyJsonSerializedObject<ImageshopImage>
    {
    }
}
