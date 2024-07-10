using Sentry.Extensions.Logging;

namespace Sentry.Maui.Essentials;

public sealed class SentryMauiEssentialsOptions : SentryLoggingOptions
{
    public SentryMauiEssentialsOptions()
    {
        AutoSessionTracking = true;
        NativeEventProcessing = true;
        DetectStartupTime = StartupTimeDetectionMode.Fast;
#if !PLATFORM_NEUTRAL
        CacheDirectoryPath = Microsoft.Maui.Storage.FileSystem.CacheDirectory;
#endif
    }

    /// <summary>
    /// Automatically attaches a screenshot of the app at the time of the event capture.
    /// </summary>
    /// <remarks>
    /// Make sure to only enable this feature if no sensitive data, such as PII, can be visible on the screen.
    /// Screenshots can be removed from some specific events during BeforeSend through the Hint.
    /// </remarks>
    public bool AttachScreenshot { get; set; }

    /// <summary>
    /// Automatically add extra native information at the time of event capture.
    /// </summary>
    public bool NativeEventProcessing { get; set; }
}
