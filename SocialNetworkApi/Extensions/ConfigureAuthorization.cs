using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SocialNetworkApi.Authorization.Handlers;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Extensions
{
    public static class ConfigureAuthorization
    {
        public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SameUserAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SameProfileUserAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SameImageOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SamePostAuthorAuthorizationHandler>();
            return services;
        }

        public static IServiceCollection AddAuthorizationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<SocialNetworkDbContext>()
                .AddDefaultTokenProviders();

            // Adding Authentication  
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })

                // Adding Jwt Bearer  
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                    };
                });
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SameOrAdminUser", policy => policy.Requirements.Add(new SameUserRequirement(true)));
                options.AddPolicy("SameUser", policy => policy.Requirements.Add(new SameUserRequirement(false)));
                options.AddPolicy("ProfileOwner", policy => policy.Requirements.Add(new SameUserRequirement(false)));
            });
            return services;
        }
    }
}
