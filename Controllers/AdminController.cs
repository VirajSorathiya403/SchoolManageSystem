using Microsoft.AspNetCore.Mvc;

namespace SchoolManageSys.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Admin Dashboard";
        return View();
    }
}