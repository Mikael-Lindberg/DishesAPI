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

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext dishesDbContext, Guid dishId) =>
{
    return await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
});
app.MapGet("/dishes/{dishName}", async (DishesDbContext dishesDbContext, string dishName) =>
{
    return await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);
});

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>
    ().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();
