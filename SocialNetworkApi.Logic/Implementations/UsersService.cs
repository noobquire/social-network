using SocialNetworkApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Extensions;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SocialNetworkApi.Services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IProfilesService _profilesService;

        public UsersService(UserManager<User> userManager, IConfiguration configuration, IProfilesService profilesService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _profilesService = profilesService;
        }

        public async Task<UserDto> RegisterAsync(UserRegisterModel registerModel)
        {
            var newUser = new User
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                UserName = registerModel.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newUser, registerModel.Password);

            if (result.Succeeded)
            {
                var profile = await _profilesService.CreateAsync(newUser.Id.ToString());
                var user = await _userManager.FindByEmailAsync(newUser.Email);
                user.ProfileId = new Guid(profile.Id);
                await _userManager.UpdateAsync(user);
                return user.ToDto();
            }
            else
            {
                var message = string.Join(" ", result.Errors.Select(e => e.Description));
                throw new RegisterUserException(message);
            }
        }

        public async Task<JwtToken> LoginAsync(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                return null;
            }
            var passwordIsCorrect = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!passwordIsCorrect)
            {
                return null;
            }
            if (user.IsDeleted)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpirationTime = token.ValidTo
            };
        }

        public async Task<UserDto> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user?.ToDto();
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.ToDto();
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return false;
            }

            user.IsDeleted = true;
            

            await _userManager.UpdateAsync(user);
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _profilesService.DeleteByIdAsync(user.ProfileId.ToString());

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(bool withDeleted = false)
        {
            var users = await _userManager.Users.ToListAsync();
            if (!withDeleted)
            {
                users = users.Where(u => !u.IsDeleted).ToList();
            }
            return users.Select(user => user.ToDto());
        }

        public async Task<bool> ReinstateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsDeleted)
            {
                return false;
            }
            user.IsDeleted = false;
            await _userManager.UpdateAsync(user);
            await _userManager.SetLockoutEnabledAsync(user, false);
            await _profilesService.ReinstateAsync(user.ProfileId.ToString());

            return true;
        }
    }
}
