using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Entities
{
    public class Address
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        public string Street { get; set; }
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid postal code format")]
        public string PostalCode { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public Address() { }
    }
}

