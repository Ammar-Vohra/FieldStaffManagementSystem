using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Menus.DTOs;
using FieldStaff.Application.Features.Users.DTOs;

namespace FieldStaff.Application.Features.Menus.Interfaces
{
    public interface IMenuService
    {
        Task<MenuResponseDto> CreateAsync(CreateMenuRequestDto request, int? createdByUserId);

        Task<MenuResponseDto?> GetByIdAsync(int menuId);
        Task<List<MenuResponseDto>> GetAllAsync();
        Task<PaginatedResponse<MenuResponseDto>> GetPagedMenusAsync(MenuQueryParameters parameters);

        Task<MenuResponseDto> UpdateAsync(int menuId, UpdateMenuRequestDto request, int? updatedByUserId);
        Task<bool> DeleteAsync(int menuId, int? deletedByUserId);
    }
}
