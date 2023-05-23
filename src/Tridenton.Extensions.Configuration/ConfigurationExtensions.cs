using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tridenton.Core.Util;

namespace Tridenton.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfiguration<TConfiguration, TImplementation>(this IServiceCollection services, Action<ConfigurationOptionsBuilder> options)
        where TConfiguration : class
        where TImplementation : class, TConfiguration, new()
    {
        return services.AddConfiguration<TConfiguration, TImplementation>(options, out _);
    }

    public static IServiceCollection AddConfiguration<TConfiguration, TImplementation>(this IServiceCollection services, Action<ConfigurationOptionsBuilder> options, out TConfiguration configuration)
        where TConfiguration : class
        where TImplementation : class, TConfiguration, new()
    {
        var settings = new ConfigurationOptionsBuilder();
        options.Invoke(settings);

        var conf = settings.AppsettingsConfiguration;

        if (conf is null)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(settings.JsonFilePath, $"Json file was not specified");

            var corePath = Path.GetDirectoryName((settings.Assembly ?? Assembly.GetExecutingAssembly()).Location)!;

            var path = Path.Combine(corePath, settings.JsonFilePath);

            conf = new ConfigurationBuilder().AddJsonFile(path).Build();
        }

        var implementation = new TImplementation();

        var properties = typeof(TConfiguration).GetProperties();

        for (long i = 0; i < properties.LongLength; i++)
        {
            var property = properties[i];

            var value = conf.GetSection(property.Name).Get(property.PropertyType);

            ReflectionManager.SetItemValue(implementation, property, value);
        }

        services.AddSingleton<TConfiguration>(implementation);

        configuration = implementation;

        return services;
    }
}