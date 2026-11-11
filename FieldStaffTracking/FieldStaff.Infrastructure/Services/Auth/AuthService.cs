using FieldStaff.Application.DTOs.LoginDto;
using FieldStaff.Application.Features.Auth;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FieldStaff.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IJwtService _jwtservice;

        public AuthService(IAuthRepository repository, IJwtService jwtservice)
        {
            _repository =  repository;
            _jwtservice = jwtservice;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto requestDto)
        {
            var user = await _repository.GetUserByEmailAsync(requestDto.Email);

            if (user == null || user.IsDeleted || !user.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "Invlaid Username or password");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(requestDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invlaid Username or password");
            }

            if (user.Role == null || !user.Role.IsActive || user.Role.IsDeleted)
            {
                throw new UnauthorizedAccessException(
                    "Your assigned role is inactive.");
            }

            string token = _jwtservice.GenerateToken(
                user.UserId,
                user.Role.RoleId,
                user.UserName);

            return new LoginResponseDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.Role.RoleId,
                Token = token
            };
        }
    }
}
