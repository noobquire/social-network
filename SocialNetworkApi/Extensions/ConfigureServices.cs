using Microsoft.Extensions.DependencyInjection;
using SocialNetworkApi.Services.Implementations;
using SocialNetworkApi.Services.Interfaces;

namespace SocialNetworkApi.Extensions
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IProfilesService, ProfilesService>();
            services.AddScoped<IImagesService, ImagesService>();
            services.AddScoped<IPostsService, PostsService>();
            return services;
        }
    }
}
