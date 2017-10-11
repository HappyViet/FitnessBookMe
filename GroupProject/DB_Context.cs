using GroupProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject
{
    public partial class DB_Context : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<ClassPackage> ClassPackages { get; set; }
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
                .Property(e => e._Gender).HasColumnName("Gender");
                modelBuilder.Entity<User>().ToTable("users")
                .Property(e => e._UserRole).HasColumnName("UserRole");
                modelBuilder.Entity<User>().ToTable("users")
                .Property(e => e.DateOfBirth).HasColumnType("Date");

                modelBuilder.Entity<User>().ToTable("users")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("NormalUser")
                .HasValue<Instructor>("SystemUser");
                
            });

            modelBuilder.Entity<Location>(entity =>
            {
                modelBuilder.Entity<Location>().ToTable("locations")
                .Property(l => l.LocationID)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ClassPackage>(entity=> {
                modelBuilder.Entity<ClassPackage>().ToTable("class_packages")
                .Property(c => c.ClassPackageID)
                .ValueGeneratedOnAdd();
                modelBuilder.Entity<ClassPackage>().ToTable("class_packages")
               .Property(e => e.ExpirationDate).HasColumnType("Date");
            });
        }
    }
}
