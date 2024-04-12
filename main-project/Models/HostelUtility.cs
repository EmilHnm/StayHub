using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace main_project.Models;

public class HostelUtility
{
    public int HostelId { get; set; }
    public int UtilityId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual Hostel? Hostels { get; set; }
    public virtual Utility? Utility { get; set; }
    public static IQueryable<HostelUtility> Query()
    {
        return new DatabaseContext().hostelUtilities;
    }

}