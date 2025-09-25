namespace Imageshop.Optimizely.Plugin.Import
{

    public class ImageshopAsset
    {
        public string code { get; set; }
        public Image image { get; set; }
        public Text text { get; set; }
        public object extraInfo { get; set; }
        public int documentId { get; set; }
        public object AuthorName { get; set; }
        public Interfacelist[] InterfaceList { get; set; }
        public object profile { get; set; }
        public object focalPoint { get; set; }
    }

    public class Image
    {
        public string file { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string thumbnail { get; set; }
    }

    public class Text
    {
        public No no { get; set; }
        public En en { get; set; }
        public Sv sv { get; set; }
        public Nb nb { get; set; }
        public Nn nn { get; set; }
        public Da da { get; set; }
    }

    public class No
    {
        public string title { get; set; }
        public string description { get; set; }
        public string rights { get; set; }
        public string credits { get; set; }
        public string tags { get; set; }
        public string altText { get; set; }
        public object[] categories { get; set; }
        public Documentinfo[] documentinfo { get; set; }
    }

    public class Documentinfo
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class En
    {
        public string title { get; set; }
        public string description { get; set; }
        public string rights { get; set; }
        public string credits { get; set; }
        public string tags { get; set; }
        public string altText { get; set; }
        public object[] categories { get; set; }
        public Documentinfo1[] documentinfo { get; set; }
    }

    public class Documentinfo1
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Sv
    {
        public object title { get; set; }
        public object description { get; set; }
        public object rights { get; set; }
        public object credits { get; set; }
        public object tags { get; set; }
        public object altText { get; set; }
        public object categories { get; set; }
        public object documentinfo { get; set; }
    }

    public class Nb
    {
        public string title { get; set; }
        public string description { get; set; }
        public string rights { get; set; }
        public string credits { get; set; }
        public string tags { get; set; }
        public string altText { get; set; }
        public object[] categories { get; set; }
        public Documentinfo2[] documentinfo { get; set; }
    }

    public class Documentinfo2
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Nn
    {
        public string title { get; set; }
        public string description { get; set; }
        public string rights { get; set; }
        public string credits { get; set; }
        public string tags { get; set; }
        public string altText { get; set; }
        public object[] categories { get; set; }
        public Documentinfo3[] documentinfo { get; set; }
    }

    public class Documentinfo3
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Da
    {
        public object title { get; set; }
        public object description { get; set; }
        public object rights { get; set; }
        public object credits { get; set; }
        public object tags { get; set; }
        public object altText { get; set; }
        public object categories { get; set; }
        public object documentinfo { get; set; }
    }

    public class Interfacelist
    {
        public int InterfaceID { get; set; }
        public string InterfaceName { get; set; }
    }

}
