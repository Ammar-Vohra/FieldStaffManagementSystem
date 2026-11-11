
//using FieldStaff.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;

//namespace FieldStaff.Infrastructure.Persistence
//{
//    public static class DatabaseSeeder
//    {
//        // ==========================================
//        // 1. MAIN SEEDING METHOD
//        // ==========================================

//        public static async Task SeedAsync(
//            FieldStaffDbContext context,
//            IConfiguration configuration)
//        {
//            // 1. Seed Roles
//            var roles = await SeedRolesAsync(context);

//            // 2. Seed Initial Admin
//            var adminRole = roles.First(r =>
//                r.RoleName.Equals(
//                    "Admin",
//                    StringComparison.OrdinalIgnoreCase));

//            var admin = await SeedAdminAsync(
//                context,
//                configuration,
//                adminRole.RoleId);

//            // 3. Seed Menus
//            var menus = await SeedMenusAsync(context, admin.UserId);

//            // 4. Seed Permissions
//            var permissions = await SeedPermissionsAsync(
//                context,
//                menus,
//                admin.UserId);

//            // 5. Seed Task Statuses
//            await SeedTaskStatusesAsync(context, admin.UserId);

//            // 6. Seed Task Priorities
//            await SeedTaskPrioritiesAsync(context, admin.UserId);

//            // 7. Assign Permissions to Roles
//            await SeedRolePermissionsAsync(
//                context,
//                roles,
//                permissions,
//                admin.UserId);
//        }


//        // ==========================================
//        // 2. SEED ROLES
//        // ==========================================

//        private static async Task<List<Role>> SeedRolesAsync(
//            FieldStaffDbContext context)
//        {
//            string[] roleNames =
//            {
//                "Admin",
//                "Manager",
//                "Field Staff"
//            };

//            var roles = await context.Roles
//                .IgnoreQueryFilters()
//                .ToListAsync();

//            foreach (var roleName in roleNames)
//            {
//                var role = roles.FirstOrDefault(r =>
//                    r.RoleName.Equals(
//                        roleName,
//                        StringComparison.OrdinalIgnoreCase));

//                if (role == null)
//                {
//                    context.Roles.Add(new Role
//                    {
//                        RoleName = roleName,
//                        IsActive = true,
//                        IsDeleted = false,
//                        CreatedAt = DateTime.UtcNow
//                    });
//                }
//                else
//                {
//                    role.IsActive = true;
//                    role.IsDeleted = false;
//                }
//            }

//            await context.SaveChangesAsync();

//            return await context.Roles
//                .IgnoreQueryFilters()
//                .ToListAsync();
//        }


//        // ==========================================
//        // 3. SEED INITIAL ADMIN USER
//        // ==========================================

//        private static async Task<User> SeedAdminAsync(
//            FieldStaffDbContext context,
//            IConfiguration configuration,
//            int adminRoleId)
//        {
//            var username = configuration["InitialAdmin:UserName"];
//            var fullName = configuration["InitialAdmin:FullName"];
//            var email = configuration["InitialAdmin:Email"];
//            var password = configuration["InitialAdmin:Password"];

//            if (string.IsNullOrWhiteSpace(username) ||
//                string.IsNullOrWhiteSpace(email) ||
//                string.IsNullOrWhiteSpace(password))
//            {
//                throw new InvalidOperationException(
//                    "Initial Admin credentials are missing from appsettings.json.");
//            }

//            // Find existing Admin using username or email
//            var admin = await context.Users
//                .IgnoreQueryFilters()
//                .FirstOrDefaultAsync(u =>
//                    u.UserName == username ||
//                    u.Email == email);

//            if (admin != null)
//            {
//                // Restore existing Admin without resetting password
//                admin.RoleId = adminRoleId;
//                admin.IsActive = true;
//                admin.IsDeleted = false;

//                await context.SaveChangesAsync();

//                return admin;
//            }

//            // Create initial Admin
//            var newAdmin = new User
//            {
//                UserName = username,
//                FullName = string.IsNullOrWhiteSpace(fullName)
//                    ? username
//                    : fullName,
//                Email = email,

//                PasswordHash =
//                    BCrypt.Net.BCrypt.HashPassword(password),

//                RoleId = adminRoleId,

//                IsActive = true,
//                IsDeleted = false,

//                CreatedAt = DateTime.UtcNow,

//                // No authenticated user exists during initial setup.
//                CreatedBy = null
//            };

//            context.Users.Add(newAdmin);

//            await context.SaveChangesAsync();

//            return newAdmin;
//        }


//        // ==========================================
//        // 4. SEED MENUS
//        // ==========================================

//        private static async Task<List<Menu>> SeedMenusAsync(
//            FieldStaffDbContext context,
//            int adminId)
//        {
//            var menuData = new[]
//            {
//                new { Name = "Dashboard", Path = "/dashboard" },
//                new { Name = "Customers", Path = "/customers" },
//                new { Name = "Service Tasks", Path = "/tasks" },
//                new { Name = "Users", Path = "/users" },
//                new { Name = "Roles & Permissions", Path = "/roles" }
//            };

