using System;
using FluentValidation;
using RestaurantApi.Entities;
using RestaurantApi.Models;

namespace RestaurantApi.Validators
{
	public class RestaurantQueryWalidator : AbstractValidator<RestaurantQuery>
	{ 
	private int[] allowPageSizes = new[] { 5, 10, 15 };

		private string[] allowSortByColumnNames = {
			nameof(Restaurant.Name),
			nameof(Restaurant.Category),
			nameof(Restaurant.Description),
		};

		public RestaurantQueryWalidator()
		{
			RuleFor(r => r.PageNumebr).GreaterThanOrEqualTo(1);
			RuleFor(r => r.PageSize).Custom((value, context) =>
			{
				if (!allowPageSizes.Contains(value))
				{
					context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowPageSizes)}");
				}
			});
			RuleFor(r => r.SortBy)
				.Must(value => string.IsNullOrEmpty(value) || allowSortByColumnNames.Contains(value))
				.WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowSortByColumnNames)}]");
		}
	}
}

