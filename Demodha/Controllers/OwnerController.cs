using Microsoft.AspNetCore.Mvc;

namespace Demodha.Controllers
{
    public class OwnerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
