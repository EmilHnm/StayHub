using System.ComponentModel.DataAnnotations;

namespace main_project.Models;

public class Utility
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<HostelUtility>? HostelUtility { get; set; }
    public virtual ICollection<Hostel>? Hostels { get; set; }

    public static IQueryable<Utility> Query()
    {
        return new DatabaseContext().utilities;
    }
}