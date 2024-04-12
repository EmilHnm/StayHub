using main_project.Models;
using main_project.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace main_project.Service;
public class AuthService
{
    public static bool CreateSession(HttpContext httpContext, VM_SignIn vm)
    {
        DatabaseContext db = new DatabaseContext();
        User? user = db.users.Where((u) => u.Email == vm.Email && u.Password == vm.Password).FirstOrDefault();
        if (user != null)
        {
            httpContext.Session.SetInt32("UserId", user.Id);
            return true;
        }
        return false;
    }

    public static bool CreateUser(VM_SignUp vm)
    {
        if (User.Query().Where((u) => u.Email == vm.Email).FirstOrDefault() == null)
        {
            DatabaseContext db = new DatabaseContext();
            User user = new User
            {
                Email = vm.Email,
                Password = vm.Password,
            };
            db.users.Add(user);
            db.SaveChanges();
            return true;
        }
        return false;
    }
    public static void Logout(HttpContext httpContext)
    {
        httpContext.Session.Clear();
        httpContext.Session.Remove("UserId");
    }

    public static bool IsAuthenticated(HttpContext httpContext)
    {
        if (httpContext.Session.GetInt32("UserId") != null)
        {
            return true;
        }
        return false;
    }

    public static User? GetAuthenticatedUser(HttpContext httpContext)
    {
        DatabaseContext db = new DatabaseContext();
        int? userId = httpContext.Session.GetInt32("UserId");
        if (userId != null)
        {
            return db.users
                .Include(u => u.RenterProfile)
                .Include(u => u.HostProfile)
                .Where((u) => u.Id == userId)
                .FirstOrDefault();
        }
        return null;
    }
}