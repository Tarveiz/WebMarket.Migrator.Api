namespace WebMarket.Migrator.Settings.Constants;

/// <summary>
/// Константы мигратора
/// </summary>
internal static class MigratorConstants
{
    public const string JournalSchema = "system";

    public const string JournalTable = "schema_versions";

    /// <summary>
    /// Интересная идея. Благодаря этому числу, блочиться возможность мигратора накатывать миграции из нескольких подов на уровне контура.
    /// Мигратор не сможет выполнить миграции "одновременно" или во время выполнения одной из миграций, пока она не будет завершена.
    /// </summary>
    public const long LockKey = 0x5745424D41524B54;
}
