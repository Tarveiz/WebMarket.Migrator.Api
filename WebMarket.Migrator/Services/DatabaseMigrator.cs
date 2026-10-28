using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Logging;
using Npgsql;
using WebMarket.Migrator.Exceptions;
using WebMarket.Migrator.Helpers.Mappers;
using WebMarket.Migrator.Services.Interfaces;
using WebMarket.Migrator.Settings.Constants;
using WebMarket.Migrator.Settings.Secrets;
using WebMarket.Migrator.Sql;

namespace WebMarket.Migrator.Services;

/// <summary>
/// Миграция БД
/// </summary>
public sealed partial class DatabaseMigrator(PgSqlSecret pgSqlSecret, ILogger<DatabaseMigrator> logger) : IDatabaseMigrator
{
    private static readonly Lazy<IReadOnlyList<SqlScript>> Scripts = new(() => MigrationScriptCatalog.Load(typeof(DatabaseMigrator).Assembly));

    public async Task<IReadOnlyList<string>> MigrateAsync(CancellationToken ct = default)
    {
        var connectionString = pgSqlSecret.ToConnectionString();

        await using var lockConnection = new NpgsqlConnection(new NpgsqlConnectionStringBuilder(connectionString) { Pooling = false }.ConnectionString);
        await lockConnection.OpenAsync(ct);

        await using (var command = new NpgsqlCommand(MigratorSql.AcquireLockAndEnsureJournalSchema, lockConnection))
        {
            command.Parameters.AddWithValue("key", MigratorConstants.LockKey);
            await command.ExecuteNonQueryAsync(ct);
        }

        var result = DeployChanges.To.PostgresqlDatabase(connectionString)
            .WithScripts(Scripts.Value)
            .JournalToPostgresqlTable(MigratorConstants.JournalSchema, MigratorConstants.JournalTable)
            .WithTransactionPerScript()
            .WithVariablesDisabled()
            .LogToNowhere()
            .Build()
            .PerformUpgrade();

        if (!result.Successful)
            throw new MigrationException(result.ErrorScript?.Name, result.Error);

        var applied = result.Scripts.Select(s => s.Name).ToArray();

        if (applied.Length == 0)
            LogDatabaseUpToDate();
        else
            LogMigrationCompleted(applied.Length, applied);

        return applied;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Database is up to date")]
    private partial void LogDatabaseUpToDate();

    [LoggerMessage(Level = LogLevel.Information, Message = "Database migration completed, applied {Count} scripts: {Scripts}")]
    private partial void LogMigrationCompleted(int count, string[] scripts);
}
