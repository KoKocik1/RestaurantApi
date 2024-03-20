using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Models
{
	public class RegisterUserDto
	{
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }

        public int RoleId { get; set; } = 1;
        public RegisterUserDto()
		{
		}
	}
}

