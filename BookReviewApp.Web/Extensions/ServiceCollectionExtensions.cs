using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Services;
using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using BookReviewApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<Context>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IReviewService, ReviewService>();


        return services;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<Context>();

        services.AddControllersWithViews();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
        });

        return services;
    }

    public static async Task InitializeRolesAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var manager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var existingRoles = manager!.Roles.Select(x => x.Name).ToList();
            var rolesToAdd = new[] { UserRoles.ADMIN };

            foreach (var role in rolesToAdd)
            {
                if (!existingRoles.Contains(role))
                {
                    await manager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
