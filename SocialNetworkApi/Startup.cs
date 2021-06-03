using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialNetworkApi.Data;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Extensions;
using SocialNetworkApi.Middleware;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using System;
using System.Threading.Tasks;

namespace SocialNetworkApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRouting(options => options.LowercaseUrls = true);
            if (Environment.IsProduction())
            {
                services.AddProductionDataAccess(Configuration);
            }
            else if (Environment.IsEnvironment("Test"))
            {
                services.AddTestDataAccess();
            }
            else
            {
                services.AddDevelopmentDataAccess(Configuration);
            }
            services.AddServices();
            services.AddAuthorizationHandlers();
            services.AddIdentityConfiguration(Configuration);
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Social Network API v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<UnhandledExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (env.IsProduction())
            {
                CreateDatabase(app);
            }

            CreateAdminUser(app.ApplicationServices).Wait();
        }

        private static void CreateDatabase(IApplicationBuilder app)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<SocialNetworkDbContext>();
            dbContext?.Database.EnsureCreated();
        }

        private async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var usersService = provider.GetRequiredService<IUsersService>();
            var userManager = provider.GetRequiredService<UserManager<User>>();

            var adminRoleExists = await roleManager.Roles.AnyAsync(r => r.Name == "Admin");
            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            var existingUser = await usersService.GetByEmailAsync(Configuration["Admin:Email"]);
            if (existingUser == null)
            {
                var registerAdmin = new UserRegisterModel
                {
                    FirstName = Configuration["Admin:FirstName"],
                    LastName = Configuration["Admin:LastName"],
                    Email = Configuration["Admin:Email"],
                    Password = Configuration["Admin:Password"],
                    Username = Configuration["Admin:Username"]
                };

                await usersService.RegisterAsync(registerAdmin);
            }

            var userToMakeAdmin = await userManager.FindByEmailAsync(Configuration["Admin:Email"]);
            if (!await userManager.IsInRoleAsync(userToMakeAdmin, "Admin"))
            {
                await userManager.AddToRoleAsync(userToMakeAdmin, "Admin");
            }
        }
    }
}
