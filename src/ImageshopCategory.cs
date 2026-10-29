namespace Imageshop.Optimizely.Plugin
{
    /// <summary>
    /// A category associated with an Imageshop file.
    /// </summary>
    public class ImageshopCategory
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ImageshopCategory[] ChildrenCategories { get; set; }
    }
}