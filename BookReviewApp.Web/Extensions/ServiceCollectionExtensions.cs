using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        return services;
    }

    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<Context>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddDatabaseDeveloperPageExceptionFilter();
        return services;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddDefaultIdentity<AppUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<Context>();

        return services;
    }
}
