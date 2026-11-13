using FieldStaff.Application.Features.Auth;
using FieldStaff.Application.Features.Auth.Interfaces;
using FieldStaff.Application.Features.Roles;
using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Application.Features.Users;
using FieldStaff.Application.Features.Users.Interfaces;
using FieldStaff.Application.Features.TaskPriorities;
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using FieldStaff.Application.Features.TaskStatuses;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using FieldStaff.Application.Features.Menus;
using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Application.Features.Permissions;
using FieldStaff.Application.Features.Permissions.Interfaces;
using FieldStaff.Application.Features.MenuPermissions;
using FieldStaff.Application.Features.MenuPermissions.Interfaces;
using FieldStaff.Application.Features.RolePermissions;
using FieldStaff.Application.Features.RolePermissions.Interfaces;
using FieldStaff.Application.Services;
using FieldStaff.Application.Features;
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
            services.AddScoped<ITaskStatusRepository, TaskStatusRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IMenuPermissionRepository, MenuPermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();


            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskPriorityService, TaskPriorityService>();
            services.AddScoped<ITaskStatusService, TaskStatusService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IMenuPermissionService, MenuPermissionService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();

            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}