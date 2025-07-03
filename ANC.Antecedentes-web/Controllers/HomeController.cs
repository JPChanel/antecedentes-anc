using ANC.AntecedentesWeb.Models;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace ANC.AntecedentesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Salir();
            return View();
        }



        public IActionResult Error(int? statusCode)
        {
            return View("Error"); // Muestra la vista de error personalizado
        }



        public ActionResult Salir()
        {
            HttpContext.Session.SetString("ssgenerico", "");
            HttpContext.Session.SetString("ssglobal", "");
            HttpContext.Session.SetString("TokenSesion", "");

            HttpContext.Session.Remove("ssgenerico");
            HttpContext.Session.Remove("ssglobal");
            HttpContext.Session.Remove("TokenSesion");
            

            return RedirectToAction("Index", "Home");
        }

    }
}