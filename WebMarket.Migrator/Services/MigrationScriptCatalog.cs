using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DbUp.Engine;

namespace WebMarket.Migrator.Services;

/// <summary>
/// Загрузка встроенных скриптов миграции в порядке применения
/// </summary>
internal static partial class MigrationScriptCatalog
{
    private const string VersionedPrefix = "Versioned/";

    private const string RepeatablePrefix = "Repeatable/";

    private const int HashLength = 16;

    [GeneratedRegex(@"^V(?<version>\d{4})__[a-z0-9_]+\.sql$")]
    private static partial Regex VersionedNameRegex();

    [GeneratedRegex(@"^R__[a-z0-9_]+(\.[a-z0-9_]+)?\.sql$")]
    private static partial Regex RepeatableNameRegex();

    /// <summary>
    /// Версионные скрипты по номеру версии, затем повторяемые по имени
    /// </summary>
    public static IReadOnlyList<SqlScript> Load(Assembly assembly)
    {
        var versioned = new SortedList<int, SqlScript>();
        var repeatable = new SortedList<string, SqlScript>(StringComparer.Ordinal);

        foreach (var resource in assembly.GetManifestResourceNames())
        {
            var name = Path.GetFileName(resource.Replace('\\', '/'));

            if (resource.StartsWith(VersionedPrefix, StringComparison.Ordinal))
                AddVersioned(versioned, name, ReadScript(assembly, resource));
            else if (resource.StartsWith(RepeatablePrefix, StringComparison.Ordinal))
                AddRepeatable(repeatable, name, ReadScript(assembly, resource));
        }

        return [.. versioned.Values, .. repeatable.Values];
    }

    private static void AddVersioned(SortedList<int, SqlScript> scripts, string name, string contents)
    {
        var match = VersionedNameRegex().Match(name);

        if (!match.Success)
            throw new InvalidOperationException($"Versioned script '{name}' does not match the 'V{{NNNN}}__description.sql' naming convention.");

        var version = int.Parse(match.Groups["version"].Value);

        if (!scripts.TryAdd(version, new SqlScript(name, contents)))
            throw new InvalidOperationException($"Scripts '{scripts[version].Name}' and '{name}' have the same version {version}.");
    }

    private static void AddRepeatable(SortedList<string, SqlScript> scripts, string name, string contents)
    {
        if (!RepeatableNameRegex().IsMatch(name))
            throw new InvalidOperationException($"Repeatable script '{name}' does not match the 'R__schema.name.sql' naming convention.");

        // Хэш в имени для журнала: изменённый скрипт применяется повторно
        if (!scripts.TryAdd(name, new SqlScript($"{name}#{ComputeHash(contents)}", contents)))
            throw new InvalidOperationException($"Repeatable script '{name}' is duplicated.");
    }

    private static string ReadScript(Assembly assembly, string resource)
    {
        using var reader = new StreamReader(assembly.GetManifestResourceStream(resource)!);
        return reader.ReadToEnd().ReplaceLineEndings("\n").Trim(); // Хэш не зависит от переносов строк ОС
    }

    private static string ComputeHash(string contents) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(contents)))[..HashLength];
}
