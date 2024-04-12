namespace main_project.Models;

public class HostelImages
{
    public int Id { get; set; }
    public int HostelId { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual Hostel? Hostel { get; set; }
    public static IQueryable<HostelImages> Query()
    {
        DatabaseContext db = new DatabaseContext();
        return db.hostelImages;
    }
}