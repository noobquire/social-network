using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialNetworkApi.Authorization.Handlers;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Data.Repositories;
using SocialNetworkApi.Middleware;
using SocialNetworkApi.Services.Implementations;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;

namespace SocialNetworkApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<SocialNetworkDbContext>(
                options => options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Profile>, ProfileRepository>();
            services.AddScoped<IRepository<Post>, PostRepository>();
            services.AddScoped<IRepository<Chat>, ChatRepository>();
            services.AddScoped<IRepository<Message>, MessageRepository>();
            services.AddScoped<IRepository<Image>, ImageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IProfilesService, ProfilesService>();
            services.AddScoped<IImagesService, ImagesService>();
            services.AddScoped<IPostsService, PostsService>();

            services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SameUserAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SameProfileUserAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SameImageOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SamePostAuthorAuthorizationHandler>();

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
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                    };
                });
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SameOrAdminUserPolicy", policy => policy.Requirements.Add(new SameUserRequirement(true)));
                options.AddPolicy("SameUserPolicy", policy => policy.Requirements.Add(new SameUserRequirement(false)));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<UnhandledExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CreateAdminUser(app.ApplicationServices).Wait();
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
                var registerAdmin = new UserRegisterModel()
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
