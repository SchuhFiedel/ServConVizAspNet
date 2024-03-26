using Microsoft.Extensions.DependencyInjection;
using ServConViz.Services;


namespace ServConViz
{
    public static class HttpRequestObserverServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpRequestObserver(
        this IServiceCollection services,
        Action<HttpRequestObserverOptions> setupAction = null)
        {
            services.AddSingleton<HttpRequestObserverService>();

            if (setupAction != null) services.ConfigureHttpRequestObserver(setupAction);

            return services;
        }

        
        public static void ConfigureHttpRequestObserver(
        this IServiceCollection services,
        Action<HttpRequestObserverOptions> setupAction)
        {
            services.Configure(setupAction);
        }
        
        
    }
}
