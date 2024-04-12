namespace main_project.Controllers;

using System.Text.RegularExpressions;
using main_project.Middleware;
using main_project.Models.ViewModels;
using main_project.Service;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    [Authentication]
    public IActionResult Index()
    {
        ViewData["HostProfile"] = AccountService.Instance().GetHostProfile(HttpContext);
        ViewData["RenterProfile"] = AccountService.Instance().GetRenterProfile(HttpContext);
        return View();
    }
    [Authentication]

    public JsonResult CreateHostProfile([FromBody] VM_HostProfileBody vm)
    {

        if (ModelState.IsValid)
        {
            if (AccountService.Instance().CreateHostProfile(vm, HttpContext) != null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new List<string> { "Có lỗi xảy ra, vui lòng thử lại sau" } });
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
    [Authentication]

    public JsonResult UpdateHostProfile([FromBody] VM_HostProfileBody vm)
    {
        if (ModelState.IsValid)
        {
            if (AccountService.Instance().UpdateHostProfile(vm, HttpContext) != null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
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

    [Authentication]

    public JsonResult CreateRenterProfile([FromBody] VM_RenterProfileBody vm)
    {

        if (ModelState.IsValid)
        {
            if (AccountService.Instance().CreateRenterProfile(vm, HttpContext) != null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new List<string> { "Có lỗi xảy ra, vui lòng thử lại sau" } });
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

    public JsonResult UpdateRenterProfile([FromBody] VM_RenterProfileBody vm)
    {
        if (ModelState.IsValid)
        {
            if (AccountService.Instance().UpdateRenterProfile(vm, HttpContext) != null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
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
    [Authentication]
    public JsonResult ChangePassword([FromBody] VM_ChangePasswordBody vm)
    {
        if (ModelState.IsValid)
        {
            if (AccountService.Instance().ChangePassword(vm, HttpContext) != null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new List<string> { "Mật khẩu cũ không đúng hoặc tài khoản không tồn tại" } });
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
}