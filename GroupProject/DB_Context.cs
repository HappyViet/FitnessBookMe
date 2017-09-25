using GroupProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject
{
    public partial class DB_Context : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;Port=3306;Uid=root;Pwd=;Database=bookings");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                modelBuilder.Entity<User>().ToTable("users")
                .Property(u => u.UserID)
                .ValueGeneratedOnAdd();
                modelBuilder.Entity<User>().ToTable("users")
                .Property(e => e.FirstName);
                modelBuilder.Entity<User>().ToTable("users")
                .Property(e => e.LastName);
                modelBuilder.Entity<User>().ToTable("users")
                .Property(e => e.Email);
                modelBuilder.Entity<User>().ToTable("users")
                .Property(e => e.Password);
                modelBuilder.Entity<User>().ToTable("users")
                .Property(e => e.UserRole);
            });
        }
    }
}
