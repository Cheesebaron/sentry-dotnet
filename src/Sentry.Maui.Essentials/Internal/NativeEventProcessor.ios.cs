using Sentry.Extensibility;

#if __IOS__
namespace Sentry.Maui.Essentials.Internal;

internal sealed class NativeEventProcessor(SentryMauiEssentialsOptions options) : ISentryEventProcessor
{
    private readonly SentryMauiEssentialsOptions _options = options;
    private readonly Lazy<IosContext> _iosContext = new(() => new IosContext());
    private bool _iosContextLoaded = true;

    public SentryEvent? Process(SentryEvent @event)
    {
        if (_iosContextLoaded)
        {
            try
            {
                IosContext iosContext = _iosContext.Value;
                @event.Contexts.Device.MemorySize = iosContext.MemorySize;
                @event.Contexts.Device.StorageSize = iosContext.GetStorageSize();
            }
            catch (Exception ex)
            {
                _options.DiagnosticLogger?.Log(SentryLevel.Error, "Failed to add iOS NativeEventProcessor into event", ex);
                //In case of any failure, this process function will be disabled to avoid throwing exceptions for future events.
                _iosContextLoaded = false;
            }
        }
        else
        {
            _options.DiagnosticLogger?.Log(SentryLevel.Debug, "iOS NativeEventProcessor disabled due to previous error");
        }
        return @event;
    }

#pragma warning disable CA1822
    private sealed class IosContext
    {
        internal long? MemorySize { get; } = (long)NSProcessInfo.ProcessInfo.PhysicalMemory;

        internal long? GetStorageSize()
        {
            NSFileSystemAttributes? personalFolderAttributes =
                NSFileManager.DefaultManager.GetFileSystemAttributes(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal));

            if (personalFolderAttributes == null)
                return null;

            return (long)personalFolderAttributes.FreeSize;
        }
    }
#pragma warning restore CA1822
}
#endif
