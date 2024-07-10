namespace Sentry.Maui.Essentials.Internal;

internal sealed class SentryMauiEssentialsOptionsSetup : IConfigureOptions<SentryMauiEssentialsOptions>
{
    public void Configure(SentryMauiEssentialsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

#if __ANDROID__ || __IOS__
        options.Native.AttachScreenshot = options.AttachScreenshot;
#endif

        options.InitializeSdk = true;

        // Global Mode makes sense for client apps
        options.IsGlobalModeEnabled = true;

        if (options.AttachScreenshot)
        {
            options.AddEventProcessor(new SentryMauiEssentialsScreenshotsProcessor(options));
        }

#if __IOS__ || __ANDROID__
        if (options.NativeEventProcessing)
        {
            options.AddEventProcessor(new NativeEventProcessor(options));
        }
#endif

        options.AddEventProcessor(new MobileEventProcessor(options));

#if !PLATFORM_NEUTRAL
        options.NetworkStatusListener = new MobileNetworkStatusListener(Connectivity.Current, options);
#endif
    }
}
