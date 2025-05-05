using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using EventPlus.Server.Application.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResult> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.AuthenticateAsync(username, password);
            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Update last login time
            await _userRepository.UpdateLastLoginAsync(user.IdUser);

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Message = "Authentication successful",
                User = user,
                Token = token
            };
        }

        public async Task<AuthResult> RegisterAsync(UserViewModel userViewModel)
        {
            // Check if username is already taken
            var isUnique = await _userRepository.IsUsernameUniqueAsync(userViewModel.Username);
            if (!isUnique)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Username is already taken"
                };
            }

            // Create new user
            var user = new User
            {
                Name = userViewModel.Name,
                Surname = userViewModel.Surname,
                Username = userViewModel.Username,
                Password = userViewModel.Password, // In production, password should be hashed
                LastLogin = DateTime.Now
            };

            await _userRepository.CreateAsync(user);

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Message = "Registration successful",
                User = user,
                Token = token
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Password != currentPassword)
            {
                return false;
            }

            user.Password = newPassword; // In production, password should be hashed
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            return await _userRepository.UpdateLastLoginAsync(userId);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "DefaultSecretKeyWith32Characters1234");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}