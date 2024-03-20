using System;
using System.Security.Claims;
using RestaurantApi.Models;

namespace RestaurantApi.IService
{
	public interface IRestaurantService
	{
		RestaurantDto GetById(int id);
        PageResult<RestaurantDto> GetAll(RestaurantQuery query);
        public int Create(CreateRestaurantDto dto);

        void Delete(int id);
        public void Update(int id, UpdateRestaurantDto dto);

    }
}

