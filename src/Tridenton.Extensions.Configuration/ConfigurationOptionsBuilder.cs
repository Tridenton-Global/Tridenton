using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Tridenton.Extensions.Configuration;

public sealed class ConfigurationOptionsBuilder
{
    internal IConfiguration? AppsettingsConfiguration { get; private set; }

    internal Assembly? Assembly { get; private set; }

    internal string JsonFilePath { get; private set; }

    internal ConfigurationOptionsBuilder()
    {
        JsonFilePath = string.Empty;
    }

    public ConfigurationOptionsBuilder FromAppsettingsJson(IConfiguration configuration)
    {
        AppsettingsConfiguration = configuration;
        return this;
    }

    public ConfigurationOptionsBuilder FromAssembly(Assembly assembly)
    {
        Assembly = assembly;
        return this;
    }

    public ConfigurationOptionsBuilder FromJson(params string[] paths)
    {
        JsonFilePath = Path.Combine(paths);
        return this;
    }
}