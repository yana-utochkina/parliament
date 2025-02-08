using System;
using System.Collections.Generic;
using ParliamentDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace ParliamentInfrastructure;

public partial class DbparliamentContext : DbContext
{
    public DbparliamentContext()
    {
    }

    public DbparliamentContext(DbContextOptions<DbparliamentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<UsersDepartment> UsersDepartments { get; set; }

    public virtual DbSet<UsersEventsRating> UsersEventsRatings { get; set; }

    public virtual DbSet<UsersEventsRole> UsersEventsRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Utochkina\\SQLEXPRESS; Database=DBParliament; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InstagramLink).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Contact).WithMany(p => p.Departments)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_Contacts");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.AccessType).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.Events)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Events_Departments");

            entity.HasOne(d => d.Location).WithMany(p => p.Events)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_Events_Locations");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasOne(d => d.Contact).WithMany(p => p.Locations)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Locations_Contacts");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PublicationDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.News)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_News_Departments");
        });

        modelBuilder.Entity<UsersDepartment>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Position).HasMaxLength(50);
        });

        modelBuilder.Entity<UsersEventsRating>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UsersEventsRating");
        });

        modelBuilder.Entity<UsersEventsRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UsersEventsRole");

            entity.HasIndex(e => e.UserId, "IX_UsersEvents");

            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
