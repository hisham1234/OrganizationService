using Microsoft.EntityFrameworkCore;

namespace Organization_Service.Models 
{
    public class OrganizationContext : DbContext
    {
        public OrganizationContext(DbContextOptions<OrganizationContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> User { get; set; }
        public DbSet<OfficeEntity> Office { get; set; }
        public DbSet<RoleEntity> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        #region Required
        {
            modelBuilder.Entity<UserEntity>().Property(b => b.CreatedAt).HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<UserEntity>().Property(b => b.UpdatedAt).HasDefaultValueSql("current_timestamp on update current_timestamp");
            modelBuilder.Entity<UserEntity>().HasOne(e => e.Office).WithMany(e => e.Users).OnDelete(DeleteBehavior.SetNull);
            //modelBuilder.Entity<User>().hasm(e => e.Role).WithMany(e => e.User).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<OfficeEntity>().Property(b => b.CreatedAt).HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<OfficeEntity>().Property(b => b.UpdatedAt).HasDefaultValueSql("current_timestamp on update current_timestamp");
            modelBuilder.Entity<RoleEntity>().Property(b => b.CreatedAt).HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<RoleEntity>().Property(b => b.UpdatedAt).HasDefaultValueSql("current_timestamp on update current_timestamp");
            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
