
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Persistence
{
    public class FieldStaffDbContext : DbContext
    {
        public FieldStaffDbContext(
            DbContextOptions<FieldStaffDbContext> options)
            : base(options)
        {
        }

        // =========================================
        // DbSets
        // =========================================

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ServiceTask> ServiceTasks { get; set; }
        public DbSet<StatusTask> TaskStatuses { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<TaskStatusHistory> TaskStatusHistories { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Dynamic Authorization

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }


        // =========================================
        // MODEL CONFIGURATION
        // =========================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Load all IEntityTypeConfiguration classes
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(FieldStaffDbContext).Assembly
            );


            // =========================================
            // BASE ENTITY AUDIT RELATIONSHIPS
            // =========================================

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var entity = modelBuilder.Entity(entityType.ClrType);

                    entity.HasOne(typeof(User), null)
                        .WithMany()
                        .HasForeignKey(nameof(BaseEntity.CreatedBy))
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(typeof(User), null)
                        .WithMany()
                        .HasForeignKey(nameof(BaseEntity.UpdatedBy))
                        .OnDelete(DeleteBehavior.Restrict);
                }
            }


            // =========================================
            // GLOBAL QUERY FILTERS
            // =========================================

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Role>()
                .HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<Customer>()
                .HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<ServiceTask>()
                .HasQueryFilter(t => !t.IsDeleted);

            modelBuilder.Entity<Menu>()
                .HasQueryFilter(m => !m.IsDeleted);

            modelBuilder.Entity<Permission>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<UserPermission>()
                .HasQueryFilter(up => !up.IsDeleted);
        }
    }
}
