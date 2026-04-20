using System.Collections.Generic;
using EPiServer.DataAnnotations;
using EPiServer.PlugIn;

namespace Imageshop.Optimizely.Plugin
{
#if NET10_0_OR_GREATER

    [PropertyDefinitionType(DisplayName = "ImageshopImageCollection")]
#else

    [PropertyDefinitionTypePlugIn(DisplayName = "Imageshop Image Collection")]
#endif
    public class PropertyImageshopImageCollection : PropertyJsonSerializedObject<IEnumerable<ImageshopImage>>
    {
    }
}
