
using BCrypt.Net;
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Application.Features.Users.DTOs;
using FieldStaff.Application.Features.Users.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

    
        public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request, int? createdByUserId)
        {
            
            string email = request.Email.Trim();

            bool emailExists =
                await _userRepository.EmailExistsAsync(email);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists.");
            }

            var role = await _roleRepository.GetRoleByIdAsync(
                request.RoleId);

            if (role == null || !role.IsActive || role.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Selected role does not exist or is inactive.");
            }

            var user = new User
            {
                RoleId = request.RoleId,
                UserName = request.UserName.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = request.IsActive,
                IsDeleted = false,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();

          
            return MapToResponseDto(user);
        }

       
        public async Task<UserResponseDto?> GetUserByIdAsync(
            int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            return MapToResponseDto(user);
        }

        
        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(user =>
                MapToResponseDto(user
                )).ToList();
        }


      
        public async Task<PaginatedResponse<UserResponseDto>> GetPagedUsersAsync(
            UserQueryParameters parameters)
        {
            
            if (parameters.PageNumber < 1)
            {
                throw new InvalidOperationException(
                    "Page number must be greater than zero.");
            }

            
            if (parameters.PageSize < 1 || parameters.PageSize > 100)
            {
                throw new InvalidOperationException(
                    "Page size must be between 1 and 100.");
            }

            
            var allowedSortColumns = new[]
            {
                "username",
                "email",
                "rolename",
                "isactive",
                "createdat"
            };

            if (!allowedSortColumns.Contains(
                parameters.SortBy,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Invalid sorting column.");
            }

            
            if (!parameters.SortDirection.Equals(
                "asc",
                StringComparison.OrdinalIgnoreCase) &&
                !parameters.SortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Sort direction must be either 'asc' or 'desc'.");
            }

            
            if (parameters.RoleId.HasValue &&
                parameters.RoleId.Value < 1)
            {
                throw new InvalidOperationException(
                    "Invalid RoleId.");
            }

            
            parameters.SortBy = parameters.SortBy.ToLowerInvariant();
            parameters.SortDirection = parameters.SortDirection.ToLowerInvariant();

            
            var (users, totalRecords) =
                await _userRepository.GetPagedAsync(parameters);

            
            int totalPages = (int)Math.Ceiling(
                (double)totalRecords / parameters.PageSize);

            
            var userDtos = users.Select(user =>
                MapToResponseDto(
                    user
                )).ToList();

            
            return new PaginatedResponse<UserResponseDto>
            {
                Data = userDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasPreviousPage = parameters.PageNumber > 1,
                HasNextPage = parameters.PageNumber < totalPages
            };
        }



        public async Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserRequestDto request, int? updatedByUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            
            string email = request.Email.Trim();

            bool emailExists =
                await _userRepository.EmailExistsAsync(
                    email,
                    userId);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "Another user with this email already exists.");
            }

            
            var role = await _roleRepository.GetRoleByIdAsync(
                request.RoleId);

            if (role == null || !role.IsActive || role.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Selected role does not exist or is inactive.");
            }

            
            user.RoleId = request.RoleId;
            user.UserName = request.UserName.Trim();
            user.Email = email;
            user.IsActive = request.IsActive;

            
            user.UpdatedBy = updatedByUserId;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToResponseDto(user);
        }

        
        public async Task<bool> DeleteUserAsync(
            int userId,
            int? deletedByUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            user.IsDeleted = true;
            user.UpdatedBy = deletedByUserId;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        
        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                RoleId = user.RoleId,
                //RoleName = roleName,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedBy = user.CreatedBy,
                CreatedAt = user.CreatedAt,
                UpdatedBy = user.UpdatedBy,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
