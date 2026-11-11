
using FieldStaff.Application.Features.Auth;
using FieldStaff.Infrastructure.Persistence;
using FieldStaff.Infrastructure.Repositories;
using FieldStaff.Infrastructure.Services;
using FieldStaff.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FieldStaff.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<FieldStaffDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                ));

            // Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
