namespace Sentry.Maui.Essentials.Internal;

internal sealed class ScreenshotAttachment : SentryAttachment
{
    public ScreenshotAttachment(SentryMauiEssentialsOptions options)
        : this(
            AttachmentType.Default,
            new ScreenshotAttachmentContent(options),
            "screenshot.jpg",
            "image/jpeg")
    {
    }

    private ScreenshotAttachment(
        AttachmentType type,
        IAttachmentContent content,
        string fileName,
        string? contentType)
        : base(type, content, fileName, contentType)
    {
    }
}

internal sealed class ScreenshotAttachmentContent(SentryMauiEssentialsOptions options) : IAttachmentContent
{
    private readonly SentryMauiEssentialsOptions _options = options;

    public Stream GetStream()
    {
        Stream stream = Stream.Null;

        if (!Screenshot.Default.IsCaptureSupported)
        {
            _options.DiagnosticLogger?.LogDebug("Capturing screenshot not supported");
            return stream;
        }

        // Not including this on Windows specific build because on WinUI this can deadlock.
#if __ANDROID__ || __IOS__
        Stream CaptureScreenBlocking()
        {
            // This actually runs synchronously (returning Task.FromResult) on the following platforms:
            // Android: https://github.com/dotnet/maui/blob/3c7b65264d2f341a48db32263a271fd8718cfd23/src/Essentials/src/Screenshot/Screenshot.android.cs#L49
            // iOS: https://github.com/dotnet/maui/blob/3c7b65264d2f341a48db32263a271fd8718cfd23/src/Essentials/src/Screenshot/Screenshot.ios.cs#L49
            IScreenshotResult screen = Screenshot.Default.CaptureAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return screen.OpenReadAsync(ScreenshotFormat.Jpeg).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        if (MainThread.IsMainThread)
        {
            stream = CaptureScreenBlocking();
        }
        else
        {
#if __ANDROID__ //Android does not require UI thread to capture screen but iOS does.
            stream = CaptureScreenBlocking();
#else
            stream = MainThread.InvokeOnMainThreadAsync(async () =>
            {
                IScreenshotResult screen = await Screenshot.Default.CaptureAsync().ConfigureAwait(true);

                return await screen.OpenReadAsync(ScreenshotFormat.Jpeg).ConfigureAwait(true);
            }).ConfigureAwait(false).GetAwaiter().GetResult();
#endif
        }
#endif
        return stream;
    }
}
