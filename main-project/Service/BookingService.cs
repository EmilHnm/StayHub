using main_project.Controllers;
using main_project.Models;
using main_project.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace main_project.Service;
public class BookingService
{
    private static BookingService? _instance = null;
    private BookingService()
    {
    }

    public static BookingService Instance()
    {
        return _instance ??= new BookingService();
    }

    public Booking? CreateBooking(VM_BookingTicket ticket, HttpContext context)
    {
        Hostel? hostel = HostelService.Instance().GetHostel(ticket.HostelId);
        if (hostel == null)
        {
            return null;
        }
        int? renterId = AccountService.Instance().GetRenterProfile(context)?.Id;
        if (renterId == null)
        {
            return null;
        }
        else
        {
            Booking? oldBooked = Booking.Query().Where(b => (b.HostelId == hostel.Id && b.RenterId == renterId && b.Status == "pending") || (b.HostelId == hostel.Id &&
                b.RenterId == renterId &&
                b.StartDate >= ticket.StartDate &&
                b.EndDate <= ticket.EndDate &&
                b.Status == "approved")).FirstOrDefault();
            if (oldBooked != null)
            {
                return null;
            }
            LeaseContract? oldLease = LeaseContract.Query().Where(l => l.HostelId == hostel.Id && l.RenterId == renterId &&
            (l.Status == "active" || (l.StartDate >= ticket.StartDate && l.EndDate <= ticket.EndDate && l.Status == "active"))
            ).FirstOrDefault();
            if (oldLease != null)
            {
                return null;
            }
            Booking booking = new Booking()
            {
                HostelId = hostel.Id,
                RenterId = renterId ?? 0,
                Status = "pending",
                StartDate = ticket.StartDate,
                EndDate = ticket.EndDate,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            DatabaseContext db = new DatabaseContext();
            db.bookings.Add(booking);
            db.SaveChanges();
            return booking;
        }
    }

    public List<Booking> GetBooked(HttpContext context)
    {
        int? renterId = AccountService.Instance().GetRenterProfile(context)?.Id;
        if (renterId == null)
        {
            return new List<Booking>();
        }
        return Booking.Query().Include(b => b.Hostel).Where(b => b.RenterId == renterId)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
    }

    public bool Cancel(int id, HttpContext context)
    {
        try
        {
            //
            int? renterId = AccountService.Instance().GetRenterProfile(context)?.Id;
            if (renterId == null)
            {
                return false;
            }
            Booking? booking = Booking.Query().Where(b => b.Id == id && b.RenterId == renterId).FirstOrDefault();
            if (booking == null)
            {
                return false;
            }
            booking.Status = "cancelled";
            booking.UpdatedAt = DateTime.Now;
            DatabaseContext db = new DatabaseContext();
            db.bookings.Update(booking);
            db.SaveChanges();
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public Booking? Approve(int id, HttpContext context)
    {
        try
        {
            Booking? booking = Booking.Query().Where(b => b.Id == id).Include(b => b.Hostel).FirstOrDefault();
            if (booking == null)
            {
                return null;
            }
            if (booking.Status != "pending")
            {
                return null;
            }
            if (booking.Hostel?.Status != "active")
            {
                return null;
            }
            if (booking.Hostel?.HostId != AccountService.Instance().GetHostProfile(context)?.Id)
            {
                return null;
            }
            booking.Status = "approved";
            booking.UpdatedAt = DateTime.Now;
            DatabaseContext db = new DatabaseContext();
            db.bookings.Update(booking);
            db.SaveChanges();

            List<Booking> otherBookings = Booking.Query().Where(b => b.HostelId == booking.HostelId && b.Status == "pending" && b.StartDate >= booking.StartDate && b.EndDate <= booking.EndDate).ToList();
            foreach (Booking otherBooking in otherBookings)
            {
                otherBooking.Status = "rejected";
                otherBooking.UpdatedAt = DateTime.Now;
                db.bookings.Update(otherBooking);
            }
            db.SaveChanges();
            return booking;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public Booking? Reject(int id, HttpContext context)
    {
        try
        {
            Booking? booking = Booking.Query().Where(b => b.Id == id).Include(b => b.Hostel).FirstOrDefault();
            if (booking == null)
            {
                return null;
            }
            if (booking.Status != "pending")
            {
                return null;
            }
            if (booking.Hostel?.HostId != AccountService.Instance().GetHostProfile(context)?.Id)
            {
                return null;
            }
            booking.Status = "rejected";
            booking.UpdatedAt = DateTime.Now;
            DatabaseContext db = new DatabaseContext();
            db.bookings.Update(booking);
            db.SaveChanges();
            return booking;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public LeaseContract? CreateLease(int id, HttpContext context)
    {
        try
        {
            Booking? booking = Booking.Query().Where(b => b.Id == id).Include(b => b.Hostel).FirstOrDefault();
            if (booking == null)
            {
                return null;
            }
            if (booking.Status != "approved")
            {
                return null;
            }
            if (booking.Hostel?.Status != "active")
            {
                return null;
            }
            if (booking.Hostel?.HostId != AccountService.Instance().GetHostProfile(context)?.Id)
            {
                return null;
            }
            LeaseContract contract = new LeaseContract()
            {
                RenterId = booking.RenterId,
                HostelId = booking.HostelId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = "active",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            DatabaseContext db = new DatabaseContext();
            db.leaseContracts.Add(contract);
            db.bookings.Remove(booking);
            db.SaveChanges();
            return contract;
        }
        catch (System.Exception)
        {

            return null;
        }
    }


    public LeaseContract? GetLease(int id)
    {
        return LeaseContract.Query().Where(l => l.Id == id).Include(l => l.Hostel).Include(l => l.Renter).FirstOrDefault();
    }


    public LeaseContract? CancelLease(int id, HttpContext context)
    {
        try
        {
            LeaseContract? contract = LeaseContract.Query().Where(c => c.Id == id).Include(c => c.Hostel).FirstOrDefault();
            if (contract == null)
            {
                return null;
            }
            if (contract.Status != "active")
            {
                return null;
            }
            if (contract.Hostel?.HostId != AccountService.Instance().GetHostProfile(context)?.Id)
            {
                return null;
            }
            if (contract.StartDate < DateTime.Now)
            {
                return null;
            }
            contract.Status = "cancelled";
            contract.UpdatedAt = DateTime.Now;
            DatabaseContext db = new DatabaseContext();
            db.leaseContracts.Update(contract);
            db.SaveChanges();
            return contract;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public LeaseContract? ExtendLease(VM_ExtendLease vm, HttpContext context)
    {
        try
        {
            LeaseContract? lease = LeaseContract.Query().Where(l => l.Id == vm.LeaseId).FirstOrDefault();
            if (lease == null)
            {
                return null;
            }

            lease.EndDate = vm.NewEndDate;
            lease.UpdatedAt = DateTime.Now;
            DatabaseContext db = new DatabaseContext();
            db.leaseContracts.Update(lease);
            db.SaveChanges();
            return lease;
        }
        catch (System.Exception)
        {
            return null;
        }
    }
}