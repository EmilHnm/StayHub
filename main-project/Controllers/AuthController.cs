using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using main_project.Models;
using main_project.Models.ViewModels;
using main_project.Service;
using main_project.Middleware;

namespace main_project.Controllers;

public class AuthController : Controller
{
    [Guest]
    public IActionResult SignIn()
    {
        if (AuthService.IsAuthenticated(HttpContext))
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [Guest]
    public IActionResult SignIn([FromForm] VM_SignIn data)
    {
        if (ModelState.IsValid)
        {
            if (AuthService.CreateSession(HttpContext, data))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Email", "Email or password is incorrect");
            }
            return View();
        }
        return View();
    }
    [Guest]
    public IActionResult SignUp()
    {
        return View();
    }

    [Guest]
    [HttpPost]
    public IActionResult SignUp([FromForm] VM_SignUp data)
    {
        if (ModelState.IsValid)
        {
            if (AuthService.CreateUser(data))
            {
                return RedirectToAction("SignIn", "Auth");
            }
        }
        return View();
    }

    [Authentication]
    public IActionResult LogOut()
    {
        AuthService.Logout(HttpContext);
        return RedirectToAction("SignIn", "Auth");
    }
}