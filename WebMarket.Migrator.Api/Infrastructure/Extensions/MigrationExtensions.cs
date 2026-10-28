using WebMarket.Migrator.Services.Interfaces;

namespace WebMarket.Migrator.Api.Infrastructure.Extensions;

public static partial class MigrationExtensions
{
    /// <summary>
    /// Приводит структуру БД к актуальной, при ошибке останавливает запуск сервиса
    /// </summary>
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        try
        {
            await app.Services.GetRequiredService<IDatabaseMigrator>().MigrateAsync(app.Lifetime.ApplicationStopping);
        }
        catch (Exception ex) { LogMigrationFailed(app.Logger, ex); throw; }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Database migration failed")]
    private static partial void LogMigrationFailed(ILogger logger, Exception exception);
}
