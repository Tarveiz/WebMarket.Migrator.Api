namespace WebMarket.Migrator.Settings.Secrets;

/// <summary>
/// Секрет подключения к PostgreSQL
/// </summary>
public sealed class PgSqlSecret
{
    public required string Host { get; init; }

    public required int Port { get; init; }

    public required string Database { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }
}
