using FieldStaff.Application.Features.UserPermissions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.UserPermissions.Interfaces
{
    public interface IUserPermissionService
    {
        Task<UserPermissionResponseDto> CreateAsync(
            CreateUserPermissionRequestDto request,
            int? createdByUserId);

        Task<UserPermissionResponseDto?> GetByIdAsync(int userPermissionId);

        Task<List<UserPermissionResponseDto>> GetAllAsync();

        Task<UserPermissionResponseDto> UpdateAsync(
            int userPermissionId,
            UpdateUserPermissionRequestDto request,
            int? updatedByUserId);

        Task<bool> DeleteAsync(
            int userPermissionId,
            int? deletedByUserId);
    }
}
