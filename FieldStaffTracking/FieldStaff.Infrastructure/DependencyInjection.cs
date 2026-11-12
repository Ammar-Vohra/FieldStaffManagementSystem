using FieldStaff.Application.Features.Auth;
using FieldStaff.Application.Features.Auth.Interfaces;
using FieldStaff.Application.Features.Roles;
using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Application.Features.Users;
using FieldStaff.Application.Features.Users.Interfaces;
using FieldStaff.Application.Features.TaskPriorities;
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using FieldStaff.Application.Services;
using FieldStaff.Infrastructure.Persistence;
using FieldStaff.Infrastructure.Repositories;
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
           
         

            
            services.AddDbContext<FieldStaffDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                );

            });

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskPriorityRepository, TaskPriorityRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskPriorityService, TaskPriorityService>();

            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}