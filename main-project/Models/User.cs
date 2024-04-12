using Microsoft.EntityFrameworkCore;

namespace main_project.Models;

public class User
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual RenterProfile? RenterProfile { get; set; }
    public virtual HostProfile? HostProfile { get; set; }

    public static IQueryable<User> Query()
    {
        return new DatabaseContext().users;
    }
}