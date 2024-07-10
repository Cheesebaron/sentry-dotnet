using Sentry.Maui.Essentials.Internal;

namespace Sentry.Maui.Essentials;

public static class ServiceCollectionExtensions
{
    public static void AddSentry(
        this IServiceCollection serviceCollection,
        Action<SentryMauiEssentialsOptions>? configureOptions)
    {
        if (configureOptions != null)
        {
            serviceCollection.Configure(configureOptions);
        }

        serviceCollection.AddLogging();
        serviceCollection.AddSingleton<ILoggerProvider, SentryMobileLoggerProvider>();
        serviceCollection.AddSingleton<IConfigureOptions<SentryMauiEssentialsOptions>, SentryMauiEssentialsOptionsSetup>();

        serviceCollection.AddSentry<SentryMauiEssentialsOptions>();
    }
}
