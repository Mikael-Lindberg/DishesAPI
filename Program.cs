using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
    builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/dishes", async (DishesDbContext dishesDbContext,
    ClaimsPrincipal claimsPrincipal,
    IMapper mapper, 
    string? name) =>
{
    Console.WriteLine($"User authenticated? {claimsPrincipal.Identity?.IsAuthenticated}");

    return mapper.Map<IEnumerable<DishDto>> (await dishesDbContext.Dishes
        .Where(d => name == null || d.Name.Contains(name))
        .ToListAsync());
});

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext dishesDbContext, IMapper mapper, Guid dishId) =>
{
    return mapper.Map<DishDto>(await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId));
});
app.MapGet("/dishes/{dishName}", async (DishesDbContext dishesDbContext, IMapper mapper, string dishName) =>
{
    return mapper.Map<DishDto>(await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName));
});

app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext dishesDbContext, IMapper mapper, Guid dishId) =>
{
    return mapper.Map<IEnumerable<IngredientDto>>((await dishesDbContext.Dishes
        .Include(d => d.Ingredients)
        .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients);
});

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>
    ().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();
