using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi.Entities;

namespace RestaurantApi.Authorization
{
	public class MinimumTwoRestaurantHandler: AuthorizationHandler<MinimumTwoRestaurant>
	{
        private readonly ILogger<MinimumTwoRestaurantHandler> _logger;
        private readonly RestaurantDBContext _dbContext;
		public MinimumTwoRestaurantHandler(ILogger<MinimumTwoRestaurantHandler> logger,
            RestaurantDBContext dBContext)
		{
            _logger = logger;
            _dbContext = dBContext;
		}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumTwoRestaurant requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier).Value);
            var countRestaurant = _dbContext.Restaurants
                .Count(r => r.CreatedById == userId);

            if (countRestaurant>=requirement.MinimumRestaurantCreated)
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation("Create less than 2 resstaurants");
            }

            return Task.CompletedTask;
        }
    }
}

