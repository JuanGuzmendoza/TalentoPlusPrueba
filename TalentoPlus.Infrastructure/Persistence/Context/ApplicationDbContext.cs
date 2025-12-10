using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.DocumentNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<Employee>(e => e.UserId)
                    .IsRequired(false); 
            });

            builder.Entity<Department>(entity =>
            {
                entity.HasIndex(d => d.Name).IsUnique(); 
            });
        }
    }
}