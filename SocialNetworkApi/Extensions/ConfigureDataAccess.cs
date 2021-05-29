using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetworkApi.Data;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Data.Repositories;

namespace SocialNetworkApi.Extensions
{
    public static class ConfigureDataAccess
    {
        public static IServiceCollection AddDevelopmentDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SocialNetworkDbContext>(
                options =>
                {
                    options.UseLazyLoadingProxies();
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                })
                .AddRepositories();
            return services;
        }

        public static IServiceCollection AddProductionDataAccess(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<SocialNetworkDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "SocialNetwork.db" };
                var connectionString = connectionStringBuilder.ToString();
                var connection = new SqliteConnection(connectionString);

                options.UseSqlite(connection);
            })
            .AddRepositories();
            return services;
        }

        public static IServiceCollection AddTestDataAccess(this IServiceCollection services)
        {
            services.AddDbContext<SocialNetworkDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            })
            .AddRepositories();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Profile>, ProfileRepository>();
            services.AddScoped<IRepository<Post>, PostRepository>();
            services.AddScoped<IRepository<Chat>, ChatRepository>();
            services.AddScoped<IRepository<Message>, MessageRepository>();
            services.AddScoped<IRepository<Image>, ImageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
