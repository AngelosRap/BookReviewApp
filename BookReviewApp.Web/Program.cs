using BookReviewApp.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddAuthenticationServices();

var app = builder.Build();

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Apply migrations + seed
await app.InitializeRolesAsync();
await app.ApplyMigrationsAndSeedAsync();

app.Run();
