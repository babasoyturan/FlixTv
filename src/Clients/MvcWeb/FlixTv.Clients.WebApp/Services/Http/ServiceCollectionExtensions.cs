using FlixTv.Clients.WebApp.Options;
using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Implementations;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Http.Headers;

namespace FlixTv.Clients.WebApp.Services.Http
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFlixApi(this IServiceCollection services)
        {
            services.AddTransient<AuthHeaderHandler>();
            services.AddTransient<RefreshTokenHandler>();
            services.AddTransient<CorrelationIdHandler>();

            // Polly siyasətləri (retry + circuit breaker)
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * Math.Pow(2, i)));

            var breaker = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

            // "flix-auth" – yalnız Auth üçün (refresh loop-un qarşısını almaq üçün handlers minimal)
            services.AddHttpClient("flix-auth", (sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(opt.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            //.AddPolicyHandler(retry)
            //.AddPolicyHandler(breaker);

            // "flix-api" – bütün digər çağırışlar üçün
            services.AddHttpClient("flix-api", (sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(opt.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<CorrelationIdHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<RefreshTokenHandler>()
            .AddPolicyHandler((sp, req) =>
                req.Method == HttpMethod.Get ? retry : Polly.Policy.NoOpAsync<HttpResponseMessage>())
            .AddPolicyHandler((sp, req) =>
                req.Method == HttpMethod.Get ? breaker : Polly.Policy.NoOpAsync<HttpResponseMessage>());

            // BaseApiClient-i reuse etmək üçün factory
            services.AddScoped(sp =>
                new BaseApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("flix-api")));

            // Domain servisləri (typed deyil, BaseApiClient istifadə edəcəklər)
            services.AddHttpContextAccessor();
            services.AddScoped<IPerRequestTokenStore, PerRequestTokenStore>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMoviesService, MoviesService>();

            return services;
        }
    }
}
