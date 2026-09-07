using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsBookingSystem.Models
{
    [Table("MEMBER")]
    public class Member
    {
        [Key]
        [Column("MEMBERID")]
        public int MemberID { get; set; }

        [Required, MaxLength(50)]
        [Column("FIRSTNAME")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("LASTNAME")]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        [Column("PHONE")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("ADDRESS")]
        public string? Address { get; set; }

        [Required, MaxLength(255)]
        [Column("PASSWORDHASH")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("REGISTRATIONDATE")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<MemberSport> MemberSports { get; set; } = new List<MemberSport>();
    }

    [Table("SPORT")]
    public class Sport
    {
        [Key]
        [Column("SPORTID")]
        public int SportID { get; set; }

        [Required, MaxLength(50)]
        [Column("SPORTNAME")]
        public string SportName { get; set; } = string.Empty;

        public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
        public ICollection<MemberSport> MemberSports { get; set; } = new List<MemberSport>();
    }

    [Table("MEMBER_SPORT")]
    public class MemberSport
    {
        [Column("MEMBERID")]
        public int MemberID { get; set; }
        public Member? Member { get; set; }

        [Column("SPORTID")]
        public int SportID { get; set; }
        public Sport? Sport { get; set; }
    }

    [Table("FACILITY")]
    public class Facility
    {
        [Key]
        [Column("FACILITYID")]
        public int FacilityID { get; set; }

        [Required, MaxLength(100)]
        [Column("FACILITYNAME")]
        public string FacilityName { get; set; } = string.Empty;

        [Column("SPORTID")]
        public int SportID { get; set; }
        public Sport? Sport { get; set; }

        [Required, MaxLength(150)]
        [Column("LOCATION")]
        public string Location { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; }

        [Column("CAPACITY")]
        public int? Capacity { get; set; }

        [Required, MaxLength(20)]
        [Column("STATUS")]
        public string Status { get; set; } = "Available";

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    [Table("BOOKING")]
    public class Booking
    {
        [Key]
        [Column("BOOKINGID")]
        public int BookingID { get; set; }

        [Column("MEMBERID")]
        public int MemberID { get; set; }
        public Member? Member { get; set; }

        [Column("FACILITYID")]
        public int FacilityID { get; set; }
        public Facility? Facility { get; set; }

        [Column("BOOKINGDATE")]
        public DateTime BookingDate { get; set; }

        [Required, MaxLength(5)]
        [Column("STARTTIME")]
        public string StartTime { get; set; } = string.Empty;

        [Required, MaxLength(5)]
        [Column("ENDTIME")]
        public string EndTime { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        [Column("STATUS")]
        public string Status { get; set; } = "Pending";

        [Column("CREATEDAT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Review? Review { get; set; }
    }

    [Table("REVIEW")]
    public class Review
    {
        [Key]
        [Column("REVIEWID")]
        public int ReviewID { get; set; }

        [Column("MEMBERID")]
        public int MemberID { get; set; }
        public Member? Member { get; set; }

        [Column("FACILITYID")]
        public int FacilityID { get; set; }
        public Facility? Facility { get; set; }

        [Column("BOOKINGID")]
        public int? BookingID { get; set; }
        public Booking? Booking { get; set; }

        [Range(1, 5)]
        [Column("RATING")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        [Column("COMMENTTEXT")]
        public string? Comment { get; set; }

        [Column("REVIEWDATE")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }

    [Table("INQUIRY")]
    public class Inquiry
    {
        [Key]
        [Column("INQUIRYID")]
        public int InquiryID { get; set; }

        [Required, MaxLength(100)]
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        [Column("MESSAGE")]
        public string Message { get; set; } = string.Empty;

        [Column("SUBMITTEDDATE")]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;
    }
}
