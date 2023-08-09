using Microsoft.AspNetCore.Mvc;

namespace crudBundle.Controllers
{
    public class HomeController : Controller
    {
        [Route("Error")]
        public IActionResult Error()
        {
            return View(); //Views/shared/error
        }
    }
}