//            var menus = await context.Menus
//                .IgnoreQueryFilters()
//                .ToListAsync();

//            foreach (var item in menuData)
//            {
//                var menu = menus.FirstOrDefault(m =>
//                    m.MenuName.Equals(
//                        item.Name,
//                        StringComparison.OrdinalIgnoreCase));

//                if (menu == null)
//                {
//                    context.Menus.Add(new Menu
//                    {
//                        MenuName = item.Name,
//                        MenuPath = item.Path,

//                        IsActive = true,
//                        IsDeleted = false,

//                        CreatedAt = DateTime.UtcNow,
//                        CreatedBy = adminId
//                    });
//                }
//                else
//                {
//                    menu.MenuPath = item.Path;
//                    menu.IsActive = true;
//                    menu.IsDeleted = false;
//                }
//            }

//            await context.SaveChangesAsync();

//            return await context.Menus
//                .IgnoreQueryFilters()
//                .ToListAsync();
//        }


//        // ==========================================
//        // 5. SEED PERMISSIONS
//        // ==========================================

//        private static async Task<List<Permission>> SeedPermissionsAsync(
//            FieldStaffDbContext context,
//            List<Menu> menus,
//            int adminId)
//        {
//            var permissionData = new[]
//            {
//                // Dashboard
//                new { Name = "View Dashboard", Code = "Dashboard.View", Menu = "Dashboard" },

//                // Customers
//                new { Name = "View Customers", Code = "Customers.View", Menu = "Customers" },
//                new { Name = "Create Customers", Code = "Customers.Create", Menu = "Customers" },
//                new { Name = "Update Customers", Code = "Customers.Update", Menu = "Customers" },
//                new { Name = "Delete Customers", Code = "Customers.Delete", Menu = "Customers" },

//                // Service Tasks
//                new { Name = "View Service Tasks", Code = "Tasks.View", Menu = "Service Tasks" },
//                new { Name = "Create Service Tasks", Code = "Tasks.Create", Menu = "Service Tasks" },
//                new { Name = "Update Service Tasks", Code = "Tasks.Update", Menu = "Service Tasks" },
//                new { Name = "Assign Service Tasks", Code = "Tasks.Assign", Menu = "Service Tasks" },
//                new { Name = "Update Task Status", Code = "Tasks.UpdateStatus", Menu = "Service Tasks" },

//                // Users
//                new { Name = "View Users", Code = "Users.View", Menu = "Users" },
//                new { Name = "Create Users", Code = "Users.Create", Menu = "Users" },
//                new { Name = "Update Users", Code = "Users.Update", Menu = "Users" },

//                // Roles & Permissions
//                new { Name = "View Roles", Code = "Roles.View", Menu = "Roles & Permissions" },
//                new { Name = "Manage Roles", Code = "Roles.Manage", Menu = "Roles & Permissions" },
//                new { Name = "Manage Permissions", Code = "Permissions.Manage", Menu = "Roles & Permissions" }
//            };

//            var permissions = await context.Permissions
//                .IgnoreQueryFilters()
//                .ToListAsync();

//            foreach (var item in permissionData)
//            {
//                var menu = menus.First(m =>
//                    m.MenuName.Equals(
//                        item.Menu,
//                        StringComparison.OrdinalIgnoreCase));

//                var permission = permissions.FirstOrDefault(p =>
//                    p.PermissionCode == item.Code);

//                if (permission == null)
//                {
//                    context.Permissions.Add(new Permission
//                    {
//                        PermissionName = item.Name,
//                        PermissionCode = item.Code,
//                        MenuId = menu.MenuId,

//                        IsActive = true,
//                        IsDeleted = false,

//                        CreatedAt = DateTime.UtcNow,
//                        CreatedBy = adminId
//                    });
//                }
//                else
//                {
//                    permission.PermissionName = item.Name;
//                    permission.MenuId = menu.MenuId;
//                    permission.IsActive = true;
//                    permission.IsDeleted = false;
//                }
//            }

//            await context.SaveChangesAsync();

//            return await context.Permissions
//                .IgnoreQueryFilters()
//                .ToListAsync();
//        }


//        // ==========================================
//        // 6. SEED TASK STATUSES
//        // ==========================================

//        private static async Task SeedTaskStatusesAsync(
//            FieldStaffDbContext context,
//            int adminId)
//        {
//            var statusData = new[]
//            {
//                new { Name = "Pending", Code = "PENDING" },
//                new { Name = "Assigned", Code = "ASSIGNED" },
//                new { Name = "In Progress", Code = "IN_PROGRESS" },
//                new { Name = "Completed", Code = "COMPLETED" },
//                new { Name = "Cancelled", Code = "CANCELLED" }
//            };

//            var statuses = await context.TaskStatuses
//                .IgnoreQueryFilters()
//                .ToListAsync();

//            foreach (var item in statusData)
//            {
//                var status = statuses.FirstOrDefault(s =>
//                    s.TaskStatusCode == item.Code);

//                if (status == null)
//                {
//                    context.TaskStatuses.Add(new StatusTask
//                    {
//                        TaskStatusName = item.Name,
//                        TaskStatusCode = item.Code,

