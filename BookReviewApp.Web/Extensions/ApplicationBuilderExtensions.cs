using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<Context>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            await context.Database.MigrateAsync();
            await DbSeeder.SeedAsync(context, userManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }
}
