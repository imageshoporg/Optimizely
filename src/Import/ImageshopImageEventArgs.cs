using EPiServer.Core;
using System;

namespace Imageshop.Optimizely.Plugin.Import;

public class ImageshopImageEventArgs : EventArgs
{
    public ContentReference ContentLink { get; set; }
    public string ImageUrl { get; set; }
    /// <summary>Set value to abort the current event handling</summary>
    /// <remarks>Changing this value may not always change the behavior, see the specific event for details.</remarks>
    public bool CancelAction { get; set; }
    /// <summary>Gets or sets the reason for cancel.</summary>
    /// <value>The cancel reason.</value>
    /// <remarks>
    /// If the cancellation results in an CancelAction, this string will be used as the message in the exception.
    /// </remarks>
    public string CancelReason { get; set; }

    public ImageshopAsset ImageshopAsset { get; set; }

}