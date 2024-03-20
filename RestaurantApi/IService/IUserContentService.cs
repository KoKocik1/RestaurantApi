using System;
using System.Security.Claims;

namespace RestaurantApi.IService
{
	public interface IUserContentService
	{
        ClaimsPrincipal User { get; }
        int? GetUserId { get; }

    }
}

