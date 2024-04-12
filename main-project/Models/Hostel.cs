using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace main_project.Models;

public class Hostel
{
    [Key]
    public int Id { get; set; }
    public int HostId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public int? Rooms { get; set; }
    public int? Fee { get; set; }
    public string Status { get; set; } = "active";
    public int AccommodationTypeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<HostelImages>? HostelImages { get; set; }
    public virtual HostProfile? HostProfile { get; set; }
    public virtual AccommodationType? AccommodationType { get; set; }
    public virtual ICollection<Utility>? Utilities { get; set; }
    public virtual ICollection<HostelUtility>? HostelUtility { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
    public virtual ICollection<RenterProfile>? BookedBy { get; set; }
    public virtual ICollection<LeaseContract>? LeaseContracts { get; set; }
    public virtual ICollection<RenterProfile>? RentedBy { get; set; }

    public static IQueryable<Hostel> Query()
    {
        DatabaseContext db = new DatabaseContext();
        return db.hostels;
    }
}