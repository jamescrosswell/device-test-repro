using DeviceRunners.XHarness;
using Microsoft.Maui.LifecycleEvents;
using Sentry.Tests;

namespace Sentry.Maui.Device.TestApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var appBuilder = MauiApp.CreateBuilder()
            .ConfigureLifecycleEvents(life =>
            {
#if __ANDROID__
                life.AddAndroid(android =>
                {
                    android.OnCreate(Platform.Init);
                });
#endif
            })
            .UseXHarnessTestRunner(conf =>
            {
                conf.AddTestAssemblies([
                    typeof(PollingNetworkStatusListenerTest).Assembly,
                ]);
                conf.AddXunit();
            });

        return appBuilder.Build();
    }
}
