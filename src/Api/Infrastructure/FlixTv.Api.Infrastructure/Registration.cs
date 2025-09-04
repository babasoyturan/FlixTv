using FlixTv.Api.Application.Interfaces.Tokens;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Api.Infrastructure.Seeding;
using FlixTv.Api.Infrastructure.Seeding.Options;
using FlixTv.Api.Infrastructure.Seeding.Tmdb;
using FlixTv.Api.Infrastructure.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Infrastructure
{
    public static class Registration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenSettings>(configuration.GetSection("JWT"));
            services.AddTransient<ITokenService, TokenService>();

            services.Configure<TmdbOptions>(configuration.GetSection("Tmdb"));
            services.AddHttpClient<TmdbClient>((sp, http) =>
            {
                var opt = sp.GetRequiredService<IOptions<TmdbOptions>>().Value;
                http.BaseAddress = new Uri("https://api.themoviedb.org/3/");
                http.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {opt.ApiReadAccessToken}");
            });

            // Seeding options
            services.Configure<MoviesSeedingOptions>(configuration.GetSection("Seeding:Movies"));
            services.Configure<SimilaritiesSeedingOptions>(configuration.GetSection("Seeding:Similarities"));

            var moviesSeed = configuration.GetSection("Seeding:Movies").Get<MoviesSeedingOptions>() ?? new();
            var simSeed = configuration.GetSection("Seeding:Similarities").Get<SimilaritiesSeedingOptions>() ?? new();

            if (moviesSeed.Enabled)
                services.AddHostedService<MovieSeeder>();

            if (simSeed.Enabled)
                services.AddHostedService<SimilarMoviesSeeder>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]!)),
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    ClockSkew = TimeSpan.Zero
                };

                opt.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        var userManager = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

                        var userId = ctx.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (string.IsNullOrEmpty(userId))
                        {
                            ctx.Fail("No user id.");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user is null || user.IsBanned)
                        {
                            ctx.Fail("User invalid or banned.");
                            return;
                        }

                        var currentStamp = await userManager.GetSecurityStampAsync(user);
                        var tokenStamp = ctx.Principal.FindFirst("security_stamp")?.Value;

                        if (string.IsNullOrEmpty(tokenStamp) ||
                            !string.Equals(currentStamp, tokenStamp, StringComparison.Ordinal))
                        {
                            ctx.Fail("Security stamp mismatch.");
                            return;
                        }
                    }
                };
            });
        }
    }
}
