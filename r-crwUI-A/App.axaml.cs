using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using r_crwUI_A.Interfaces;
using r_crwUI_A.Services;
using r_crwUI_A.ViewModels;

using System;

namespace r_crwUI_A
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static IHost? _Hosting;
        public static IHost Hosting
        {
            get
            {
                if (_Hosting != null) return _Hosting;
                var host_builder = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                    .ConfigureServices(ConfigureServices);
                return _Hosting = host_builder.Build();
            }
        }

        public static IServiceProvider Services => Hosting.Services;

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<HelpWindowViewModel>();
            services.AddTransient<IConfigureProvider, ConfigureProvider>();
            services.AddSingleton<ILogger, Logger>();
        }
    }
}
