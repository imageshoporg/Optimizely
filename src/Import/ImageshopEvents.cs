using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace Imageshop.Optimizely.Plugin.Import;

[ServiceConfiguration(typeof(ImageshopEvents), FactoryMember = "Instance", Lifecycle = ServiceInstanceScope.Singleton)]
public class ImageshopEvents : IDisposable
{

    private readonly ILogger _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static readonly object KeyLock = new();
    private EventHandlerList _events = new();
    private readonly Dictionary<string, object> _eventKeys = new();
    private static ImageshopEvents _instance;
    public static readonly string ImageDownloadedEvent = "ImageDownloadedEvent";
    public static readonly string ImageDownloadingEvent = "ImageDownloadingEvent";
    public static readonly string ImageDeletedEvent = "ImageDeletedEvent";

    private EventHandlerList Events
    {
        get
        {
            if (_events == null)
                throw new ObjectDisposedException(GetType().FullName);
            return _events;
        }
    }

    private object GetEventKey(string stringKey)
    {
        if (!_eventKeys.TryGetValue(stringKey, out var obj))
        {
            lock (KeyLock)
            {
                if (!_eventKeys.TryGetValue(stringKey, out obj))
                {
                    obj = new object();
                    _eventKeys[stringKey] = obj;
                }
            }
        }
        return obj;
    }

    public static ImageshopEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (KeyLock)
                {
                    _instance ??= new ImageshopEvents();
                }
            }
            return _instance;
        }
    }


    /// <summary>
    /// Triggered when image is downloaded from imageshop and imported to Optimizely
    /// </summary>
    public event EventHandler<ImageshopImageEventArgs> OnImageDownloaded
    {
        add => Events.AddHandler(GetEventKey(ImageDownloadedEvent), value);
        remove => Events.RemoveHandler(GetEventKey(ImageDownloadedEvent), value);
    }

    /// <summary>
    /// Triggered when image pre downloading from imageshop and imported to Optimizely, possibility to cancel download.
    /// </summary>
    /// <remarks>Possibility to cancel download</remarks>
    public event EventHandler<ImageshopImageEventArgs> OnImageDownloading
    {
        add => Events.AddHandler(GetEventKey(ImageDownloadingEvent), value);
        remove => Events.RemoveHandler(GetEventKey(ImageDownloadingEvent), value);
    }

    /// <summary>
    /// Triggered when sync job running and finds out that image is deleted or revoked
    /// </summary>
    public event EventHandler<ImageshopImageEventArgs> OnImageDeleted
    {
        add => Events.AddHandler(GetEventKey(ImageDeletedEvent), value);
        remove => Events.RemoveHandler(GetEventKey(ImageDeletedEvent), value);
    }

    internal virtual bool RaiseImageshopEvent(string imageshopEventsName, object sender, ImageshopImageEventArgs eventArgs)
    {
        EventHandler<ImageshopImageEventArgs> eventHandler = Events[GetEventKey(imageshopEventsName)] as EventHandler<ImageshopImageEventArgs>;
        if (eventHandler != null)
            _logger.Information($"RaiseImageshopEvent: {imageshopEventsName} " + eventHandler.Method.Name);
        eventHandler?.Invoke(sender, eventArgs);
        return !eventArgs.CancelAction;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;
        if (_events != null)
        {
            _events.Dispose();
            _events = null;
        }

        if (this != _instance)
            return;
        _instance = null;
    }

}