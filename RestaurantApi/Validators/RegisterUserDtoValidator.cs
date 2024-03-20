using System;
using FluentValidation;
using RestaurantApi.Entities;
using RestaurantApi.Models;

namespace RestaurantApi.Validators
{
	public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
	{
        public RegisterUserDtoValidator(RestaurantDBContext dbContext) { 
		
			RuleFor(x=>x.Email)
				.NotEmpty()
				.EmailAddress();

			RuleFor(x => x.Password)
				.MinimumLength(6);

			RuleFor(x => x.ConfirmedPassword == x.Password);

			RuleFor(x => x.Email)
				.Custom((value, context) =>
				{
					var emailInUse=dbContext.Users.Any(u => u.Email == value);
					if (emailInUse)
					{
						context.AddFailure("Email", "That email is taken");
					}
                });
		}
	}
}

