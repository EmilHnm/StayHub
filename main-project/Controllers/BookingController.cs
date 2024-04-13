namespace main_project.Controllers;

using System.ComponentModel.DataAnnotations;
using System.Text;
using main_project.Middleware;
using main_project.Models;
using main_project.Models.ViewModels;
using main_project.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class BookingController : Controller
{
    [HasRenterProfile]
    [HttpPost]
    public JsonResult Create([FromBody] VM_BookingTicket vm)
    {

        if (ModelState.IsValid)
        {
            Booking? booking = BookingService.Instance().CreateBooking(vm, HttpContext);
            if (booking == null)
            {
                return Json(new
                {
                    success = false,
                });
            }
            // Return response
            return Json(new
            {
                success = "true",
            });
        }
        var errors = new List<string>();
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                errors.Add(error.ErrorMessage);
            }
        }
        return Json(new { success = false, errors });
    }
    [HasRenterProfile]
    public IActionResult BookedList()
    {
        List<Booking> bookings = BookingService.Instance().GetBooked(HttpContext);
        return View(bookings);
    }
    [HasRenterProfile]
    public IActionResult Cancel(int id)
    {
        BookingService.Instance().Cancel(id, HttpContext);
        return Redirect(Request.Headers["Referer"]);
    }
    [HasHostProfile]
    public IActionResult Approve(int id)
    {
        Booking? result = BookingService.Instance().Approve(id, HttpContext);
        if (result != null)
        {
            if (BookingService.Instance().CreateLease(id, HttpContext) != null)
            {
                return Redirect(Request.Headers["Referer"]);
            }
            return NotFound();
        }
        else
        {
            return NotFound();
        }
    }

    [HasHostProfile]
    public IActionResult Reject(int id)
    {
        Booking? result = BookingService.Instance().Reject(id, HttpContext);
        if (result != null)
        {
            return Redirect(Request.Headers["Referer"]);
        }
        else
        {
            return NotFound();
        }
    }

    [HasHostProfile]
    public IActionResult CancelLease(int id)
    {
        LeaseContract? result = BookingService.Instance().CancelLease(id, HttpContext);
        if (result != null)
        {
            return Redirect(Request.Headers["Referer"]);
        }
        else
        {
            return NotFound();
        }
    }
    [HasHostProfile]
    [HttpPost]
    public JsonResult ExtendLease([FromBody] VM_ExtendLease vm)
    {
        var errors = new List<string>();
        if (ModelState.IsValid)
        {
            LeaseContract? lease = BookingService.Instance().GetLease(vm.LeaseId);
            if (lease == null)
            {
                errors.Add("Không tìm thấy hợp đồng");
                return Json(new { success = false, errors });
            }
            if (lease.Hostel?.HostId != AccountService.Instance().GetHostProfile(HttpContext)?.Id)
            {
                errors.Add("Bạn không có quyền gia hạn hợp đồng này");
                return Json(new { success = false, errors });
            }
            Console.WriteLine(lease.EndDate);
            Console.WriteLine(vm.NewEndDate);
            Console.WriteLine(lease.EndDate >= vm.NewEndDate);

            if (lease.EndDate >= vm.NewEndDate)
            {
                errors.Add("Ngày kết thúc mới phải lớn hơn ngày kết thúc hiện tại của hợp đồng");
                return Json(new { success = false, errors });
            }

            LeaseContract? result = BookingService.Instance().ExtendLease(vm, HttpContext);
            if (result != null)
            {
                return Json(new
                {
                    success = "true",
                });
            }
        }

        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                errors.Add(error.ErrorMessage);
            }
        }
        return Json(new { success = false, errors });
    }
}