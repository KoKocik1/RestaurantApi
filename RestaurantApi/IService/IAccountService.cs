using System;
using RestaurantApi.Models;

namespace RestaurantApi.IService
{
	public interface IAccountService
	{
        void RegisterUser(RegisterUserDto dto);
        string GenerateJwt(LoginDto dto);
    }
}

