using ParliamentDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace ParliamentInfrastructure;

public partial class ParliamentDbContext : DbContext
{

    public ParliamentDbContext(DbContextOptions<ParliamentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<UserDepartmentDetail> UserDepartmentDetail { get; set; }

    public virtual DbSet<UserEventDetail> UserEventDetails { get; set; }

    public virtual DbSet<User> Users {  get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InstagramLink).HasMaxLength(255);
            entity.Property(e => e.TelegramLink).HasMaxLength(255);
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
            entity.Property(e => e.AccessType).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.Events)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Events_Departments");

            entity.HasOne(d => d.Location).WithMany(p => p.Events)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.SetNull)
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
            entity.Property(e => e.PublicationDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.ShortDescription).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.News)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_News_Departments");
        });

        modelBuilder.Entity<UserDepartmentDetail>(entity =>
        {
            entity.HasKey(e => new { e.WorkerId, e.DepartmentId});
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("CONVERT(DATE, GETDATE())");

            entity.HasOne(u => u.User).WithMany(ud => ud.UserDepartmentDetails)
                .HasForeignKey(u => u.WorkerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserDepartmentDetails_Users");

            entity.HasOne(d => d.Department).WithMany(ud => ud.UserDepartmentDetails)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserDepartmentDetails_Departments");
        });

        modelBuilder.Entity<UserEventDetail>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.EventId });

            entity.HasOne(u => u.User).WithMany(ud => ud.UserEventDetails)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserEventDetails_Users");

            entity.HasOne(e => e.Event).WithMany(ud => ud.UserEventDetails)
                .HasForeignKey(u => u.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserEventDetails_Events");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
