using FieldStaff.Application.Features;
using FieldStaff.Application.Features.Auth;
using FieldStaff.Application.Features.Auth.Interfaces;
using FieldStaff.Application.Features.Authorization;
using FieldStaff.Application.Features.Authorization.Interfaces;
using FieldStaff.Application.Features.Authorization.Services;
using FieldStaff.Application.Features.Menus;
using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Application.Features.Permissions;
using FieldStaff.Application.Features.Permissions.Interfaces;
using FieldStaff.Application.Features.RoleMenus;
using FieldStaff.Application.Features.RoleMenus.Interfaces;
using FieldStaff.Application.Features.RolePermissions;
using FieldStaff.Application.Features.RolePermissions.Interfaces;
using FieldStaff.Application.Features.UserPermissions;
using FieldStaff.Application.Features.UserPermissions.Interfaces;
using FieldStaff.Application.Features.Roles;
using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Application.Features.TaskPriorities;
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using FieldStaff.Application.Features.TaskStatuses;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using FieldStaff.Application.Features.Users;
using FieldStaff.Application.Features.Users.Interfaces;
using FieldStaff.Application.Services;
using FieldStaff.Infrastructure.Persistence;
using FieldStaff.Infrastructure.Persistence.Repositories;
using FieldStaff.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FieldStaff.Application.Features.UserPermissions.Services;

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
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IRoleMenuRepository, RoleMenuRepository>();
            services.AddScoped<IPermissionAuthorizationRepository, PermissionAuthorizationRepository>();



            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskPriorityService, TaskPriorityService>();
            services.AddScoped<ITaskStatusService, TaskStatusService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IRoleMenuService, RoleMenuService>();
            services.AddScoped<IPermissionAuthorizationService, PermissionAuthorizationService>();

            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}