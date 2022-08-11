﻿using DotNetCoreNotIsolated.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DotNetCoreNotIsolated.Startup))]

namespace DotNetCoreNotIsolated;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<IExceptionHandlingService, ExceptionHandlingService>();
        builder.Services.AddServices();
    }
}