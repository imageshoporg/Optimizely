namespace Imageshop.Optimizely.Plugin.Import
{

    //JSON structure 
    //{
    //  "code": "IFS-12595",
    //  "image": {
    //    "file": "https://v.imgi.no/28gm2sxycf",
    //    "width": 477,
    //    "height": 0,
    //    "thumbnail": "https://magik.imageshop.no/iMageWithFallbackAndMagikAndS3.aspx?d=1&t=638992224000000000&c=image%2fjpeg&base=224&h=7B7A58E975B6E7B6370A0814349BDD8F&x=0&y=0&real=&img=cc8f5619-b62c-4bb3-9209-8c943f2b8958-wm.jpg"
    //  },
    //  "text": {
    //    "no": {
    //      "title": null,
    //      "description": null,
    //      "rights": null,
    //      "credits": null,
    //      "tags": null,
    //      "altText": null,
    //      "categories": null,
    //      "documentinfo": null
    //    },
    //    "en": {
    //      "title": "If_lifestyle_woman_man_ craftswoman_ craftsman_laptop_working",
    //      "description": " If_lifestyle_woman_man_ craftswoman_ craftsman_laptop_working",
    //      "rights": "",
    //      "credits": "",
    //      "tags": " If_lifestyle_woman_man_ craftswoman_ craftsman_laptop_working",
    //      "altText": null,
    //      "categories": [],
    //      "documentinfo": [
    //        {
    //          "DocumentInfoTypeId": 464,
    //          "Name": "Alternative text",
    //          "Value": null
    //        }
    //      ]
    //    },
    //    "sv": {
    //      "title": null,
    //      "description": null,
    //      "rights": null,
    //      "credits": null,
    //      "tags": null,
    //      "altText": null,
    //      "categories": null,
    //      "documentinfo": null
    //    },
    //    "nb": {
    //      "title": null,
    //      "description": null,
    //      "rights": null,
    //      "credits": null,
    //      "tags": null,
    //      "altText": null,
    //      "categories": null,
    //      "documentinfo": null
    //    },
    //    "nn": {
    //      "title": null,
    //      "description": null,
    //      "rights": null,
    //      "credits": null,
    //      "tags": null,
    //      "altText": null,
    //      "categories": null,
    //      "documentinfo": null
    //    },
    //    "da": {
    //      "title": null,
    //      "description": null,
    //      "rights": null,
    //      "credits": null,
    //      "tags": null,
    //      "altText": null,
    //      "categories": null,
    //      "documentinfo": null
    //    }
    //  },
    //  "extraInfo": null,
    //  "documentId": 5152515,
    //  "AuthorName": null,
    //  "InterfaceList": [
    //    {
    //      "InterfaceID": 571518,
    //      "InterfaceName": "Lifestyle Image Library"
    //    }
    //  ],
    //  "profile": null,
    //  "focalPoint": {
    //    "x": 0.22799999999999998,
    //    "y": 0.053892215568862256
    //  }
    //}

    public class ImageshopAsset
    {
        internal string documentUrl;

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

    public class LangInfo
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


    public class No: LangInfo
    {
    }

    public class Documentinfo
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class En : LangInfo
    {
    }

    public class Documentinfo1
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Sv : LangInfo
    {
    }

    public class Nb : LangInfo
    {
    }
    public class Documentinfo2
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Nn : LangInfo
    {
    }

    public class Documentinfo3
    {
        public int DocumentInfoTypeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Da : LangInfo
    {
    }

    public class Interfacelist
    {
        public int InterfaceID { get; set; }
        public string InterfaceName { get; set; }
    }

}
