using AutoMapper;
using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.UnitOfWork;
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
    public class RoleBasedAuthService : IRoleBasedAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrganiserRepository _organiserRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IConfiguration _configuration;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public RoleBasedAuthService(
			IUnitOfWork unitOfWork,
			IUserRepository userRepository,
            IOrganiserRepository organiserRepository,
            IAdministratorRepository administratorRepository,
            IConfiguration configuration,
			IMapper mapper)
        {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
            _organiserRepository = organiserRepository;
            _administratorRepository = administratorRepository;
            _configuration = configuration;
        }

        public async Task<AuthResult> AuthenticateAsync(string username, string password)
        {
            // Try to authenticate as a regular user
            var user = await _userRepository.AuthenticateAsync(username, password);
            if (user != null)
            {
                await _userRepository.UpdateLastLoginAsync(user.IdUser);
                return CreateSuccessResult(user.IdUser, user.Username, user.Name, user.Surname, "User");
            }

            // Try to authenticate as an organiser
            var organiser = await _organiserRepository.AuthenticateAsync(username, password);
            if (organiser != null)
            {
                await _organiserRepository.UpdateLastLoginAsync(organiser.IdUser);
                return CreateSuccessResult(organiser.IdUser, organiser.Username, organiser.Name, organiser.Surname, "Organiser");
            }

            // Try to authenticate as an administrator
            var admin = await _administratorRepository.AuthenticateAsync(username, password);
            if (admin != null)
            {
                await _administratorRepository.UpdateLastLoginAsync(admin.IdUser);
                return CreateSuccessResult(admin.IdUser, admin.Username, admin.Name, admin.Surname, "Administrator");
            }

            // No authentication match found
            return new AuthResult
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        public async Task<AuthResult> RegisterUserAsync(UserViewModel userViewModel)
        {
            var AuthStatus = await ValidateFormData(userViewModel.Username, userViewModel.Password);
            if (!AuthStatus.Success)
            {
                return AuthStatus;
            }

            var user = new User
            {
                Name = userViewModel.Name,
                Surname = userViewModel.Surname,
                Username = userViewModel.Username,
                Password = userViewModel.Password, // unhashed
                LastLogin = DateTime.Now
            };

            await _userRepository.CreateAsync(user);

            return CreateSuccessResult(user.IdUser, user.Username, user.Name, user.Surname, "User");
        }

		public async Task<OrganiserViewModel> GetOrganiserByIdAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
			}
			var organiserEntity = await _unitOfWork.Organisers.GetByIdAsync(id);
			return _mapper.Map<OrganiserViewModel>(organiserEntity);
		}

		public async Task<AuthResult> RegisterOrganiserAsync(OrganiserViewModel organiserViewModel)
        {
            var AuthStatus = await ValidateFormData(organiserViewModel.Username, organiserViewModel.Password);
            if (!AuthStatus.Success)
            {
                return AuthStatus;
            }

            var organiser = new Organiser
            {
                Name = organiserViewModel.Name,
                Surname = organiserViewModel.Surname,
                Username = organiserViewModel.Username,
                Password = organiserViewModel.Password, // unshased
                LastLogin = DateTime.Now,
                FollowerCount = organiserViewModel.FollowerCount,
                Rating = organiserViewModel.Rating
            };

            await _organiserRepository.CreateAsync(organiser);

            return CreateSuccessResult(organiser.IdUser, organiser.Username, organiser.Name, organiser.Surname, "Organiser");
        }

        public async Task<AuthResult> RegisterAdministratorAsync(AdministratorViewModel administratorViewModel)
        {
            var AuthStatus = await ValidateFormData(administratorViewModel.Username, administratorViewModel.Password);
            if (!AuthStatus.Success)
            {
                return AuthStatus;
            }

            var administrator = new Administrator
            {
                Name = administratorViewModel.Name,
                Surname = administratorViewModel.Surname,
                Username = administratorViewModel.Username,
                Password = administratorViewModel.Password, // unahased
                LastLogin = DateTime.Now
            };

            await _administratorRepository.CreateAsync(administrator);

            return CreateSuccessResult(administrator.IdUser, administrator.Username, administrator.Name, administrator.Surname, "Administrator");
        }

        public async Task<bool> ChangePasswordAsync(int userId, string userType, string currentPassword, string newPassword)
        {
            switch (userType)
            {
                case "User":
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user == null || user.Password != currentPassword)
                    {
                        return false;
                    }
                    user.Password = newPassword; // In production, password should be hashed
                    await _userRepository.UpdateAsync(user);
                    return true;

                case "Organiser":
                    var organiser = await _organiserRepository.GetByIdAsync(userId);
                    if (organiser == null || organiser.Password != currentPassword)
                    {
                        return false;
                    }
                    organiser.Password = newPassword; // In production, password should be hashed
                    await _organiserRepository.UpdateAsync(organiser);
                    return true;

                case "Administrator":
                    var administrator = await _administratorRepository.GetByIdAsync(userId);
                    if (administrator == null || administrator.Password != currentPassword)
                    {
                        return false;
                    }
                    administrator.Password = newPassword; // In production, password should be hashed
                    await _administratorRepository.UpdateAsync(administrator);
                    return true;

                default:
                    return false;
            }
        }

        public async Task<bool> UpdateLastLoginAsync(int userId, string userType)
        {
            switch (userType)
            {
                case "User":
                    return await _userRepository.UpdateLastLoginAsync(userId);
                case "Organiser":
                    return await _organiserRepository.UpdateLastLoginAsync(userId);
                case "Administrator":
                    return await _administratorRepository.UpdateLastLoginAsync(userId);
                default:
                    return false;
            }
        }

        private async Task<AuthResult> ValidateFormData(string username, string password)
        {
            var authResult = new AuthResult
            {
                Success = false,
                Message = "Username is already taken"
            };

            if (!await _userRepository.IsUsernameUniqueAsync(username))
            {
                return authResult;
            }

            if (!await _organiserRepository.IsUsernameUniqueAsync(username))
            {
                return authResult;
            }

            if (!await _administratorRepository.IsUsernameUniqueAsync(username))
            {
                return authResult;
            }

            if (password != null && password.Length < 6)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Password must be at least 6 characters long"
                };
            }

            return new AuthResult
            {
                Success = true,
                Message = "User is unique and valid"
            };
        }

        private AuthResult CreateSuccessResult(int userId, string username, string name, string surname, string role)
        {
            var token = GenerateJwtToken(userId, username, role);

            return new AuthResult
            {
                Success = true,
                Message = "Authentication successful",
                UserId = userId,
                Username = username,
                Name = name,
                Surname = surname,
                Role = role,
                Token = token
            };
        }

        private string GenerateJwtToken(int userId, string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "DefaultSecretKeyWith32Characters1234");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task<SignOutResult> SignOutAsync(int userId)
        {
            return Task.FromResult(new SignOutResult
            {
                Success = true,
                Message = "User signed out successfully"
            });
        }
    }
}