using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SocialNetworkApi.Data;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.IntegrationTests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        private Mock<IUnitOfWork> _unitOfWork;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            _factory = new WebApplicationFactory<Startup>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                });
            }).CreateClient();
        }

        [Test]
        public async Task Register_ReturnsRegisteredUser()
        {
            var register = new UserRegisterModel
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "username",
                Email = "john.doe@email.com",
                Password = "testPassword123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(register));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.PostAsync("/api/users/register", content);
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(resultString);

            result.Id.Should().NotBeNullOrWhiteSpace();
            result.Email.Should().Be(register.Email);
        }
    }
}