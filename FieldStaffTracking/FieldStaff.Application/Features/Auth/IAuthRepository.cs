using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Auth
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
