using EPiServer.DataAnnotations;
using EPiServer.PlugIn;

namespace Imageshop.Optimizely.Plugin
{

#if NET10_0_OR_GREATER

    [PropertyDefinitionType(DisplayName = "ImageshopVideo")]
#else

    [PropertyDefinitionTypePlugIn(DisplayName = "Imageshop Video")]
#endif
    public class PropertyImageshopVideo : PropertyJsonSerializedObject<ImageshopVideo>
    {
    }
}
