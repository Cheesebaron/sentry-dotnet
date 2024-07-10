using OperatingSystem = Sentry.Protocol.OperatingSystem;

namespace Sentry.Maui.Essentials.Internal;

internal static class MobileOsData
{
    public static void ApplyMobileOsData(this OperatingSystem os, IDiagnosticLogger? logger)
    {
        try
        {
            // https://docs.microsoft.com/dotnet/maui/platform-integration/device/information
            IDeviceInfo deviceInfo = DeviceInfo.Current;
            if (deviceInfo.Platform == DevicePlatform.Unknown)
            {
                // return early so we don't get NotImplementedExceptions (i.e., in unit tests, etc.)
                return;
            }

            os.Version = deviceInfo.VersionString;
            os.Name = deviceInfo.Platform.ToString();

        }
        catch (Exception ex)
        {
            // Log, but swallow the exception so we can continue sending events
            logger?.LogError(ex, "Error getting MAUI OS information.");
        }
    }
}
