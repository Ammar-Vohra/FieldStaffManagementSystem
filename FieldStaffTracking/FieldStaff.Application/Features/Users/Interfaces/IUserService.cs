
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Users.DTOs;

namespace FieldStaff.Application.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request, int? createdByUserId);

        Task<UserResponseDto?> GetUserByIdAsync(int userId);

        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<PaginatedResponse<UserResponseDto>> GetPagedUsersAsync(UserQueryParameters parameters);

        Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserRequestDto request, int? updatedByUserId);

        Task<bool> DeleteUserAsync(int userId, int? deletedByUserId);
    }
}
