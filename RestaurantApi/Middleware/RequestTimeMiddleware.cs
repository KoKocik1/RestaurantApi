using System;
using System.Diagnostics;
using RestaurantApi.Exceptions;

namespace RestaurantApi.Middleware
{
	public class RequestTimeMiddleware : IMiddleware
    {
        private Stopwatch _stopWatch;
        private readonly ILogger<RequestTimeMiddleware> _logger;

		public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
		{
            _logger = logger;
            _stopWatch = new Stopwatch();
		}

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

                _stopWatch.Start();
                await next.Invoke(context);
                _stopWatch.Stop();
                var elapsedMiliseconds=_stopWatch.ElapsedMilliseconds;
                if (elapsedMiliseconds / 1000 > 4)
                {
                    var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {elapsedMiliseconds} ms";
                _logger.LogWarning(message);
                }

        }
    }
}

