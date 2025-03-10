using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CoworkingDomain.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CoworkingInfrastructure;

public partial class CoworkingDbContext : IdentityDbContext<User>
{
    public CoworkingDbContext()
    {
    }

    public CoworkingDbContext(DbContextOptions<CoworkingDbContext> options)
           : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingsFacility> BookingsFacilities { get; set; }

    public virtual DbSet<CoworkingFacilityPrice> CoworkingFacilityPrices { get; set; }

    public virtual DbSet<CoworkingSpace> CoworkingSpaces { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Victus04\\SQLEXPRESS; Database=CoworkingDB; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseCollation("Cyrillic_General_100_CI_AS");

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__bookings__3213E83FA162F58A");

            entity.ToTable("bookings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CoworkingSpaceId).HasColumnName("coworking_space_id");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CoworkingSpace).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CoworkingSpaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__cowork__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__bookings__user_i__656C112C");
        });

        modelBuilder.Entity<BookingsFacility>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.FacilityId }).HasName("PK__bookings__A6CD2B1BD2EBC312");

            entity.ToTable("bookings_facilities");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingsFacilities)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__bookings___booki__6A30C649");

            entity.HasOne(d => d.Facility).WithMany(p => p.BookingsFacilities)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("FK__bookings___facil__6B24EA82");
        });

        modelBuilder.Entity<CoworkingFacilityPrice>(entity =>
        {
            entity.HasKey(e => new { e.CoworkingSpaceId, e.FacilityId }).HasName("PK__coworkin__893C2A71B09BC337");

            entity.ToTable("coworking_facility_prices");

            entity.Property(e => e.CoworkingSpaceId).HasColumnName("coworking_space_id");
            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.CoworkingSpace).WithMany(p => p.CoworkingFacilityPrices)
                .HasForeignKey(d => d.CoworkingSpaceId)
                .HasConstraintName("FK__coworking__cowor__4316F928");

            entity.HasOne(d => d.Facility).WithMany(p => p.CoworkingFacilityPrices)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("FK__coworking__facil__440B1D61");
        });

        modelBuilder.Entity<CoworkingSpace>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__coworkin__3213E83F06EA49CC");

            entity.ToTable("coworking_spaces");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.HourlyRate)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("hourly_rate");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__faciliti__3213E83F43BCA915");

            entity.ToTable("facilities");

            entity.HasIndex(e => e.Name, "UQ__faciliti__72E12F1B7FB757DC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83FC8A94D43");

            entity.ToTable("payments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__payments__bookin__71D1E811");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__reviews__3213E83F5C200BF1");

            entity.ToTable("reviews");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CoworkingSpaceId).HasColumnName("coworking_space_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reviews__booking__09A971A2");

            entity.HasOne(d => d.CoworkingSpace).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CoworkingSpaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reviews__coworki__08B54D69");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__reviews__user_id__07C12930");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
