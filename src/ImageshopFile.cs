using Imageshop.Optimizely.Plugin.Import;
using System;

namespace Imageshop.Optimizely.Plugin
{
    /// <summary>
    /// Property type for files
    /// </summary>
    public class ImageshopFile
    {
        public ImageshopFile()
        {
            Changed = DateTime.MinValue; 
        }

        public virtual int DocumentID { get; set; } 
        public virtual string Code { get; set; }
        public virtual string Url { get; set; }
        public virtual string Name { get; set; }
        public virtual string Credits { get; set; }
        public virtual string Description { get; set; }
        public virtual string Comment { get; set; }
        public virtual string Rights { get; set; }
        public virtual string Tags { get; set; }
        public virtual string AuthorName { get; set; }
        public virtual Documentinfo[] DocumentInfo { get; set; }
        public DateTime Changed { get; set; }
    }
}
