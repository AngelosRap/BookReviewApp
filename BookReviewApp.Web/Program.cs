using BookReviewApp.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddAuthenticationServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Apply migrations + seed
await app.ApplyMigrationsAndSeedAsync();

// HTTP pipeline
if (app.Environment.IsDevelopment())
    app.UseMigrationsEndPoint();
else
    app.UseExceptionHandler("/Home/Error");

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
