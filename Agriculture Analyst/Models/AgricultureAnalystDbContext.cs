using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Agriculture_Analyst.Models;

public partial class AgricultureAnalystDbContext : DbContext
{
    public AgricultureAnalystDbContext()
    {
    }

    public AgricultureAnalystDbContext(DbContextOptions<AgricultureAnalystDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aianalysis> Aianalyses { get; set; }

    public virtual DbSet<DiaryEntry> DiaryEntries { get; set; }

    public virtual DbSet<Plant> Plants { get; set; }

    public virtual DbSet<PlantImage> PlantImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }
     public virtual DbSet<Inventory> Inventories { get; set; }
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<PlantTask> PlantTasks { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aianalysis>(entity =>
        {
            entity.HasKey(e => e.AnalysisId).HasName("PK__AIAnalys__5B789DC886C98FCE");

            entity.ToTable("AIAnalysis");

            entity.Property(e => e.AnalyzedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiseaseDetected).HasMaxLength(200);
            entity.Property(e => e.HealthStatus).HasMaxLength(100);

            entity.HasOne(d => d.Image).WithMany(p => p.Aianalyses)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK_AI_Image");
        });

        modelBuilder.Entity<DiaryEntry>(entity =>
        {
            entity.HasKey(e => e.DiaryId).HasName("PK__DiaryEnt__267B56F4A5B60AE5");

            entity.Property(e => e.Activity).HasMaxLength(100);
            entity.Property(e => e.Weather).HasMaxLength(100);

            entity.HasOne(d => d.Plant).WithMany(p => p.DiaryEntries)
                .HasForeignKey(d => d.PlantId)
                .HasConstraintName("FK_Diary_Plants");
        });

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.PlantId).HasName("PK__Plants__98FE395C0F11A3CF");

            entity.Property(e => e.PlantName).HasMaxLength(100);
            entity.Property(e => e.PlantType).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Plants)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Plants_Users");
        });

        modelBuilder.Entity<PlantImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__PlantIma__7516F70C9E1DA089");

            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Plant).WithMany(p => p.PlantImages)
                .HasForeignKey(d => d.PlantId)
                .HasConstraintName("FK_Images_Plants");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId);

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<PlantTask>(entity =>
        {
            entity.HasKey(e => e.TaskId);

            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasOne(e => e.Plant)
                .WithMany(p => p.PlantTasks)
                .HasForeignKey(e => e.PlantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(u => u.PlantTasks)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventory"); // 👈 rõ Data First

            entity.HasKey(e => e.InvId);

            entity.Property(e => e.InvName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Inventories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Item"); // 👈 RẤT NÊN CÓ

            entity.HasKey(e => e.ItemId);

            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Unit)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.ToTable("InventoryTransaction");

            entity.HasKey(e => e.TransId);

            entity.Property(e => e.DonGia)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("[SoLuong] * [DonGia]", stored: true);

            entity.Property(e => e.NgayGiaoDich)
                .HasDefaultValueSql("(getdate())");

            // 🔥 RẤT QUAN TRỌNG
            entity.HasOne(e => e.Inventory)
                .WithMany(i => i.InventoryTransactions)
                .HasForeignKey(e => e.InvId);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.InventoryTransactions)
                .HasForeignKey(e => e.ItemId);

            entity.HasOne(e => e.User)
                .WithMany(u => u.InventoryTransactions)
                .HasForeignKey(e => e.UserId);
        });




        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
