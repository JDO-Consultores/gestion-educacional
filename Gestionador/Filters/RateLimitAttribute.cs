using System;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace Gestionador.Filters
{
    public class RateLimitAttribute : ActionFilterAttribute
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private readonly int _maxRequests;
        private readonly TimeSpan _timeWindow;

        public RateLimitAttribute(int maxRequests = 10, int seconds = 60)
        {
            _maxRequests = maxRequests;
            _timeWindow = TimeSpan.FromSeconds(seconds);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            string cacheKey = $"RateLimit_{ipAddress}";

            var entry = Cache.Get(cacheKey) as RateLimitEntry;

            if (entry == null)
            {
                entry = new RateLimitEntry
                {
                    Count = 1,
                    Expiry = DateTime.UtcNow.Add(_timeWindow)
                };
                Cache.Set(cacheKey, entry, entry.Expiry);
            }
            else
            {
                if (DateTime.UtcNow > entry.Expiry)
                {
                    entry.Count = 1;
                    entry.Expiry = DateTime.UtcNow.Add(_timeWindow);
                    Cache.Set(cacheKey, entry, entry.Expiry);
                }
                else
                {
                    entry.Count++;
                    if (entry.Count > _maxRequests)
                    {
                        filterContext.Result = new ContentResult
                        {
                            Content = "Rate limit exceeded. Try again later.",
                            ContentType = "text/plain"
                        };
                        return;
                    }
                    Cache.Set(cacheKey, entry, entry.Expiry);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private class RateLimitEntry
        {
            public int Count { get; set; }
            public DateTime Expiry { get; set; }
        }
    }
}