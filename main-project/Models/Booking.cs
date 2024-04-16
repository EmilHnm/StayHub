namespace main_project.Models;

public class Booking
{
    public int Id { get; set; }
    public int RenterId { get; set; }
    public int HostelId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "pending";
    public int? LeaseContractId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Hostel? Hostel { get; set; }
    public RenterProfile? RenterProfile { get; set; }

    public static IQueryable<Booking> Query()
    {
        DatabaseContext db = new DatabaseContext();
        return db.bookings;
    }
}