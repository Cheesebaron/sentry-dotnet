namespace Sentry.Maui.Essentials.Internal;

internal sealed class MobileEventProcessor(SentryMauiEssentialsOptions options)
    : ISentryEventProcessor
{
    private readonly SentryMauiEssentialsOptions _options = options;

    public SentryEvent? Process(SentryEvent @event)
    {
        @event.Sdk.Name = Constants.SdkName;
        @event.Contexts.Device.ApplyMobileDeviceData(_options.DiagnosticLogger);
        @event.Contexts.OperatingSystem.ApplyMobileOsData(_options.DiagnosticLogger);

        return @event;
    }
}
