using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
namespace IdentityService.Infrastructure.Contexts;


public static class ApplicationDBContextInitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDBContextInitialiser>();

        await initializer.InitialiseAsync();

    }
}

public class ApplicationDBContextInitialiser
{
    private readonly ApplicationDBContext _context;
    public ApplicationDBContextInitialiser(ApplicationDBContext context)
    {
        _context = context; }

    public async Task InitialiseAsync()
    {
        await _context.Database.MigrateAsync();
    }

}