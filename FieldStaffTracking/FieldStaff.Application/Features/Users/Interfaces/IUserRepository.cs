
using FieldStaff.Application.Features.Users.DTOs;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Users.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);

        Task<User?> GetByEmailAsync(string email);

        Task<List<User>> GetAllAsync();
        Task<(List<User> Users, int TotalRecords)> GetPagedAsync(UserQueryParameters parameters);

        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

        Task<User> CreateAsync(User user);

        Task UpdateAsync(User user);

        Task SaveChangesAsync();
    }
}
