using System.Collections.Generic;

namespace Imageshop.Optimizely.Plugin
{
    /// <summary>
    /// Property type that contains metadata, permalink and additional data for an image in Imageshop.
    /// </summary>
    public class ImageshopImage : ImageshopFile
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual string CropName { get; set; }
        public virtual FocalPoint? FocalPoint { get; set; }
        public virtual string AltText { get; set; }
        public virtual ImageshopImageProfile Profile { get; set; }
        public virtual IEnumerable<ImageshopInterfaceInfo> InterfaceList { get; set; }

        public virtual string GetCroppedUrl(string cropName)
        {
            if (!string.IsNullOrWhiteSpace(cropName))
            {
                return string.Format("{0}-{1}", Url, cropName);
            }

            return Url!;
        }
    }

    /// <summary>
    /// Contains ID and name of an interface than an ImageshopImage is connected to
    /// </summary>
    public class ImageshopInterfaceInfo
    {
        public virtual int InterfaceID { get; set; }
        public virtual string InterfaceName { get; set; }
    }

    /// <summary>
    /// Profile information for an ImageshopImage
    /// </summary>
    public class ImageshopImageProfile
    {
        public virtual string Name { get; set; }
        public virtual IEnumerable<ImageshopImageProfileSize> ProfileSizes { get; set; } 
    }

    /// <summary>
    /// Profile sizes for a profile connected to an ImageshopImage
    /// </summary>
    public class ImageshopImageProfileSize
    {
        public virtual string Url { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual string CropName { get; set; }
        public virtual string CropFormat { get; set; }
        public virtual string SizeName { get; set; }
    }

    public class FocalPoint
    {
        public float X { get; set; } 
        public float Y { get; set; }
    }
}
