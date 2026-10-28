using WebMarket.Migrator.Helpers.Extensions;
using WebMarket.Migrator.Services;
using WebMarket.Migrator.Services.Interfaces;
using WebMarket.Migrator.Settings.Secrets;

namespace WebMarket.Migrator.Api;

public class Program
{
    private const string VaultSecretsDirectory = "/vault/secrets";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        if (Directory.Exists(VaultSecretsDirectory))
            foreach (var file in Directory.EnumerateFiles(VaultSecretsDirectory, "*.json").Order())
                builder.Configuration.AddJsonFile(file, optional: true, reloadOnChange: true);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        Configure(app);

        await app.MigrateDatabaseAsync();
        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddSingleton(configuration.GetRequiredSection(nameof(PgSqlSecret)).Get<PgSqlSecret>()!);
        services.AddSingleton<IDatabaseMigrator, DatabaseMigrator>();
    }

    private static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.MapOpenApi();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}
