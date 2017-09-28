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
        public static List<User> _Instructors = new List<User>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;Port=3306;Uid=root;Pwd=;Database=bookings");
            }
            _Instructors.Add(new User { UserID = 1, FirstName = "Pranav", LastName = "Joshi", Email = "joshi.pranav@hotmail.com", Designation = "Yoga Trainer" });
            _Instructors.Add(new User { UserID = 2, FirstName = "John", LastName = "Doe", Email = "john.doe@hotmail.com", Designation = "Athletic Trainer" });
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
