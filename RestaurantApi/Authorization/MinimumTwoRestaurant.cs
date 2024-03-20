using System;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantApi.Authorization
{
    public class MinimumTwoRestaurant : IAuthorizationRequirement
	{
		public int MinimumRestaurantCreated { get; }
		public MinimumTwoRestaurant(int count)
		{
            MinimumRestaurantCreated = count;
		}
	}
}

