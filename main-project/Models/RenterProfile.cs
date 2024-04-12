using System.ComponentModel.DataAnnotations.Schema;

namespace main_project.Models;

public class RenterProfile
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public string? FullName { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? IdentityCardNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
    public virtual ICollection<Hostel>? BookingHostels { get; set; }
    public virtual ICollection<LeaseContract>? LeaseContracts { get; set; }
    public virtual ICollection<Hostel>? RentedHostels { get; set; }
}