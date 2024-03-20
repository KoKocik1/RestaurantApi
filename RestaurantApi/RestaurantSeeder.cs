using System;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;

namespace RestaurantApi
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDBContext _dbContext;

        public RestaurantSeeder(RestaurantDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            
            if (_dbContext.Database.CanConnect())
            {

                var pendingMigration = _dbContext.Database.GetPendingMigrations();
                if(pendingMigration!=null && pendingMigration.Any()) {
                    _dbContext.Database.Migrate();
                }


                if (!_dbContext.Roles.Any())
                {
                    var roles = getRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
                    if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = getRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }
            }
        }
        private IEnumerable<Role> getRoles()
        {
            var roles = new List<Role>()
            {
                new Role(){
                    Name="User"
                },
                new Role(){
                    Name="Manager"
                },
                new Role(){
                    Name="Admin"
                }
            };
            return roles;
        }
        private IEnumerable<Restaurant> getRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name="KFC",
                    Category="Fast Food",
                    Description="KFC (short for Kentucky Fried Chicken)",
                    ContactEmail="contact@kfc.com",
                    ContactNumber="123456789",
                    HasDelivery=true,
                    Dishes=new List<Dish>()
                    {
                        new Dish()
                        {
                            Name="Hot Chicken",
                            Price=10.30M,
                            Description="Hot Chicken meal"
                        },
                        new Dish()
                        {
                            Name="Spicy Chicken",
                            Price=5.30M,
                            Description="Spicy Chicken meal"
                        },
                    },
                    Address=new Address()
                    {
                        City="Krakow",
                        Street="Dluga 5",
                        PostalCode="30-001"
                    }
                },
                new Restaurant()
                {
                    Name="MC Donald",
                    Category="Fast Food",
                    Description="Burgers and cheeseburgers with frites",
                    ContactEmail="contact@mcdonald.com",
                    ContactNumber="987654321",
                    HasDelivery=false,
                    Dishes=new List<Dish>()
                    {
                        new Dish()
                        {
                            Name="Hot Burger",
                            Price=9.30M,
                            Description="Hot Burger meal"
                        },
                        new Dish()
                        {
                            Name="Spicy Burger",
                            Price=4.30M,
                            Description="Spicy Burger meal"
                        },
                    },
                    Address=new Address()
                    {
                        City="Warszawa",
                        Street="Waska 7",
                        PostalCode="50-001"
                    }
                },
            };
            return restaurants;
        }
    }
}

