using DishesAPI.EndpointFilters;
using DishesAPI.EndpointHandlers;

namespace DishesAPI.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var dishesEndpoints = endpointRouteBuilder.MapGroup("/dishes");
        var dishWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}");

        dishesEndpoints.MapGet("", DishesHandlers.GetDishesAsync);
        dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync).WithName("GetDish");
        dishesEndpoints.MapGet("/{dishName}", DishesHandlers.GetDishByNameAsync);
        dishesEndpoints.MapPost("", DishesHandlers.CreateDishAsync);
        dishWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishAsync)
            .AddEndpointFilter(new DishisLockedFilter(
                new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")));
        dishWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishAsync)
            .AddEndpointFilter(new DishisLockedFilter(
                new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")));
    }

    public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientsEndpoints = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}/ingredients");

        ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);
    }
}
