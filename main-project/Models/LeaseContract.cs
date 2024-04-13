namespace main_project.Models;

public class LeaseContract
{
    public int Id { get; set; }
    public int RenterId { get; set; }
    public int HostelId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Deposit { get; set; }
    public int MonthlyRent { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual RenterProfile? Renter { get; set; }
    public virtual Hostel? Hostel { get; set; }

    public static IQueryable<LeaseContract> Query()
    {
        DatabaseContext db = new DatabaseContext();
        return db.leaseContracts;
    }
}