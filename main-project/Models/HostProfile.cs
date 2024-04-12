using System.ComponentModel.DataAnnotations.Schema;

namespace main_project.Models;
public class HostProfile
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    public virtual ICollection<Hostel>? Hostels { get; set; }
}