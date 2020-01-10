using Microsoft.AspNetCore.Mvc;

namespace AkkaMjrTwo.UI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
