namespace Sentry.Maui.Essentials.Internal;

internal sealed class SentryMauiEssentialsScreenshotsProcessor(
        SentryMauiEssentialsOptions options)
    : ISentryEventProcessorWithHint
{
    public SentryEvent? Process(SentryEvent @event) => @event;

    public SentryEvent? Process(SentryEvent @event, SentryHint hint)
    {
        hint.Attachments.Add(new ScreenshotAttachment(options));
        return @event;
    }
}
