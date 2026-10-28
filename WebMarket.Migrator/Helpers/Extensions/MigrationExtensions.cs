using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebMarket.Migrator.Services;
using WebMarket.Migrator.Services.Interfaces;

namespace WebMarket.Migrator.Helpers.Extensions;

public static partial class MigrationExtensions
{
    /// <summary>
    /// Приводит структуру БД к актуальной, при ошибке останавливает запуск сервиса
    /// </summary>
    public static async Task MigrateDatabaseAsync(this IHost host)
    {
        try
        {
            await host.Services.GetRequiredService<IDatabaseMigrator>().MigrateAsync(host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping);
        }
        catch (Exception ex) { LogMigrationFailed(host.Services.GetRequiredService<ILogger<DatabaseMigrator>>(), ex); throw; }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Database migration failed")]
    private static partial void LogMigrationFailed(ILogger logger, Exception exception);
}
