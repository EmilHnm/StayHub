namespace main_project.Models;

public class AccommodationType
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Hostel>? Hotels { get; set; }
    public static IQueryable<AccommodationType> Query()
    {
        DatabaseContext db = new DatabaseContext();
        return db.accommodationTypes;
    }
}