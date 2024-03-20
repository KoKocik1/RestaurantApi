using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi.Entities;

namespace RestaurantApi.Authorization
{
	public class ResourceOperationRequirementHandler: AuthorizationHandler<ResourceOperationRequirement, Restaurant>
	{
		public ResourceOperationRequirementHandler()
		{
		}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceOperationRequirement requirement,
            Restaurant restaurant)
        {
            if(requirement.ResourceOperation==ResourceOperation.Read||
                requirement.ResourceOperation == ResourceOperation.Create)
            {
                context.Succeed(requirement);
            }

            var userId= context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (restaurant.CreatedById == int.Parse(userId))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}

