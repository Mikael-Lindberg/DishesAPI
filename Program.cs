using DishesAPI.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
    builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/dishes", async (DishesDbContext dishesDbContext) =>
{
    return await dishesDbContext.Dishes.ToListAsync();
});

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>
    ().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();
