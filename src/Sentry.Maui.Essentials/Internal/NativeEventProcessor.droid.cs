#if __ANDROID__
using Android.OS;
using Sentry.Extensibility;
using Sentry.Mobile.Extensions;

namespace Sentry.Maui.Essentials.Internal;

internal sealed class NativeEventProcessor(SentryOptions options) : ISentryEventProcessor
{
    private readonly SentryOptions _options = options;
    private readonly Lazy<AndroidContext> _androidContext = new(() => new AndroidContext());
    private volatile bool _androidContextLoaded = true;

    public SentryEvent? Process(SentryEvent @event)
    {
        if (_androidContextLoaded)
        {
            try
            {
                AndroidContext androidContext = _androidContext.Value;
                @event.Contexts.Device.MemorySize = androidContext.MemorySize;
                @event.Contexts.Device.FreeMemory = androidContext.GetFreeMemory();
                @event.Contexts.Device.StorageSize = androidContext.GetAvailableRom();
                if (androidContext.CpuModel != null)
                    @event.SetTag("cpu.model", androidContext.CpuModel);
            }
            catch (Exception e)
            {
                _options.DiagnosticLogger?.Log(SentryLevel.Error,
                    "Failed to add Android NativeEventProcessor into event", e);
                _androidContextLoaded = false;
            }
        }
        else
        {
            _options.DiagnosticLogger?.LogDebug("Android NativeEventProcessor disabled due to previous error");
        }

        return @event;
    }

#pragma warning disable CA1822
    private sealed class AndroidContext
    {
        internal long? MemorySize { get; }
        internal string? CpuModel { get; }
        private string GetCpuModel()
        {
            var modelKey = "Hardware";
            try
            {
                string? model = null;
                var reader = new Java.IO.RandomAccessFile("/proc/cpuinfo", "r");
                do
                {
                    model = reader.ReadLine();
                } while (model != null && model.Contains(modelKey) == false);

                reader.Close();
                if (model?.Contains(modelKey) == true)
                {
                    return model.Replace($"{modelKey}\t:", "");
                }
            }
            catch
            {
                // Left empty intentionally
            }
            return $"{Build.Board}";
        }

        internal long? GetAvailableRom()
        {
            try
            {
                var statfs = new StatFs(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
                return statfs.AvailableBytes;
            }
            catch
            {
                // Left empty intentionally
            }
            return null;
        }

        internal long? GetFreeMemory()
            => GetMemoryInfo()?.AvailMem;

        internal ActivityManager.MemoryInfo? GetMemoryInfo()
        {
            var activityManager = ActivityManager.FromContext(Application.Context);
            if (activityManager != null)
            {
                var memoryManager = new ActivityManager.MemoryInfo();
                activityManager.GetMemoryInfo(memoryManager);
                return memoryManager;
            }
            return null;
        }

        internal AndroidContext()
        {
            CpuModel = GetCpuModel().FilterUnknownOrEmpty();
            var activityManager = ActivityManager.FromContext(Application.Context);
            if (activityManager != null)
            {
                MemorySize = GetMemoryInfo()?.TotalMem;
            }
        }
    }
#pragma warning restore CA1822
}
#endif
