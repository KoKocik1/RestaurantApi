using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Exceptions;
using RestaurantApi.IService;
using RestaurantApi.Models;

namespace RestaurantApi.Services
{
	public class DishService : IDishService
    {
		private readonly RestaurantDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;


        public DishService(RestaurantDBContext dBContext, IMapper mapper, ILogger<DishService> logger)
		{
			_dbContext = dBContext;
            _mapper = mapper;
            _logger = logger;
        }

        private Restaurant getRestaurantById(int restaurantId)
        {
            var restaurant = _dbContext.Restaurants.Include(r => r.Dishes).FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant is null)
                throw new NotFoundExceptions("Restaurant not found");

            return restaurant;
        }
        private Dish getDishById(int dishId, int restaurantId)
        {
            var dish = _dbContext.Dishes.FirstOrDefault(r => r.Id == dishId);
            if (dish is null || dish.RestaurantId != restaurantId)
                throw new NotFoundExceptions("Dish not found");
            return dish;

        }

        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = getRestaurantById(restaurantId);

            var dishEntity = _mapper.Map<Dish>(dto);
            dishEntity.RestaurantId = restaurantId;

            _dbContext.Dishes.Add(dishEntity);
            _dbContext.SaveChanges();

            return dishEntity.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = getRestaurantById(restaurantId);

            var dish = getDishById(dishId, restaurantId);

            var dishDto = _mapper.Map<DishDto>(dish);

            return dishDto;
        }
        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = getRestaurantById(restaurantId);

            var dishesDto = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishesDto;
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = getRestaurantById(restaurantId);
            _dbContext.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();

        }
        public void RemoveById(int restaurantId, int dishId)
        {
            var restaurant = getRestaurantById(restaurantId);

            var dish = getDishById(dishId, restaurantId);

            _dbContext.Remove(dish);
            _dbContext.SaveChanges();
        }
    }
}

