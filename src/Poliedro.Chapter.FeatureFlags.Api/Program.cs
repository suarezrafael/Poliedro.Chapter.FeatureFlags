using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Poliedro.Chapter.FeatureFlags.Api.Data;
using Poliedro.Chapter.FeatureFlags.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("FeatureFlagsDemo"));

builder.Services.AddFeatureManagement();

builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
