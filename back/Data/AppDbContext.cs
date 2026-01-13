using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.API.Models;

namespace CoursePlatform.API.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft Delete Filter
            modelBuilder.Entity<Course>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Lesson>().HasQueryFilter(e => !e.IsDeleted);

            // Configurations
            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Status).HasConversion<string>();
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Lessons)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Rename Identity Tables
            modelBuilder.Entity<User>(e => e.ToTable("Users"));
            modelBuilder.Entity<IdentityRole<int>>(e => e.ToTable("Roles"));
            modelBuilder.Entity<IdentityUserRole<int>>(e => e.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserClaim<int>>(e => e.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<int>>(e => e.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityUserToken<int>>(e => e.ToTable("UserTokens"));
            modelBuilder.Entity<IdentityRoleClaim<int>>(e => e.ToTable("RoleClaims"));
        }
    }
}
