namespace main_project.Controllers;

using main_project.Models;
using Microsoft.AspNetCore.Mvc;

public class BookingController : Controller
{
    public JsonResult Create()
    {
        return Json(new
        {
            status = "success",
            data = new Booking(),
            message = "Booking created successfully"
        });
    }
}