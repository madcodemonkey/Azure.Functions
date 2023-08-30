using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TimedJobPattern.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add services needed for DI.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection sc, IConfiguration config)
    {
        ConfigureSectionSettings<ApplicationSettings>(sc, config, ApplicationSettings.SectionName);

        sc.AddSingleton<IMemoryCache, MemoryCache>();

        sc.AddScoped<IWorkerService, WorkerService>();
        sc.AddScoped<IWorkerConfigurationService, WorkerConfigurationService>();

        return sc;
    }



    /// <summary>
    /// Uses reflection to make sure that the user did include all the properties in their local.setting.json file when starting the application.
    /// In production, it would check the Configuration settings for this information and throw errors if they are missing.
    /// </summary>
    /// <param name="sc">The service collection</param>
    /// <param name="config">The configuration object.</param>
    /// <param name="sectionName">The section name where the setting is located.</param>
    /// <param name="checkProperties"></param>
    /// <param name="optionalPropertyNames"></param>
    /// <returns>IOptions of T is returned so that you can do something with them.</returns>
    /// <remarks>
    /// Notes
    /// - Note 1: The Microsoft.Extensions.Options.ConfigurationExtensions NuGet package to get IConfiguration, IServiceCollection.Bind and IServiceCollection.Configure.
    /// 
    /// - Note 2: If you only needed IConfiguration, you could just use the Microsoft.Extensions.Configuration.Abstractions NuGet package.
    /// </remarks>    /// <typeparam name="T"></typeparam>
    private static T ConfigureSectionSettings<T>(IServiceCollection sc, IConfiguration config, 
        string sectionName, bool checkProperties = true, params string[] optionalPropertyNames) where T : class, new()
    {
        var setting = new T();
        if (string.IsNullOrWhiteSpace(sectionName))
            config.Bind(setting);
        else config.GetSection(sectionName).Bind(setting);

        if (checkProperties)
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (optionalPropertyNames.Any(w => w == propertyInfo.Name))
                {
                    continue;
                }

                string sectionAndPropertyName = string.IsNullOrWhiteSpace(sectionName) ? propertyInfo.Name : $"{sectionName}:{propertyInfo.Name}";
                var someSettingAsString = config[sectionAndPropertyName];

                bool isMissing;
                if (propertyInfo.PropertyType == typeof(string))
                {
                    isMissing = string.IsNullOrWhiteSpace(someSettingAsString) || someSettingAsString.StartsWith("---");
                }
                else 
                {
                    isMissing = string.IsNullOrWhiteSpace(someSettingAsString);
                }

                if (isMissing)
                {
                    // If you did add the setting and you are still hitting this error, do the following
                    // 1. Double check your spelling.
                    // 2. Clean the solution and re-run. Sometimes the local.settings.json doesn't get copied to the bin
                    //    directory despite the fact that you have "copy if newer" or "copy always" set. 
                    throw new Exception($"Please add the {sectionAndPropertyName} setting to your " +
                                        $"local.settings.json files, secrets file or if deployed to Azure put it in your configuration settings!");
                }
            }
        }

        sc.Configure<T>(config.GetSection(sectionName));

        return setting;
    }
}