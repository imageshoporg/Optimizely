using System;
using System.Collections.Generic;

namespace Imageshop.Optimizely.Plugin.Import
{
    public static class MimeTypeMapper
    {
        private static readonly IReadOnlyDictionary<string, string> MimeTypeToExtension =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["image/apng"] = ".apng",
                ["image/avif"] = ".avif",
                ["image/gif"] = ".gif",
                ["image/jpeg"] = ".jpg",
                ["image/png"] = ".png",
                ["image/svg+xml"] = ".svg",
                ["image/webp"] = ".webp",
                ["image/bmp"] = ".bmp",
                ["image/x-icon"] = ".ico",
                ["image/vnd.microsoft.icon"] = ".ico",
                ["image/tiff"] = ".tiff"
            };

        /// <summary>
        /// Returnerar en filändelse för en given MIME-typ.
        /// Kända typer slås upp i en tabell.
        /// Okända typer faller tillbaka till delen efter "/" i MIME-typen.
        /// Ex: "image/file" -> "file" eller ".file" beroende på includeDot.
        /// </summary>
        public static string GetExtensionForMimeType(string mimeType, bool includeDot = true)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
                return includeDot ? ".jpg" : "jpg";

            // Ta bort ev. parametrar, t.ex. "; charset=utf-8"
            var normalized = mimeType.Split(';')[0].Trim();

            // 1. Försök slå upp i tabellen
            if (MimeTypeToExtension.TryGetValue(normalized, out var mappedExt))
            {
                return includeDot ? mappedExt : mappedExt.TrimStart('.');
            }

            // 2. Fallback: använd subtypen (efter "/")
            var slashIndex = normalized.IndexOf('/');
            if (slashIndex >= 0 && slashIndex < normalized.Length - 1)
            {
                var subtype = normalized.Substring(slashIndex + 1);

                // Rensa bort eventuella konstigheter, mellanslag etc.
                subtype = subtype.Trim();

                if (!string.IsNullOrEmpty(subtype))
                {
                    return includeDot ? "." + subtype : subtype;
                }
            }

            // 3. Sista utväg
            return includeDot ? ".jpg" : "jpg";
        }
    }
}