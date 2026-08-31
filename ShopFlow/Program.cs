using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ShopFlow.Data;
using ShopFlow.Features.AddToCart;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ShopFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapAddToCartEndpoint();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopFlowDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
