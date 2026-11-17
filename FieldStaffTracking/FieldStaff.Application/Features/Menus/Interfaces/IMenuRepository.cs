using FieldStaff.Application.Features.Menus.DTOs;
using FieldStaff.Application.Features.Users.DTOs;
using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Menus.Interfaces
{
    public interface IMenuRepository
    {
        Task<Menu?> GetByIdAsync(int menuId);
        Task<List<Menu>> GetAllAsync();
        Task<(List<Menu> Menus, int TotalRecords)> GetPagedAsync(MenuQueryParameters parameters);

        Task<List<Menu>> GetChildMenusAsync(int parentMenuId);
        Task<bool> HasChildMenusAsync(int menuId);
        Task<bool> ExistsAsync(int menuId);
        Task AddAsync(Menu menu);
        Task UpdateAsync(Menu menu);
        Task SaveChangesAsync();
    }
}
