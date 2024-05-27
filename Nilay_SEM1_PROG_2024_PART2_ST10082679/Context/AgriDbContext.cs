using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nilay_SEM1_PROG_2024_PART2_ST10082679.Models;

namespace Nilay_SEM1_PROG_2024_PART2_ST10082679.Context;

public partial class AgriDbContext : DbContext
    
{

    public AgriDbContext()
    {
    }

    public AgriDbContext(DbContextOptions<AgriDbContext> options)
        : base(options)
    {
       
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Build the relative path for the database file
            var relativePath = Path.Combine(Directory.GetCurrentDirectory(), "NILAY_SEM1_PART2_PROG.mdf");
            // Get the connection string from the configuration
            var sqlServerName = "(LocalDB)\\MSSQLLocalDB"; // ***** CHANGE TO SUIT YOUR SQL SERVER *****
            var connectionStringTemplate = "Data Source=|SqlServer|;AttachDbFilename=|DataDirectory|;Integrated Security=True;Connect Timeout=30";
            var connectionString = connectionStringTemplate.Replace("|SqlServer|", sqlServerName)
                                                  .Replace("|DataDirectory|", relativePath);
            // Configure the DbContext to use SQL Server
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__9834FBBA2E14D006");

            entity.Property(e => e.ProductId).HasColumnName("Product_Id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductDate)
                .HasColumnType("datetime")
                .HasColumnName("Product_Date");
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.User).WithMany(p => p.Products)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__User_I__267ABA7A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__206D9170154350BD");

            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
