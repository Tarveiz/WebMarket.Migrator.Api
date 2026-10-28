namespace WebMarket.Migrator.Sql;

/// <summary>
/// Служебные запросы мигратора
/// </summary>
internal static class MigratorSql
{
    public const string AcquireLockAndEnsureJournalSchema = """
        select pg_advisory_lock(@key);
        create schema if not exists system;
        """;
}
