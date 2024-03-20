using System;
using RestaurantApi.Models;

namespace RestaurantApi.IService
{
	public interface IDishService
	{
        int Create(int restaurantId, CreateDishDto dto);
        List<DishDto> GetAll(int restaurantId);
        DishDto GetById(int restaurantId, int dishId);
        void RemoveAll(int restaurantId);
        void RemoveById(int restaurantId, int dishId);

    }
}

