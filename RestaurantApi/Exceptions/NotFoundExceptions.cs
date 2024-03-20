using System;
namespace RestaurantApi.Exceptions
{
	public class NotFoundExceptions : Exception
	{
		public NotFoundExceptions(string message) :base(message)
		{
		}
	}
}

