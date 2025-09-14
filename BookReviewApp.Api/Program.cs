using BookReviewApp.Api.Extensions;
using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// services 
builder.Services.AddDatabaseAndIdentityServices(builder.Configuration);
builder.Services.AddJwtAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
