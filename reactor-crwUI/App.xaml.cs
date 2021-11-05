﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using reactor_crwUI.Services;
using reactor_crwUI.Services.Interfaces;
using reactor_crwUI.ViewModel;

using Serilog;
using Serilog.Formatting.Compact;

using System;
using System.Windows;

namespace reactor_crwUI
{
    public partial class App : Application
    {
        private static IHost _Hosting;
        public static IHost Hosting
        {
            get
            {
                if (_Hosting != null) return _Hosting;
                var host_builder = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                    .ConfigureServices(ConfigureServices);
                host_builder.UseSerilog((host, log) => log.
                    MinimumLevel.Information().
                    Enrich.FromLogContext().
                    WriteTo.File(path: "log.txt", 
                                rollingInterval: RollingInterval.Day, 
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}")
                );
                return _Hosting = host_builder.Build();
            }
        }

        public static IServiceProvider Services => Hosting.Services;

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<IConfigService, ConfigService>();
        }
    }
}
