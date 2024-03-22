
namespace DishesAPI.EndpointFilters;

public class DishisLockedFilter : IEndpointFilter
{
    private readonly Guid _lockedDishId;

    public DishisLockedFilter(Guid lockedDishId)
    {
        _lockedDishId = lockedDishId;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var dishId = context.GetArgument<Guid>(2);
        var rendangId = new Guid("fd630a57-2352-4731-b25c-db9cc7601b16");

        if (dishId == _lockedDishId)
        {
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "Dish is perfect and cannot be changed.",
                Detail = "You cannot update or delete perfection."
            });
        }

        var result = await next.Invoke(context);
        return result;
    }
}
