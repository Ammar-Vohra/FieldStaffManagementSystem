using FieldStaff.Application.Features.Menus.DTOs;

namespace FieldStaff.Application.Features.Menus.Interfaces
{
    public interface IMenuService
    {
        Task<MenuResponseDto> CreateAsync(CreateMenuRequestDto request, int? createdByUserId);

        Task<MenuResponseDto?> GetByIdAsync(int menuId);
        Task<List<MenuResponseDto>> GetAllAsync();
        Task<MenuResponseDto> UpdateAsync(int menuId, UpdateMenuRequestDto request, int? updatedByUserId);
        Task<bool> DeleteAsync(int menuId, int? deletedByUserId);
    }
}
