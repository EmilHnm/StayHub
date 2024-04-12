using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using main_project.Models;
using main_project.Service;
using main_project.Models.ViewModels;
using main_project.Middleware;

namespace main_project.Controllers;

public class HostelController : Controller
{

    public IActionResult Index()
    {
        int page = 1;
        int itemsPerPage = 12;
        string? query = "";
        int type = 0;

        if (Request.Query["page"].Count > 0 && int.TryParse(Request.Query["page"], out page))
        {
            page = int.TryParse(Request.Query["page"], out page) ? page : 1;
        }
        if (Request.Query["query"].Count > 0)
        {
            query = Request.Query["query"];
        }
        if (Request.Query["type"].Count > 0 && int.TryParse(Request.Query["type"], out type))
        {
            type = int.TryParse(Request.Query["type"], out type) ? type : 0;
        }
        ViewData["type"] = type;
        ViewData["query"] = query;
        ViewData["accommodationTypes"] = AccommodationTypeService.Instance().GetAccommodationTypes();
        VM_Pagination<Hostel> hostels = VM_Pagination<Hostel>.Create(HostelService.Instance().Search(query, type).AsQueryable(), page, itemsPerPage, "/Hostel/Index");
        return View(hostels);
    }
    public IActionResult Details(int id)
    {
        Hostel? hostel = HostelService.Instance().GetHostel(id);
        if (hostel == null)
        {
            return NotFound();
        }
        else
        {
            return View(hostel);
        }
    }
    [HttpGet]
    [HasHostProfile]
    public IActionResult Create()
    {
        ViewData["accommodationTypes"] = AccommodationTypeService.Instance().GetAccommodationTypes();
        ViewData["Utilities"] = UtilityService.Instance().GetUtilities();
        ViewData["HostProfile"] = AccountService.Instance().GetHostProfile(HttpContext);
        return View();
    }

    [HttpPost]
    [HasHostProfile]
    public IActionResult Create([FromForm] VM_HostelForm vm)
    {

        if (!ModelState.IsValid)
        {
            ViewData["accommodationTypes"] = AccommodationTypeService.Instance().GetAccommodationTypes();
            ViewData["Utilities"] = UtilityService.Instance().GetUtilities();
            return View(vm);
        }
        int hostId = AccountService.Instance().GetHostProfile(HttpContext)?.Id ?? 0;
        if (hostId == 0)
        {
            return NotFound();
        }
        else
        {
            Hostel hostel = new Hostel()
            {
                Name = vm.Name,
                Address = vm.Address,
                Description = vm.Description,
                Rooms = vm.Rooms,
                Fee = vm.Fee,
                AccommodationTypeId = vm.AccommodationTypeId,
                HostId = hostId
            };

            List<string> paths = HostelService.Instance().SaveImages(vm.ImageFiles);

            Hostel? createdHostel = HostelService.Instance().CreateHostel(hostel, paths, vm.Utilities);
            if (createdHostel != null)
            {
                foreach (string path in paths)
                {
                    HostelService.Instance().SaveHostelImage(createdHostel.Id, path);
                }
                return RedirectToAction("Details", new { id = createdHostel.Id });
            }
            else
            {
                ViewData["accommodationTypes"] = AccommodationTypeService.Instance().GetAccommodationTypes();
                ViewData["Utilities"] = UtilityService.Instance().GetUtilities();
                return View(vm);
            }
        }
    }
    [Authentication]
    [HasHostProfile]
    public IActionResult Manage()
    {
        int hostId = AccountService.Instance().GetHostProfile(HttpContext)?.Id ?? 0;
        if (hostId == 0)
        {
            return NotFound();
        }
        else
        {
            int page = 1;
            int itemsPerPage = 10;
            if (Request.Query["page"].Count > 0 && int.TryParse(Request.Query["page"], out page))
            {
                page = int.TryParse(Request.Query["page"], out page) ? page : 1;
            }
            VM_Pagination<Hostel> hostels = VM_Pagination<Hostel>.Create(HostelService.Instance().GetHostelsByHostId(hostId).AsQueryable(), page, itemsPerPage, "/Hostel/Manage");
            return View(hostels);
        }
    }
    [Authentication]
    [HasHostProfile]
    public IActionResult Edit(int id)
    {
        Hostel? hostel = HostelService.Instance().GetHostel(id);
        if (hostel == null)
        {
            return NotFound();
        }
        else
        {
            ViewData["accommodationTypes"] = AccommodationTypeService.Instance().GetAccommodationTypes();
            ViewData["Utilities"] = UtilityService.Instance().GetUtilities();
            ViewData["HostProfile"] = AccountService.Instance().GetHostProfile(HttpContext);
            VM_HostelForm vm = new VM_HostelForm()
            {
                Id = hostel.Id,
                Name = hostel.Name ?? "",
                Address = hostel.Address ?? "",
                Description = hostel.Description ?? "",
                Rooms = hostel.Rooms ?? 0,
                Fee = hostel.Fee ?? 0,
                AccommodationTypeId = hostel.AccommodationTypeId,
                Status = hostel.Status,
                ImageFileURLs = hostel.HostelImages?.Select(image => image.ImageUrl ?? "").ToList() ?? new List<string>(),
                Utilities = hostel.HostelUtility?.AsQueryable().Select(utility => utility.UtilityId).ToList() ?? new List<int>(),
            };
            return View(vm);
        }
    }
    [Authentication]
    [HttpPost]
    [HasHostProfile]
    public IActionResult Update([FromForm] VM_HostelForm vm)
    {

        if (!ModelState.IsValid)
        {
            ViewData["accommodationTypes"] = AccommodationTypeService.Instance().GetAccommodationTypes();
            ViewData["Utilities"] = UtilityService.Instance().GetUtilities();
            return View("Edit", new { id = vm.Id });
        }
        int hostId = AccountService.Instance().GetHostProfile(HttpContext)?.Id ?? 0;
        if (hostId == 0 || vm.Id == null)
        {
            Console.WriteLine(hostId);
            return NotFound();
        }
        else
        {
            if (HostelService.Instance().UpdateHostel(vm) == null)
            {
                return NotFound();
            }
            return RedirectToAction("Manage");
        }
    }
    [Authentication]
    [HasHostProfile]
    public IActionResult Delete(int id)
    {
        Hostel? hostel = HostelService.Instance().GetHostel(id);
        if (hostel == null)
        {
            return NotFound();
        }
        else
        {
            if (hostel.HostId != AccountService.Instance().GetHostProfile(HttpContext)?.Id)
            {
                return NotFound();
            }
            HostelService.Instance().DeleteHostel(id);
            return RedirectToAction("Manage");
        }
    }
}