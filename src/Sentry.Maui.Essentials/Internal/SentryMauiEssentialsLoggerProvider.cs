using Sentry.Extensions.Logging;

namespace Sentry.Maui.Essentials.Internal;

[ProviderAlias("Sentry")]
internal sealed class SentryMauiEssentialsLoggerProvider(IOptions<SentryLoggingOptions> options, IHub hub)
    : SentryLoggerProvider(options, hub);
