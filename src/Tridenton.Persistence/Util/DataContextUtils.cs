using Microsoft.Extensions.DependencyInjection;

namespace Tridenton.Persistence;

public readonly struct DataContextUtils
{
    public static async ValueTask EnsureDataContextCreatedAsync<TDataContext>(IServiceProvider serviceProvider) where TDataContext : DataContext
    {
        var scope = serviceProvider.CreateScope();

        var dataContext = scope.ServiceProvider.GetRequiredService<TDataContext>();

        await dataContext.Database.EnsureCreatedAsync();
    }
}