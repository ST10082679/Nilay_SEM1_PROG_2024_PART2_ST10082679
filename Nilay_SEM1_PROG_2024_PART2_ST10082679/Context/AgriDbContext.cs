using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NILAY_SEM1_PART2_PROG;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

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
