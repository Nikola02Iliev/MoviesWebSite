using Microsoft.AspNetCore.Mvc;

namespace MoviesWebSite.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult ReviewRestriction()
        {
            return View();
        }

        public IActionResult RatingRestriction()
        {
            return View();
        }
    }
}
