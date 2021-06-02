using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.IntegrationTests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            _factory = new WebApplicationFactory<Startup>();
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                        options.DefaultScheme = "Test";
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                    services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);
                });
            }).CreateClient();

            var register = new UserRegisterModel
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "username",
                Email = "john.doe@email.com",
                Password = "testPassword123"
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
                userService.RegisterAsync(register).Wait();
            }

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Register_ReturnsRegisteredUser()
        {
            // Arrange
            var register = new UserRegisterModel
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john.doe1@email.com",
                Password = "testPassword123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(register));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Act
            var response = await _client.PostAsync("/api/users/register", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(resultString);

            result.Id.Should().NotBeNullOrWhiteSpace();
            result.Email.Should().Be(register.Email);
        }

        [Test]
        public async Task Login_ReturnsJwtToken()
        {
            // Arrange
            var login = new LoginModel
            {
                Email = "john.doe@email.com",
                Password = "testPassword123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(login));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Act
            var response = await _client.PostAsync("/api/users/login", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<JwtToken>(resultString);

            result.Token.Should().NotBeNullOrWhiteSpace();
            result.ExpirationTime.Should().BeAfter(DateTime.UtcNow);
        }

        [Test]
        public async Task GetById_ReturnsUser()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            var user = await userService.GetByEmailAsync("john.doe@email.com");

            // Act
            var response = await _client.GetAsync($"/api/users/{user.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(resultString);

            result.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task GetAll_ReturnsUserList()
        {
            // Act
            var response = await _client.GetAsync($"/api/users");

            // Assert
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResponse<UserDto>>(resultString);

            result.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task DeleteById_SoftDeletesUser()
        {
            // Arrange
            string userId;
            using (var scope = _factory.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
                var registerUser = new UserRegisterModel()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "deleteName",
                    Email = "john.doe_delete@email.com",
                    Password = "testPassword123"
                };
                var user = await userService.RegisterAsync(registerUser);
                userId = user.Id;
            }
            
            // Act
            var response = await _client.DeleteAsync($"/api/users/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            using (var scope = _factory.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
                var result = await userService.GetByIdAsync(userId);
                result.IsDeleted.Should().BeTrue();
            }
        }

        [Test]
        public async Task ReinstateById_ReinstatesUser()
        {
            string userId;
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
                var registerUser = new UserRegisterModel()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "reinstateName",
                    Email = "john.doe_reinstate@email.com",
                    Password = "testPassword123"
                };
                var user = await userService.RegisterAsync(registerUser);
                userId = user.Id;
                await userService.DeleteByIdAsync(user.Id);
            }
            
            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/reinstate");

            // Assert
            response.EnsureSuccessStatusCode();

            using (var scope = _factory.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
                var result = await userService.GetByIdAsync(userId);
                result.IsDeleted.Should().BeFalse();
            }
        }
    }
}