//                        CreatedAt = DateTime.UtcNow,
//                        CreatedBy = adminId
//                    });
//                }
//                else
//                {
//                    status.TaskStatusName = item.Name;
//                }
//            }

//            await context.SaveChangesAsync();
//        }


//        // ==========================================
//        // 7. SEED TASK PRIORITIES
//        // ==========================================

//        private static async Task SeedTaskPrioritiesAsync(
//            FieldStaffDbContext context,
//            int adminId)
//        {
//            var priorityData = new[]
//            {
//                new { Name = "Low", Code = "LOW" },
//                new { Name = "Medium", Code = "MEDIUM" },
//                new { Name = "High", Code = "HIGH" }
//            };

//            var priorities = await context.TaskPriorities
//                .IgnoreQueryFilters()
//                .ToListAsync();

//            foreach (var item in priorityData)
//            {
//                var priority = priorities.FirstOrDefault(p =>
//                    p.PriorityCode == item.Code);

//                if (priority == null)
//                {
//                    context.TaskPriorities.Add(new TaskPriority
//                    {
//                        PriorityName = item.Name,
//                        PriorityCode = item.Code,

//                        CreatedAt = DateTime.UtcNow,
//                        CreatedBy = adminId
//                    });
//                }
//                else
//                {
//                    priority.PriorityName = item.Name;
//                }
//            }

//            await context.SaveChangesAsync();
//        }


//        // ==========================================
//        // 8. ASSIGN PERMISSIONS TO ROLES
//        // ==========================================

//        private static async Task SeedRolePermissionsAsync(
//            FieldStaffDbContext context,
//            List<Role> roles,
//            List<Permission> permissions,
//            int adminId)
//        {
//            var admin = roles.First(r =>
//                r.RoleName.Equals(
//                    "Admin",
//                    StringComparison.OrdinalIgnoreCase));

//            var manager = roles.First(r =>
//                r.RoleName.Equals(
//                    "Manager",
//                    StringComparison.OrdinalIgnoreCase));

//            var fieldStaff = roles.First(r =>
//                r.RoleName.Equals(
//                    "Field Staff",
//                    StringComparison.OrdinalIgnoreCase));

//            // Admin gets all permissions
//            var adminPermissions = permissions
//                .Select(p => p.PermissionCode)
//                .ToList();

//            // Manager permissions
//            var managerPermissions = new List<string>
//            {
//                "Dashboard.View",

//                "Customers.View",
//                "Customers.Create",
//                "Customers.Update",

//                "Tasks.View",
//                "Tasks.Create",
//                "Tasks.Update",
//                "Tasks.Assign"
//            };

//            // Field Staff permissions
//            var fieldStaffPermissions = new List<string>
//            {
//                "Dashboard.View",
//                "Tasks.View",
//                "Tasks.UpdateStatus"
//            };

//            var assignments = new Dictionary<int, List<string>>
//            {
//                [admin.RoleId] = adminPermissions,
//                [manager.RoleId] = managerPermissions,
//                [fieldStaff.RoleId] = fieldStaffPermissions
//            };

//            var existingAssignments = await context.RolePermissions
//                .IgnoreQueryFilters()
//                .AsNoTracking()
//                .ToListAsync();

//            var existingSet = existingAssignments
//                .Select(rp => $"{rp.RoleId}:{rp.PermissionId}")
//                .ToHashSet();

//            foreach (var assignment in assignments)
//            {
//                int roleId = assignment.Key;

//                foreach (var permissionCode in assignment.Value)
//                {
//                    var permission = permissions.First(p =>
//                        p.PermissionCode == permissionCode);

//                    string key = $"{roleId}:{permission.PermissionId}";

//                    if (!existingSet.Contains(key))
//                    {
//                        context.RolePermissions.Add(new RolePermission
//                        {
//                            RoleId = roleId,
//                            PermissionId = permission.PermissionId,

//                            CreatedAt = DateTime.UtcNow,
//                            CreatedBy = adminId
//                        });

//                        existingSet.Add(key);
//                    }
//                }
//            }

//            await context.SaveChangesAsync();
//        }
//    }
//}





using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace FieldStaff.Infrastructure.Persistence
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            FieldStaffDbContext context,
            IConfiguration configuration)
        {
            // 1. Seed Admin Role
            var adminRole = await context.Roles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.RoleName == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    RoleName = "Admin",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }
            else
            {
                adminRole.IsActive = true;
                adminRole.IsDeleted = false;

                await context.SaveChangesAsync();
            }

            // 2. Read Admin Credentials
            var username = configuration["InitialAdmin:UserName"];
            var email = configuration["InitialAdmin:Email"];
            var password = configuration["InitialAdmin:Password"];

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Initial Admin credentials are missing from configuration.");
            }

            // 3. Check Existing Admin User
            var adminUser = await context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.UserName == username || u.Email == email);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = username,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    RoleId = adminRole.RoleId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = null
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
            else
            {
                adminUser.RoleId = adminRole.RoleId;
                adminUser.IsActive = true;
                adminUser.IsDeleted = false;

                await context.SaveChangesAsync();
            }
        }
    }
}

