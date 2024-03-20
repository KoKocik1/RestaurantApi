using System;
namespace RestaurantApi.Models
{
	public class RestaurantQuery
	{
		public int PageNumebr { get; set; }
        public int PageSize { get; set; }
        public string? SearchPhase { get; set; }
		public string? SortBy { get; set; }
		public SortDirection? SortDirection { get; set; }

        public RestaurantQuery()
		{
		}
	}
}

