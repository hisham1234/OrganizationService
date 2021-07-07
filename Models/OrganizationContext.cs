using Microsoft.EntityFrameworkCore;

namespace Organization_Service.Models 
{
    public class OrganizationContext : DbContext
    {
        public OrganizationContext(DbContextOptions<OrganizationContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Office> Office { get; set; }
        public DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        #region Required
        {
            modelBuilder.Entity<User>().Property(b => b.CreatedAt).HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<User>().Property(b => b.UpdatedAt).HasDefaultValueSql("current_timestamp on update current_timestamp");
            modelBuilder.Entity<User>().HasOne(e => e.Office).WithMany(e => e.Users).OnDelete(DeleteBehavior.SetNull);
            //modelBuilder.Entity<User>().hasm(e => e.Role).WithMany(e => e.User).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Office>().Property(b => b.CreatedAt).HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<Office>().Property(b => b.UpdatedAt).HasDefaultValueSql("current_timestamp on update current_timestamp");
            modelBuilder.Entity<Role>().Property(b => b.CreatedAt).HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<Role>().Property(b => b.UpdatedAt).HasDefaultValueSql("current_timestamp on update current_timestamp");
            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
