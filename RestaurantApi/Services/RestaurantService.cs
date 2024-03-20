using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Authorization;
using RestaurantApi.Entities;
using RestaurantApi.Exceptions;
using RestaurantApi.IService;
using RestaurantApi.Models;

namespace RestaurantApi.Services
{
	public class RestaurantService : IRestaurantService
	{
		private readonly RestaurantDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContentService _userContextService;
        public RestaurantService(RestaurantDBContext dBContext,
            IMapper mapper,
            ILogger<RestaurantService> logger,
            IAuthorizationService authorizationService,
            IUserContentService userContentService)
		{
			_dbContext = dBContext;
            _mapper = mapper;
            _logger = logger;
            _userContextService = userContentService;
            _authorizationService = authorizationService;
        }

		public RestaurantDto GetById(int id)
		{
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

			if (restaurant is null) throw new DirectoryNotFoundException("Restaurant not found");

            var restaurantsDto = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantsDto;
        }

        public PageResult<RestaurantDto> GetAll(RestaurantQuery query)
        {

            var baseQuery = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPhase == null || (r.Name.ToLower().Contains(query.SearchPhase.ToLower())
                || r.Description.ToLower().Contains(query.SearchPhase.ToLower())));

            if (string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelection = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r=>r.Name},
                    {nameof(Restaurant.Description), r=>r.Description},
                    {nameof(Restaurant.Category), r=>r.Category},
                };

                var selectedColumn = columnsSelection[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants= baseQuery
                .Skip(query.PageSize * (query.PageNumebr - 1))
                .Take(query.PageSize)
                .ToList();

            var totalItemsCount = baseQuery.Count();

            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PageResult<RestaurantDto>(restaurantsDto, totalItemsCount, query.PageSize, query.PageNumebr);
            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
        public void Delete(int id)
        {

            _logger.LogError($"Restaurant id {id} DELETE action invoked");
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null) throw new NotFoundExceptions("Restaurant not found");

            checkAuthorization(_userContextService.User, restaurant, ResourceOperation.Delete);

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public void Update(int id, UpdateRestaurantDto dto) { 
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null) throw new NotFoundExceptions("Restaurant not found");

            checkAuthorization(_userContextService.User, restaurant, ResourceOperation.Update);

            restaurant.Description = dto.Description;
            restaurant.Name = dto.Name;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
        }

        private void checkAuthorization(ClaimsPrincipal user, Restaurant restaurant, ResourceOperation operation)
        {
            var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant,
                new ResourceOperationRequirement(operation)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
        }
	}
}

