namespace WebMarket.Migrator.Exceptions;

/// <summary>
/// Ошибка применения скрипта миграции
/// </summary>
public sealed class MigrationException(string? script, Exception? innerException) : Exception($"Migration script '{script}' failed.", innerException)
{
    public string? Script { get; } = script;
}
