namespace WebMarket.Migrator.Services.Interfaces;

/// <summary>
/// Миграция структуры общей БД
/// </summary>
public interface IDatabaseMigrator
{
    /// <summary>
    /// Применяет новые и изменённые скрипты, возвращает их имена
    /// </summary>
    Task<IReadOnlyList<string>> MigrateAsync(CancellationToken ct = default);
}
