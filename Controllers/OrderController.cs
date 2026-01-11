using Microsoft.AspNetCore.Mvc;
using newltweb.Models;

namespace newltweb.Controllers
{
    public class OrderController : Controller
    {
        private readonly LtwebBtlContext _db;

        public IActionResult Confirm(int id)
        {
            return View(model: (int?)id);
        }
    }

}
