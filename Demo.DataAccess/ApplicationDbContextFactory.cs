using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Demo.DataAccess;

//Stock class to allow for DB operations through a separate project
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = ResolveConnectionString();

        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        var environmentConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (!string.IsNullOrWhiteSpace(environmentConnectionString))
        {
            return environmentConnectionString;
        }

        foreach (var appSettingsPath in GetCandidateAppSettingsPaths())
        {
            if (!File.Exists(appSettingsPath))
            {
                continue;
            }

            using var stream = File.OpenRead(appSettingsPath);
            using var document = JsonDocument.Parse(stream);

            if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) &&
                connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection) &&
                !string.IsNullOrWhiteSpace(defaultConnection.GetString()))
            {
                return defaultConnection.GetString()!;
            }
        }

        throw new InvalidOperationException(
            "Could not find ConnectionStrings:DefaultConnection. Set the ConnectionStrings__DefaultConnection environment variable or ensure .NET Demo/appsettings.json exists.");
    }

    private static IEnumerable<string> GetCandidateAppSettingsPaths()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        yield return Path.Combine(currentDirectory, "appsettings.json");
        yield return Path.Combine(currentDirectory, ".NET Demo", "appsettings.json");
        yield return Path.GetFullPath(Path.Combine(currentDirectory, "..", ".NET Demo", "appsettings.json"));
        yield return Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", ".NET Demo", "appsettings.json"));
    }
}
