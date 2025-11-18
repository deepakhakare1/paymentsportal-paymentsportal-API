using Microsoft.AspNetCore.Mvc;

namespace PaymentsApi.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
