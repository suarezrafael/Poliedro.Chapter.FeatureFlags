using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Poliedro.Chapter.FeatureFlags.Api.Data;
using Poliedro.Chapter.FeatureFlags.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure Entity Framework Core with In-Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("FeatureFlagsDemo"));

// Configure Feature Management
builder.Services.AddFeatureManagement();

// Register application services
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();

var app = builder.Build();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
