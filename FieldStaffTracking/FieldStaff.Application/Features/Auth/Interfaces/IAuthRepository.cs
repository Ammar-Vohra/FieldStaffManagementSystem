using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
