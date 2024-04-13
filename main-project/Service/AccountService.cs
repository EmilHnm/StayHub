using main_project.Models;
using main_project.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace main_project.Service;


public class AccountService
{
    private static AccountService? _instance = null;

    private AccountService()
    {
    }
    public static AccountService Instance()
    {
        return _instance ??= new AccountService();
    }

    public bool HasHostProfile(HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            return user.HostProfile != null;
        }
        return false;
    }

    public HostProfile? GetHostProfile(HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            return user.HostProfile;
        }
        return null;
    }

    public bool HasRenterProfile(HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            return user.RenterProfile != null;
        }
        return false;
    }

    public RenterProfile? GetRenterProfile(HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            return user.RenterProfile;
        }
        return null;
    }

    public HostProfile? CreateHostProfile(VM_HostProfileBody vm, HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            if (user.HostProfile != null)
            {
                return user.HostProfile;
            }
            HostProfile hostProfile = new HostProfile()
            {
                UserId = user.Id,
                FullName = vm.fullname,
                Phone = vm.phone,
            };
            DatabaseContext db = new DatabaseContext();
            db.hostProfiles.Add(hostProfile);
            db.SaveChanges();
            return hostProfile;
        }
        return null;
    }

    public HostProfile? UpdateHostProfile(VM_HostProfileBody vm, HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            if (user.HostProfile != null)
            {
                HostProfile hostProfile = user.HostProfile;
                hostProfile.FullName = vm.fullname;
                hostProfile.Phone = vm.phone;
                hostProfile.UpdatedAt = DateTime.UtcNow;
                DatabaseContext db = new DatabaseContext();
                db.Entry(hostProfile).State = EntityState.Modified;
                db.SaveChanges();
                return hostProfile;
            }
            return null;
        }
        return null;
    }

    public RenterProfile? CreateRenterProfile(VM_RenterProfileBody vm, HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            if (user.RenterProfile != null)
            {
                return user.RenterProfile;
            }
            RenterProfile hostProfile = new RenterProfile()
            {
                UserId = user.Id,
                FullName = vm.fullname,
                Phone = vm.phone,
                Gender = vm.gender,
                Address = vm.address,
                IdentityCardNumber = vm.identityCardNumber,
            };
            DatabaseContext db = new DatabaseContext();
            db.renterProfiles.Add(hostProfile);
            db.SaveChanges();
            return hostProfile;
        }
        return null;
    }

    public RenterProfile? UpdateRenterProfile(VM_RenterProfileBody vm, HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            if (user.RenterProfile != null)
            {
                RenterProfile renterProfile = user.RenterProfile;
                renterProfile.FullName = vm.fullname;
                renterProfile.Gender = vm.gender;
                renterProfile.Phone = vm.phone;
                renterProfile.Address = vm.address;
                renterProfile.IdentityCardNumber = vm.identityCardNumber;
                renterProfile.UpdatedAt = DateTime.UtcNow;
                DatabaseContext db = new DatabaseContext();
                db.Entry(renterProfile).State = EntityState.Modified;
                db.SaveChanges();
                return renterProfile;
            }
            return null;
        }
        return null;
    }

    public User? ChangePassword(VM_ChangePasswordBody vm, HttpContext context)
    {
        User? user = AuthService.GetAuthenticatedUser(context);
        if (user != null)
        {
            if (vm.currentPassword == user.Password)
            {
                user.Password = vm.newPassword;
                user.UpdatedAt = DateTime.UtcNow;
                DatabaseContext db = new DatabaseContext();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return user;
            }
        }
        return null;
    }
}