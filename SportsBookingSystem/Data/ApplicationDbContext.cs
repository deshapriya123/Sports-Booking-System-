using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; } = null!;
        public DbSet<Sport> Sports { get; set; } = null!;
        public DbSet<MemberSport> MemberSports { get; set; } = null!;
        public DbSet<Facility> Facilities { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Inquiry> Inquiries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map each entity to its actual Oracle table name (uppercase, as created by schema.sql).
            // Without this, EF Core defaults to the DbSet property name ("Bookings"), which does
            // not match the real table ("BOOKING") and causes ORA-00942.
            modelBuilder.Entity<Member>().ToTable("MEMBER");
            modelBuilder.Entity<Sport>().ToTable("SPORT");
            modelBuilder.Entity<MemberSport>().ToTable("MEMBER_SPORT");
            modelBuilder.Entity<Facility>().ToTable("FACILITY");
            modelBuilder.Entity<Booking>().ToTable("BOOKING");
            modelBuilder.Entity<Review>().ToTable("REVIEW");
            modelBuilder.Entity<Inquiry>().ToTable("INQUIRY");

            // MEMBER_SPORT composite key (many-to-many junction)
            modelBuilder.Entity<MemberSport>()
                .HasKey(ms => new { ms.MemberID, ms.SportID });

            modelBuilder.Entity<MemberSport>()
                .HasOne(ms => ms.Member)
                .WithMany(m => m.MemberSports)
                .HasForeignKey(ms => ms.MemberID);

            modelBuilder.Entity<MemberSport>()
                .HasOne(ms => ms.Sport)
                .WithMany(s => s.MemberSports)
                .HasForeignKey(ms => ms.SportID);

            // MEMBER email must be unique
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Email)
                .IsUnique();

            // SPORT name must be unique
            modelBuilder.Entity<Sport>()
                .HasIndex(s => s.SportName)
                .IsUnique();

            // FACILITY -> SPORT (many facilities per sport)
            modelBuilder.Entity<Facility>()
                .HasOne(f => f.Sport)
                .WithMany(s => s.Facilities)
                .HasForeignKey(f => f.SportID);

            // BOOKING -> MEMBER / FACILITY
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Member)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MemberID);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Facility)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FacilityID);

            // REVIEW -> MEMBER / FACILITY / BOOKING (optional 1:1 with Booking)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Member)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MemberID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Facility)
                .WithMany(f => f.Reviews)
                .HasForeignKey(r => r.FacilityID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithOne(b => b.Review)
                .HasForeignKey<Review>(r => r.BookingID);
        }
    }
}
