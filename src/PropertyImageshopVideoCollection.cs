using EPiServer.DataAnnotations;
using EPiServer.PlugIn;
using System.Collections.Generic;

namespace Imageshop.Optimizely.Plugin
{

#if NET10_0_OR_GREATER

    [PropertyDefinitionType(DisplayName = "ImageshopVideoCollection")]
#else

    [PropertyDefinitionTypePlugIn(DisplayName = "Imageshop Video Collection")]
#endif
    public class PropertyImageshopVideoCollection : PropertyJsonSerializedObject<IEnumerable<ImageshopVideo>>
    {
    }
}
